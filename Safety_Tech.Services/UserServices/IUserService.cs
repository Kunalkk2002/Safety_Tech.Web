using Safety_Tech.DTOs;
using Safety_Tech.DTOs.UserDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Safety_Tech.Services.UserServices
{
    public interface IUserService
    {
        /// <summary>
        /// Get All Users (paginated)
        /// </summary>
        /// <param name="pageNumber">Page number (default 1)</param>
        /// <param name="pageSize">Page size (default 20)</param>
        /// <returns>List of User</returns>
        Task<ServiceResponse<List<GetUserDto>>> GetAllUsers(int pageNumber = 1, int pageSize = 20);

        /// <summary>
        /// Get user By ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>User</returns>
        Task<ServiceResponse<GetUserDto>> GetUserById(int id);

       
      

        /// <summary>
        /// Add User
        /// </summary>
        /// <param name="newUser"></param>
        /// <returns>new User</returns>
        Task<ServiceResponse<AddUserDto>> AddUser(AddUserDto newUser);

        /// <summary>
        /// Update Existing User
        /// </summary>
        /// <param name="updateUser"></param>
        /// <returns>Updated User</returns>
        Task<ServiceResponse<int?>> UpdateUser(UpdateUserDto updateUser);

        /// <summary>
        /// Delete user By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>delete user</returns>
        Task<ServiceResponse<Boolean>> DeleteUser(int id);
        /// <summary>
        /// Validate user and return user details
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetUserDto>> ValidateUser(string Username, string Password);
        
        /// <summary>
        /// Issues a new refresh token for the specified user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>The new refresh token string.</returns>
        Task<string> IssueRefreshToken(int userId);

        /// <summary>
        /// Validates a refresh token and returns the associated user if valid.
        /// </summary>
        /// <param name="refreshToken">The refresh token string.</param>
        /// <returns>The user DTO if valid, otherwise null.</returns>
        Task<GetUserDto> ValidateRefreshToken(string refreshToken);
    }
}
