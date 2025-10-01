using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Safety_Tech.DTOs.incidents;
using Safety_Tech.Models.Models;

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

        public IActionResult Details()
        {
            // Fetch all records from Objectdetection table
            var records = _context.Incidents
                 .Where(o => !o.IsApprove )
                                  .OrderByDescending(o => o.CreatedAt) 
                                  .ToList();

            // Map Objectdetection->Incidents DTO
                var incidents = records.Select(r => new DTOs.incidents.Incidents
                {
                    Id = r.Id,
                    CreatedAt = r.CreatedAt,
                    Image = r.Image ?? "Unknown", // adjust if you add Location
                    Label = r.Label,
                    Confidence = r.Confidence
                    
                }).ToList();

            return View(incidents);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Approve(int incidentId)
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

            return RedirectToAction(nameof(Details), new { id = incidentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Reject(int incidentId)
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

            return RedirectToAction(nameof(Details), new { id = incidentId });
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
