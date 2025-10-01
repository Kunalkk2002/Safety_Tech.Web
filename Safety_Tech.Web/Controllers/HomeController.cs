using Microsoft.AspNetCore.Mvc;
using Safety_Tech.Web.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Safety_Tech.Web.Helper;
using Safety_Tech.Models.Models;
using Safety_Tech.Services.CSVFile;
using Microsoft.EntityFrameworkCore;
using Safety_Tech.DTOs.incidents;
using Safety_Tech.Models.ViewModels;

namespace Safety_Tech.Web.Controllers
{
    /// <summary>
    /// Controller for handling home page and general site actions.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICSVFile _csvFileService;
        private readonly ApplicationDataContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public HomeController(ILogger<HomeController> logger, ICSVFile csvFileService,ApplicationDataContext context)
        {
            _logger = logger;
            _csvFileService = csvFileService;
            _context = context;
        }


        /// <summary>
        /// Displays the main user index page. Only accessible by users with the 'User' role.
        /// </summary>
        /// <returns>The Index view.</returns>
        /// 
        [HttpGet]
        //[Authorize(Roles =UserSystemRoles.Admin)]
        public async Task<IActionResult> Index()
        {
            var PotentialInsidents = _context.Objectdetections
              .Where(o => !_context.ApprovedIncidents
                      .Any(a => a.IncidentId == o.Id))
                               .Count();

            var confirmIncidents = await _context.ApprovedIncidents
                 .Include(ai => ai.Approver)
                .ToListAsync();
           
            var potentialIncidentsCount = _context.Objectdetections
                .Where(o => !_context.ApprovedIncidents
                    .Any(a => a.IncidentId == o.Id))
                .Count();

            var viewModel = new IncidentViewModel
            {
                ApprovedIncidents = confirmIncidents,
                PotentialIncidentsCount = potentialIncidentsCount,
                ConfirmIncidentsCount = confirmIncidents.Count
            };

            string sourceFolderPath = "D:\\DIBS Project\\csvFiles";
            string processedFolderPath = "D:\\DIBS Project\\csvFiles\\ProcessFolder";
            _csvFileService.ProcessCsvFilesAsync(sourceFolderPath, processedFolderPath);
   
            return View(viewModel);
         
           
        }


        [HttpGet]
        //[Authorize(Roles =UserSystemRoles.Admin)]
        public async Task<IActionResult> Index1()
        {
            var PotentialInsidents = _context.Objectdetections
              .Where(o => !_context.ApprovedIncidents
                      .Any(a => a.IncidentId == o.Id))
                               .Count();
            var confirmIncidents = await _context.ApprovedIncidents
                 .Include(ai => ai.Approver)
                .ToListAsync();

            var potentialIncidentsCount = _context.Objectdetections
                .Where(o => !_context.ApprovedIncidents
                    .Any(a => a.IncidentId == o.Id))
                .Count();

            var viewModel = new IncidentViewModel
            {
                ApprovedIncidents = confirmIncidents,
                PotentialIncidentsCount = potentialIncidentsCount,
                ConfirmIncidentsCount = confirmIncidents.Count
            };

            string sourceFolderPath = "D:\\DIBS Project\\csvFiles";
            string processedFolderPath = "D:\\DIBS Project\\csvFiles\\ProcessFolder";
            _csvFileService.ProcessCsvFilesAsync(sourceFolderPath, processedFolderPath);

            return View(viewModel);


        }

        /// <summary>
        /// Displays the privacy page. Only accessible by users with the 'User' role.
        /// </summary>
        /// <returns>The Privacy view.</returns>
        [Authorize(Roles = UserSystemRoles.User)]
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Displays the error page with request ID for diagnostics.
        /// </summary>
        /// <returns>The Error view with error details.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Displays the HTML controls demo form.
        /// </summary>
        [HttpGet]
        public IActionResult HtmlControlsDemo()
        {
            return View(new HtmlControlsDemo());
        }

        /// <summary>
        /// Processes the HTML controls demo form submission.
        /// </summary>
        [HttpPost]
        public IActionResult HtmlControlsDemo(HtmlControlsDemo model)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Message = "Form submitted successfully!";
                return View(model);
            }
            return View(model);
        }


    }
}