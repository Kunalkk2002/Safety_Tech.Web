using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Safety_Tech.DTOs.UserDTOs;
using Safety_Tech.Services.RoleServices;
using Safety_Tech.Services.UserServices;
using Safety_Tech.Web.Helper;
using Safety_Tech.Models.Models;

namespace Safety_Tech.Web.Controllers
{
    //[ApiController]
    public class AuthController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userServices;
        private readonly IRoleService _roleService;
        private readonly IConfiguration _config;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthController(IUserService userServices, IRoleService roleService, ILogger<UserController> logger, IConfiguration config, UserManager<IdentityUser> userManager)
        {
            _userServices = userServices;
            _roleService = roleService;
            _logger = logger;
            _config = config;
            _userManager = userManager;
        }

        // Package For Authentication:
        // Google Authentication     =>  Microsoft.AspNetCore.Authentication.Google
        // Facebook Authentication   =>  Microsoft.AspNetCore.Authentication.Facebook
        // Microsoft Authentication  =>  Microsoft.AspNetCore.Authentication.MicrosoftAccount => web
        // Office 365 Authentication =>  Microsoft.AspNetCore.Authentication.OpenIdConnect

        public IActionResult SuperAdminDashboard()
        {
            return View();
        }
        public IActionResult AdminDashboard()
        {
            return View();
        }
        public IActionResult UserDashboard()
        {
            return View();
        }
        /// <summary>
        /// Handles the login action for authentication with Google.
        /// </summary>
        /// <returns>Returns Google Response.</returns>
        public async Task GoogleLogin()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action(nameof(GoogleResponse)) // Pass the name of the GoogleResponse method
                });
        }

        /// <summary>
        /// Handles the response after successful authentication with Google.
        /// </summary>
        /// <returns>Returns a redirection to the home page.</returns>
        public async Task<IActionResult> GoogleResponse()
            {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            await CheckIfUserExist(result);
            // Optionally, process the user claims

            return RedirectToAction(ViewConstants.INDEX, ViewConstants.HOME, new { area = "" });
            }

        /// <summary>
        /// Displays the login view.
        /// </summary>
        /// <returns>Returns the login view.</returns>
        /// 
        [HttpGet]
        public IActionResult loginView()
        {
            var loginDTo = new LoginDto();
            _logger.LogInformation("loginView page accessed.");
            return View(loginDTo);
        }

        /// <summary>
        /// Redirects to Facebook for authentication.
        /// </summary>
        /// <returns>Returns a challenge to authenticate with Facebook.</returns>
        public IActionResult FacebookLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(FacebookResponse))
            };

            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Handles the response after successful authentication with Facebook.
        /// </summary>
        /// <returns>Returns a redirection to the home page.</returns>
        public async Task<IActionResult> FacebookResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await CheckIfUserExist(result);


            // Optionally, process the user claims

            return RedirectToAction(ViewConstants.INDEX, ViewConstants.HOME);
        }

        /// <summary>
        /// Redirects to Microsoft for authentication.
        /// </summary>
        /// <returns>Returns a challenge to authenticate with Microsoft.</returns>
        public IActionResult MicrosoftLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(MicrosoftResponse))
            };

            return Challenge(properties, MicrosoftAccountDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Handles the response after successful authentication with Microsoft.
        /// </summary>
        /// <returns>Returns a redirection to the home page.</returns>
        public async Task<IActionResult> MicrosoftResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await CheckIfUserExist(result);


            // Optionally, process the user claims

            return RedirectToAction(ViewConstants.INDEX, ViewConstants.HOME);
        }

        /// <summary>
        /// Redirects to Microsoft 365 for authentication.
        /// </summary>
        /// <returns>Returns a challenge to authenticate with Microsoft 365.</returns>
        public IActionResult Microsoft365Login()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(Microsoft365Response))
            };

            return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Handles the response after successful authentication with Microsoft 365.
        /// </summary>
        /// <returns>Returns a redirection to the home page.</returns>
        public async Task<IActionResult> Microsoft365Response()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await CheckIfUserExist(result);


            // Optionally, process the user claims

            return RedirectToAction(ViewConstants.INDEX, ViewConstants.HOME);
        }

        /// <summary>
        /// Logs out the current user.
        /// </summary>
        /// <returns>Returns the login view.</returns>
        //public async Task<IActionResult> Logout()
        //{
        //    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        //    // Optionally, clear other user-related data or perform additional logout actions

        //    return RedirectToAction(ViewConstants.LOGINVIEW, ViewConstants.AUTH);
        //}


        [HttpPost]
        public IActionResult Logout()
        {
            // Optional: clear any server-side cookies if used (mostly for hybrid setups)
            Response.Cookies.Delete("YourCookieName");

            // Log and redirect
            _logger.LogInformation("User logged out.");
            return Ok(new { redirectUrl = Url.Action("loginView", "Auth") });
        }

        /// <summary>
        /// Method is used to check the user if it exists in database and add role from database to the claims.
        /// </summary>
        /// <param name="result">Authentication Result</param>
        /// <returns>It returns true if authentication is successful or user exists else false.</returns>
        private async Task<bool> CheckIfUserExist(AuthenticateResult result)
        {
            if (result.Succeeded)
                {
                // get the identity details from the authenticated result
                var claimsIdentity = (ClaimsIdentity)result.Principal.Identity;
                // add user role from database to the claim
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, UserRoles.User.ToString()));
                //var accessToken = result.Properties.GetTokenValue("access_token");
                var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
                    {
                    claim.Issuer,
                    claim.OriginalIssuer,
                    claim.Type,
                    claim.Value,

                    });
                await HttpContext.SignInAsync(result.Principal);
                return true;
                }
            return false;
        }
        /// <summary>
        /// Validate user and generate jwt token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var userDto = await _userServices.ValidateRefreshToken(refreshToken);
            if (userDto == null)
                return Unauthorized();
            var newJwt = GenerateJWTToken(userDto);
            var newRefreshToken = await _userServices.IssueRefreshToken(userDto.Id);
            return Ok(new { access_token = newJwt, refresh_token = newRefreshToken });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
         private string GenerateJWTToken(GetUserDto user)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.RoleName),
                    new Claim(ClaimTypes.Email, user.UserMail)
                };

                // Get JWT configuration values from appsettings.json via injected IConfiguration
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30), // Set expiry to 30 minutes
                    signingCredentials: creds);

                // TODO: Implement refresh token logic here
                return new JwtSecurityTokenHandler().WriteToken(token); // Return JWT string

            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginDto model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //var admin = await _userManager.FindByEmailAsync(model.Username);

            // Validate user credentials
            var response = await _userServices.ValidateUser(model.Username, model.Password);

            if (response == null || !response.Success || response.Data == null)
                return Unauthorized();

            // Resolve IdentityUser and sign-in with Identity user id so [Authorize] + FKs work
            IdentityUser? identityUser = null;
            if (!string.IsNullOrWhiteSpace(response.Data.UserMail))
            {
                identityUser = await _userManager.FindByEmailAsync(response.Data.UserMail);
            }
            if (identityUser == null && !string.IsNullOrWhiteSpace(response.Data.UserName))
            {
                identityUser = await _userManager.FindByNameAsync(response.Data.UserName);
            }
            // fallback: try username from login form
            if (identityUser == null && !string.IsNullOrWhiteSpace(model.Username))
            {
                identityUser = await _userManager.FindByNameAsync(model.Username);
                if (identityUser == null)
                {
                    identityUser = await _userManager.FindByEmailAsync(model.Username);
                }
            }

            var nameIdentifier = identityUser?.Id ?? Guid.NewGuid().ToString("N");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, nameIdentifier),
                new Claim(ClaimTypes.Name, response.Data.UserName ?? model.Username),
                new Claim(ClaimTypes.Email, response.Data.UserMail ?? string.Empty),
                new Claim(ClaimTypes.Role, response.Data.RoleName ?? string.Empty)
            };

            var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };
            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            // Determine redirect URL based on role
            string redirectUrl;

            switch (response.Data.RoleName)
            {
                case UserSystemRoles.SuperAdmin:
                    redirectUrl = Url.Action("SuperAdminDashboard", "Auth")!;
                    break;
                case UserSystemRoles.Admin:
                    redirectUrl = Url.Action("Index", "Home")!;  // redirect admin to Home Dashboard
                    break;
                case UserSystemRoles.User:
                    redirectUrl = Url.Action("UserDashboard", "Auth")!;
                    break;
                default:
                    redirectUrl = Url.Action("Index", "Home")!;
                    break;
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect(redirectUrl);
        }
    }
}




