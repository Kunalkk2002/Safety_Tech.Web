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
using DocumentFormat.OpenXml.Bibliography;
using System.Configuration;

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
        private readonly IConfiguration _configuration;
        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public HomeController(ILogger<HomeController> logger, ICSVFile csvFileService,ApplicationDataContext context, IConfiguration configuration)
        {
            _logger = logger;
            _csvFileService = csvFileService;
            _context = context;
            _configuration = configuration;
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
            string sourceFolderPath = _configuration["FolderPaths:SourceFolder"];
            string processedFolderPath = _configuration["FolderPaths:ProcessedFolder"];
            _csvFileService.ProcessCsvFilesAsync(sourceFolderPath, processedFolderPath);

            var PotentialInsidents = _context.Incidents.Count();

            var Incidents = await _context.Incidents
                .Where(x => !x.IsApprove && x.ApproveBy == "")
                .ToListAsync();

            var ConfirmIncidents = await _context.Incidents
                .Where(x => x.IsApprove && x.ApproveBy != null)
                .ToListAsync();

            var potentialIncidentsCount = _context.Incidents
                .Where(x => !x.IsApprove)
            .Count();


            var currentYear = DateTime.Now.Year;

            var incidentsCurrentYear = _context.Incidents
                .Where(x => x.Timestamp.Year == currentYear)
                .Count();



            var ViolationTypes = _context.Incidents
                                     .Select(i => i.ViolationType)
                                     .Distinct()
                                     .ToList();

            var thisYear = DateTime.Now.Year;
            var today = DateTime.Now;

            // Get incidents grouped by year
            var incidentCounts = await _context.Incidents
                .GroupBy(i => i.Timestamp.Year)
                .Select(g => new
                {
                    Year = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            // Get counts
            int thisYearCount = incidentCounts.FirstOrDefault(x => x.Year == thisYear)?.Count ?? 0;
            int lastYearCount = incidentCounts.FirstOrDefault(x => x.Year == thisYear - 1)?.Count ?? 0;

            // Frequency (days per incident this year)
            double daysThisYear = (today - new DateTime(thisYear, 1, 1)).TotalDays;
            double frequencyThisYear = thisYearCount > 0 ? daysThisYear / thisYearCount : 0;

            // % Change vs last year
            double percentChange = lastYearCount > 0
                ? ((double)thisYearCount - lastYearCount) / lastYearCount * 100
                : 0;

            // Prepare result
            var result = new
            {
                ThisYearIncidents = thisYearCount,
                LastYearIncidents = lastYearCount,
                FrequencyDays = Math.Round(frequencyThisYear, 1), // e.g. 6.8d
                PercentChange = Math.Round(percentChange, 2)     // e.g. -23.00%
            };

            var viewModel = new IncidentViewModel
            {
                Incidents = Incidents,
                PotentialIncidentsCount = potentialIncidentsCount,
                ConfirmIncidentsCount = ConfirmIncidents.Count,
                ViolationTypes = ViolationTypes,
                incidentsCurrentYear = incidentsCurrentYear,
                FrequencyDays = (int)Math.Round(frequencyThisYear, 1)
            };

           
   
            return View(viewModel);
                  
        }


        [HttpGet]
        //[Authorize(Roles =UserSystemRoles.Admin)]
        public async Task<IActionResult> Index1()
        {
            var PotentialInsidents = _context.Incidents.Count();

            var Incidents = await _context.Incidents
                 .Include(ai => ai.Approver)
                .ToListAsync();

            var potentialIncidentsCount = _context.Incidents
                .Count();

            var viewModel = new IncidentViewModel
            {
                Incidents = Incidents,
                PotentialIncidentsCount = potentialIncidentsCount,
                ConfirmIncidentsCount = Incidents.Count
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


    }
}