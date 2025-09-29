using AutoMapper;
using Safety_Tech.DTOs.RoleDTOs;
using Safety_Tech.Models.Models;

namespace Safety_Tech.Web.Profiles
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Role, AddRoleDto>();
            CreateMap<AddRoleDto, Role>();
        }
    }
} 