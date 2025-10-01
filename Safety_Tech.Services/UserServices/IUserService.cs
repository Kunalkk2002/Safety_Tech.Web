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
        /// Validate user and return user details
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetUserDto>> ValidateUser(string Username, string Password);
        

    }
}
