using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Safety_Tech.DTOs;
using Safety_Tech.DTOs.RoleDTOs;
using Safety_Tech.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Safety_Tech.Services.RoleServices
{
    /// <summary>
    /// Service implementation for role-related business logic.
    /// </summary>
    public class RoleService : IRoleService
    {
        private readonly ILogger<RoleService> logger; // Logger for logging information and errors
        private readonly IMapper mapper; // Mapper for object mapping
        private readonly ApplicationDataContext dataContext; // Database context for data access
        private readonly IDistributedCache distributedCache; // Distributed cache for caching roles

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleService"/> class.
        /// </summary>
        /// <param name="logger">Logger instance for logging.</param>
        /// <param name="mapper">Mapper instance for object mapping.</param>
        /// <param name="dataContext">Database context for data access.</param>
        public RoleService(ILogger<RoleService> logger, IMapper mapper, ApplicationDataContext dataContext, IDistributedCache distributedCache)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.dataContext = dataContext;
            this.distributedCache = distributedCache;
        }

        /// <summary>
        /// Creates a new role in the system.
        /// </summary>
        /// <param name="addRole">The role data to add.</param>
        /// <returns>Service response with the added role DTO.</returns>
        public async Task<ServiceResponse<AddRoleDto>> CreateRole(AddRoleDto addRole)
        {
            ServiceResponse<AddRoleDto> serviceResponse = new ServiceResponse<AddRoleDto>();
            try
            {
                // Map DTO to entity and add to database
                var role = mapper.Map<Role>(addRole);
                await dataContext.AddAsync(role);
                var result = await dataContext.SaveChangesAsync();
                addRole.RoleId = role.RoleId;

                // Check if the role was added successfully
                if (result == 1)
                {
                    serviceResponse.Data = addRole;
                    serviceResponse.Success = true;
                    serviceResponse.Message = "Role added successfully!";
                }
                else
                {
                    serviceResponse.Data = addRole;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Something went wrong!";
                }
            }
            catch (Exception ex)
            {
                // Log error and return failure response
                logger.LogError("Error= {@InnerExcpetion}", "InnerException = " + ex.InnerException + Environment.NewLine + "Message = " + ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        /// <summary>
        /// Retrieves all roles from the system.
        /// </summary>
        /// <returns>Service response with a list of role DTOs.</returns>
        public async Task<ServiceResponse<List<AddRoleDto>>> GetAllRole()
        {
            ServiceResponse<List<AddRoleDto>> serviceResponse = new ServiceResponse<List<AddRoleDto>>();
            // Try to get roles from Redis cache
            var cachedRoles = await distributedCache.GetStringAsync("AllRoles");
            List<AddRoleDto> allRole = null;
            if (!string.IsNullOrEmpty(cachedRoles))
            {
                allRole = JsonSerializer.Deserialize<List<AddRoleDto>>(cachedRoles);
            }
            else
            {
                // Get role details through mapping model to AddRoleDto, use AsNoTracking for performance
                allRole = await dataContext.Roles.AsNoTracking().Select(x => mapper.Map<AddRoleDto>(x)).ToListAsync();
                // Cache the roles for 5 minutes
                var cacheOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) };
                await distributedCache.SetStringAsync("AllRoles", JsonSerializer.Serialize(allRole), cacheOptions);
            }
            // Check if any roles were found
            if (allRole?.Count > 0)
            {
                serviceResponse.Data = allRole;
                serviceResponse.Success = true;
                serviceResponse.Message = serviceResponse.Data.Count.ToString() + " Records found";
                return serviceResponse;
            }
            // If no roles found, return not found message
            serviceResponse.Data = null;
            serviceResponse.Success = false;
            serviceResponse.Message = "Records not found";
            return serviceResponse;
        }
    }
}
