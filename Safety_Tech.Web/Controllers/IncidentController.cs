using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Safety_Tech.DTOs.incidents;
using Safety_Tech.Models.Models;
using Safety_Tech.Models.ViewModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Safety_Tech.Web.Controllers
{
    public class IncidentController : Controller
    {
        private readonly ApplicationDataContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IncidentController(ApplicationDataContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> DetailsAsync()
        {
            
            var Incidents = await _context.Incidents
                  .ToListAsync();

            var potentialIncidentsCount = _context.Incidents
                .Count();

            var viewModel = new IncidentViewModel
            {
                Incidents = Incidents,
              
            };

       
            return View(viewModel);
        }

        public IActionResult Incidents_Read([DataSourceRequest] DataSourceRequest request)
        {
            var data = _context.Incidents.ToList(); // or filtered query
            var result = data.ToDataSourceResult(request);
            return Json(result);
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Approve(Guid incidentId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var incident = await _context.Incidents.FindAsync(incidentId);
            if (incident == null)
            {
                return NotFound();
            }

            incident.ApproveBy = user.Id;
            incident.IsApprove = true;

            _context.Incidents.Update(incident); // optional, since tracked entity
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), "Home");
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Reject(Guid incidentId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var incident = await _context.Incidents.FindAsync(incidentId);
            if (incident == null)
            {
                return NotFound();
            }

            incident.ApproveBy = user.Id;
            incident.IsApprove = false;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), "Home");
        }

        [HttpGet]
        public IActionResult GetImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                return NotFound();
            }

            try
            {
                // Assuming images are stored in wwwroot/images or similar folder
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", imagePath);
                
                if (!System.IO.File.Exists(fullPath))
                {
                    return NotFound();
                }

                var imageBytes = System.IO.File.ReadAllBytes(fullPath);
                var contentType = GetContentType(imagePath);
                
                return File(imageBytes, contentType);
            }
            catch
            {
                return NotFound();
            }
        }

        private string GetContentType(string path)
        {
            var extension = Path.GetExtension(path).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
    }
}
