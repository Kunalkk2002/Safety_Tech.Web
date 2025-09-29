using Safety_Tech.DTOs.RoleDTOs;
using Safety_Tech.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Safety_Tech.Services.RoleServices
{
    public interface IRoleService
    {
        /// <summary>
        /// Add New Role
        /// </summary>
        /// <param name="addRole"></param>
        /// <returns>new Role</returns>
        Task<ServiceResponse<AddRoleDto>> CreateRole(AddRoleDto addRole);

        /// <summary>
        /// Get All Role (uses caching and AsNoTracking for performance)
        /// </summary>
        /// <returns>List Of Role</returns>
        Task<ServiceResponse<List<AddRoleDto>>> GetAllRole();
    }
}
