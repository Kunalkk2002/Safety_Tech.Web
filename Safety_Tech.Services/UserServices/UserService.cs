using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Safety_Tech.DTOs;
using Safety_Tech.DTOs.UserDTOs;
using Safety_Tech.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Safety_Tech.DTOs.RoleDTOs;
using BCrypt.Net;
using System.Security.Cryptography;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;

namespace Safety_Tech.Services.UserServices
{
    /// <summary>
    /// Service implementation for user-related business logic.
    /// </summary>
    public class UserService : IUserService
    {

        private readonly ILogger<UserService> logger;// Dependency injection for ILogger
        private readonly IMapper mapper;// Dependency injection for IMapper
        private readonly ApplicationDataContext dataContext; // Dependency injection for DataContext
        private readonly IDistributedCache distributedCache; // Distributed cache for caching users


        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="logger">Logger instance for logging.</param>
        /// <param name="mapper">Mapper instance for object mapping.</param>
        /// <param name="dataContext">Database context for data access.</param>
        public UserService(ILogger<UserService> logger, IMapper mapper, ApplicationDataContext dataContext, IDistributedCache distributedCache,
            UserManager<IdentityUser> userManager,
                        RoleManager<IdentityRole> roleManager)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.dataContext = dataContext;
            this.distributedCache = distributedCache;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        /// <summary>
        /// Adds a new user to the system.
        /// </summary>
        /// <param name="newUser">The user data to add.</param>
        /// <returns>Service response with the added user DTO.</returns>
        public async Task<ServiceResponse<AddUserDto>> AddUser(AddUserDto newUser)
        {
            ServiceResponse<AddUserDto> serviceResponse = new ServiceResponse<AddUserDto>();

            try
            {
                // Check if user with the same email already exists
                var existingUser = await dataContext.Users.FirstOrDefaultAsync(u => u.UserMail == newUser.UserMail);
                if (existingUser != null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "User with this email already exists";
                    return serviceResponse;
                }

                // Hash the password before storing
                newUser.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);
                // Map DTO to entity and add to database
                var user = mapper.Map<User>(newUser);
                await dataContext.Users.AddAsync(user);
                var result = await dataContext.SaveChangesAsync();

                // Set user ID in DTO and return success response
                newUser.UserId = user.UserId;
                if (result == 1 && user.UserId > 0)
                {
                    serviceResponse.Data = newUser;
                    serviceResponse.Success = true;
                    serviceResponse.Message = "User added successfully!";
                }
                else
                {
                    serviceResponse.Data = newUser;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Something went wrong while adding the user.";
                }
            }
            catch (Exception ex)
            {
                // Log error and return failure response
                logger.LogError("Error= {@InnerException}", "InnerException = " + ex.InnerException + Environment.NewLine + "Message = " + ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            // Invalidate user cache after adding
            await InvalidateUserCache();
            return serviceResponse;
        }

        // Method to delete a user by ID
        public async Task<ServiceResponse<bool>> DeleteUser(int id)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();

            try
            {
                // Find user by ID
                var user = await dataContext.Users.FirstOrDefaultAsync(x => x.UserId == id);

                if (user == null)
                {
                    // Return failure response if user not found
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "User not found with the provided ID.";
                    return serviceResponse;
                }

                // Remove user from database and save changes
                dataContext.Users.Remove(user);
                var result = await dataContext.SaveChangesAsync();

                // Return success response if deletion successful
                if (result >= 1)
                {
                    serviceResponse.Data = true;
                    serviceResponse.Success = true;
                    serviceResponse.Message = "User deleted successfully!";
                }
                else
                {
                    // Return failure response if deletion unsuccessful
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Something went wrong while deleting the user.";
                }
            }
            catch (Exception ex)
            {
                // Log error and return failure response
                logger.LogError("Error= {@InnerException}", "InnerException = " + ex.InnerException + Environment.NewLine + "Message = " + ex.Message);
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            // Invalidate user cache after deleting
            await InvalidateUserCache();
            return serviceResponse;
        }

        // Method to get all users
        public async Task<ServiceResponse<List<GetUserDto>>> GetAllUsers(int pageNumber = 1, int pageSize = 20)
        {
            ServiceResponse<List<GetUserDto>> serviceResponse = new ServiceResponse<List<GetUserDto>>();
            string cacheKey = $"AllUsers_{pageNumber}_{pageSize}";
            var cachedUsers = await distributedCache.GetStringAsync(cacheKey);
            List<GetUserDto> users;
            if (!string.IsNullOrEmpty(cachedUsers))
            {
                users = JsonSerializer.Deserialize<List<GetUserDto>>(cachedUsers);
            }
            else
            {
                users = await dataContext.Users
                    .AsNoTracking()
                    .Select(user => new GetUserDto
                    {
                        Id = user.UserId,
                        UserMail = user.UserMail,
                        UserName = string.IsNullOrEmpty(user.UserName) ? user.UserMail : user.UserName,
                        IsActive = user.IsActive.Value
                    })
                    .OrderBy(user => user.UserName)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                await distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(users), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });
            }
            if (users?.Count > 0)
            {
                serviceResponse.Data = users;
                serviceResponse.Success = true;
                serviceResponse.Message = serviceResponse.Data.Count.ToString() + " Records found";
            }
            else
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Records not found";
            }
            return serviceResponse;
        }

        // Method to get a user by ID
        public async Task<ServiceResponse<GetUserDto>> GetUserById(int id)
        {
            logger.LogInformation("In the GetUserById method");
            ServiceResponse<GetUserDto> serviceResponse = new ServiceResponse<GetUserDto>();

            try
            {
                // Query user by ID and map to DTO
                var userEntity = await dataContext.Users.AsNoTracking().Include(u => u.Roles).FirstOrDefaultAsync(x => x.UserId == id);
                if (userEntity != null)
                {
                    var userDto = new GetUserDto
                    {
                        Id = userEntity.UserId,
                        UserMail = userEntity.UserMail,
                        UserName = userEntity.UserName,
                        IsActive = userEntity.IsActive ?? false,
                        RoleName = userEntity.Roles.FirstOrDefault()?.RoleName
                    };
                    serviceResponse.Data = userDto;
                    serviceResponse.Success = true;
                    serviceResponse.Message = "User found!";
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "User not found!";
                }
            }
            catch (Exception ex)
            {
                // Log error and return failure response
                logger.LogError("Error: {@InnerException}", ex);
                serviceResponse.Success = false;
                serviceResponse.Message = "An error occurred while fetching user details.";
            }

            return serviceResponse;
        }


        public async Task<ServiceResponse<int?>> UpdateUser(UpdateUserDto updateUser)
        {
            ServiceResponse<int?> serviceResponse = new ServiceResponse<int?>();
            try
            {
                // get user with the help of user id 
                var user = await dataContext.Users.FirstOrDefaultAsync(x => x.UserId == updateUser.UserId);
                //check if user is null or not
                if (user == null)
                {
                    //if user is null send show user not found with the Id
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "user not found with the Id!";
                    return serviceResponse;
                }
                //if user is not null then  update user by id 
                user.UserMail = updateUser.UserMail;
                user.UserName = updateUser.UserName != null ? updateUser.UserName : user.UserName;
                user.IsActive = updateUser.IsActive;
                //update user and save it into database
                dataContext.Users.Update(user);
                var result = await dataContext.SaveChangesAsync();
                //check if user is successfully added or not 
                
            }
            // cath exception when try throw exception and show exception message
            catch (Exception ex)
            {
                logger.LogError("Error= {@InnerExcpetion}", "InnerException = " + ex.InnerException + Environment.NewLine + "Message = " + ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }
        /// <summary>
        /// Validate user and return its details
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<GetUserDto>> ValidateUser(string Username, string Password)
        {
            var serviceResponse = new ServiceResponse<GetUserDto>();

            try
            {
                // 1️⃣ Find user by username or email
                var user = await _userManager.FindByNameAsync(Username) ?? await _userManager.FindByEmailAsync(Username);

                if (user == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Invalid username or password.";
                    return serviceResponse;
                }

                // 2️⃣ Check password
                var isValidPassword = await _userManager.CheckPasswordAsync(user, Password);
                if (!isValidPassword)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Invalid username or password.";
                    return serviceResponse;
                }

                // 3️⃣ Get user roles
                var roles = await _userManager.GetRolesAsync(user);
                var roleName = roles.FirstOrDefault(); // Take first role if multiple

                // 4️⃣ Prepare DTO
                serviceResponse.Data = new GetUserDto
                {
                    UserId = user.Id,
                    UserMail = user.Email,
                    UserName = user.UserName,
                    IsActive = true, // Identity users are enabled by default; adjust if needed
                    RoleName = roleName
                };

                serviceResponse.Success = true;
                serviceResponse.Message = "User validated successfully.";
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error validating user {Username}", Username);
                serviceResponse.Success = false;
                serviceResponse.Message = "An error occurred while validating user.";
            }

            return serviceResponse;
        }
        
        //Update user role by user id and role id 

        public async Task<string> IssueRefreshToken(int userId)
        {
            var user = await dataContext.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return null;
            // Generate a secure random token
            var randomBytes = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            var refreshToken = new Models.Models.RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddDays(7), // 7 days expiry
                Created = DateTime.UtcNow,
                UserId = userId
            };
            user.RefreshTokens.Add(refreshToken);
            await dataContext.SaveChangesAsync();
            return refreshToken.Token;
        }

        public async Task<GetUserDto> ValidateRefreshToken(string refreshToken)
        {
            var token = await dataContext.RefreshTokens.Include(rt => rt.User).FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.IsActive);
            if (token == null) return null;
            var user = token.User;
            if (user == null) return null;
            // Optionally: revoke the token after use
            token.Revoked = DateTime.UtcNow;
            await dataContext.SaveChangesAsync();
            return new GetUserDto
            {
                Id = user.UserId,
                UserMail = user.UserMail,
                UserName = user.UserName,
                IsActive = user.IsActive ?? false,
                // You may want to fetch the role as well
                RoleName = user.Roles.FirstOrDefault()?.RoleName
            };
        }

        // Helper to invalidate all user cache pages
        private async Task InvalidateUserCache()
        {
            // NOTE: IDistributedCache does not support key enumeration by default.
            // In production, use a Redis client or a cache key pattern strategy.
            // For now, you may need to clear all relevant keys manually or use a prefix strategy.
        }
    }
}
