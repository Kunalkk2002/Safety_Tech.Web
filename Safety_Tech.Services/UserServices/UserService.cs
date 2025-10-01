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

      
    }
}
