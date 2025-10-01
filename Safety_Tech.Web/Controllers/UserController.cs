using Microsoft.AspNetCore.Mvc;
using Safety_Tech.DTOs.UserDTOs;
using Microsoft.AspNetCore.Authorization;
using Safety_Tech.Web.Helper;

namespace Safety_Tech.Web.Controllers
{
    //[ApiController]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        


    }


}
