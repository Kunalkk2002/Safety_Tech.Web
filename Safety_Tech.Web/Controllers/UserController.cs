using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Safety_Tech.DTOs.UserDTOs;
using Safety_Tech.Services.RoleServices;
using Safety_Tech.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Safety_Tech.Web.Helper;

namespace Safety_Tech.Web.Controllers
{
    //[ApiController]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userServices;
        private readonly IRoleService _roleService;

        public UserController(IUserService userServices, IRoleService roleService, ILogger<UserController> logger)
        {
            _userServices = userServices;
            _roleService = roleService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all users and renders the view with the user data.
        /// </summary>
        /// <returns>Returns the view with the user data.</returns>
        [HttpGet]
        [Authorize (Roles =UserSystemRoles.SuperAdmin)]
        public async Task<IActionResult> GetAllUser()
        {
            var serviceResponse = await _userServices.GetAllUsers();
            if (serviceResponse?.Success == true)
            {
                return View(ViewConstants.GETALLUSER, serviceResponse.Data); // Pass the data to the Index view
            }
            return View("Error");
        }
        [HttpGet]
        [Authorize (Roles =UserSystemRoles.AdminOrSuperAdmin)]
        public async Task<IActionResult> GetUserById(int id)
        {
            var serviceResponse = await _userServices.GetUserById(id);
            if (serviceResponse?.Success == true)
            {
                return Ok(serviceResponse.Data);
            }
            return NotFound(new { message = "User not found" });
        }

        /// <summary>
        /// Adds a new user.
        /// </summary>
        /// <param name="addUser">The user data to be added.</param>
        /// <returns>Returns the view with the added user data or error message.</returns>
        [HttpPost]
        public async Task<IActionResult> AddUser(AddUserDto addUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var serviceResponse = await _userServices.AddUser(addUser);
            if (serviceResponse?.Success == true)
            {
                return RedirectToAction(ViewConstants.GETALLUSER, new { id = serviceResponse.Data.UserId });
            }
            return View();
        }


        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="id">The ID of the user to be deleted.</param>
        /// <returns>Returns a redirection to the user list.</returns>
        /// 
        [HttpDelete]
        [Authorize (Roles =UserSystemRoles.AdminOrSuperAdmin)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var serviceResponse = await _userServices.DeleteUser(id);

            if (serviceResponse.Success)
            {
                TempData["SuccessMessage"] = serviceResponse.Message;
            }
            else
            {
                TempData["ErrorMessage"] = serviceResponse.Message;
            }

            return RedirectToAction(ViewConstants.GETALLUSER);
        }

        /// <summary>
        /// Retrieves user details for updating.
        /// </summary>
        /// <param name="id">The ID of the user to be updated.</param>
        /// <returns>Returns the view with the user data to be updated.</returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> UpdateUser(int id)
        {
            var serviceResponse = await _userServices.GetUserById(id);
            if (serviceResponse?.Success == true)
            {
                return View(serviceResponse.Data); // Pass the GetUserDto model to the view
            }
            return RedirectToAction(ViewConstants.GETALLUSER); // Handle if user not found
        }

        /// <summary>
        /// Updates user details.
        /// </summary>
        /// <param name="updateUserDto">The updated user data.</param>
        /// <returns>Returns a redirection to the user list or validation errors.</returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateUser(UpdateUserDto updateUserDto)
        {
            if (ModelState.IsValid)
            {
                var serviceResponse = await _userServices.UpdateUser(updateUserDto);
                if (serviceResponse?.Success == true)
                {
                    return RedirectToAction(ViewConstants.GETALLUSER); // Redirect to user list after successful update
                }
                ModelState.AddModelError("", serviceResponse?.Message ?? "Update failed");
            }
            return View(updateUserDto); // Show the update form with validation errors
        }

        /// <summary>
        /// Retrieves user details for editing.
        /// </summary>
        /// <param name="userId">The ID of the user to be edited.</param>
        /// <returns>Returns the view with the user data to be edited.</returns>
        public async Task<IActionResult> EditUser(int userId)
        {
            var serviceResponse = await _userServices.GetUserById(userId);

            if (serviceResponse?.Success == true)
            {
                var userData = serviceResponse.Data;

                // Populate the view model and pass it to the view
                var updateUserDto = new UpdateUserDto
                {
                    //UserId = userData.UserId,
                    UserMail = userData.UserMail,
                    UserName = userData.UserName,
                    IsActive = userData.IsActive
                };

                return View(updateUserDto);
            }
            else
            {
                // Handle error case, e.g., redirect to an error page
                return RedirectToAction(ViewConstants.INDEX);
            }
        }

        


    }


}
