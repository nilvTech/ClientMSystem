using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientMSystem.Data;
using ClientMSystem.Models;
using Microsoft.EntityFrameworkCore;
using IronPdf;
using Microsoft.AspNetCore.Http;

namespace ClientMSystem.Controllers
{
    public class TaskSheetController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<TaskSheetController> _logger;

        public TaskSheetController(ApplicationContext context, ILogger<TaskSheetController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue)
            {
                _logger.LogWarning("User ID not found in session.");
                TempData["Error"] = "Session expired. Please log in again.";
                return RedirectToAction("Login", "Account"); // Adjust as per your auth setup
            }

            var result = await _context.TimeSheets
                .Where(t => t.UserId == userId.Value)
                .AsNoTracking()
                .ToListAsync();

            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TimeSheet model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (!ModelState.IsValid || !userId.HasValue)
            {
                TempData["Error"] = "Enter all required details.";
                return View(model);
            }

            model.UserId = userId.Value;
            model.CreatedDate = DateTime.UtcNow;

            _context.TimeSheets.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Task sheet saved successfully!";
            _logger.LogInformation("TaskSheet created by UserId {UserId}", userId);

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Exports the current user's timesheets to PDF using IronPDF.
        /// </summary>
        public async Task<IActionResult> ExportToPdf()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var sheets = await _context.TimeSheets
                .Where(t => t.UserId == userId.Value)
                .ToListAsync();

            if (!sheets.Any())
            {
                TempData["Error"] = "No timesheet data to export.";
                return RedirectToAction(nameof(Index));
            }

            var htmlContent = await this.RenderViewAsync("ExportPdfTemplate", sheets, true); // Helper method required
            var renderer = new HtmlToPdf();
            var pdf = renderer.RenderHtmlAsPdf(htmlContent);

            _logger.LogInformation("Timesheet PDF exported for UserId {UserId}", userId);

            return File(pdf.BinaryData, "application/pdf", "Timesheet_Report.pdf");
        }
    }
}
