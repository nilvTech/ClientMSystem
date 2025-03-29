using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientMSystem.Data;
using ClientMSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using IronPdf;

namespace ClientMSystem.Controllers
{
    public class TaskSheetController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<TaskSheetController> _logger;

        public TaskSheetController(ApplicationContext context, ILogger<TaskSheetController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue)
            {
                return View();
            }

            var result = await _context.TimeSheets.AsNoTracking().ToListAsync();
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
                TempData["Error"] = "Enter all details.";
                return View(model);
            }

            try
            {
                model.UserId = userId.Value;
                _context.TimeSheets.Add(model);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Sheet created successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the timesheet.");
                TempData["Error"] = "An unexpected error occurred. Please try again.";
                return View(model);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var rec = await _context.TimeSheets.FindAsync(id);

            if (rec != null)
            {
                _context.TimeSheets.Remove(rec);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Record deleted successfully";
            }
            else
            {
                TempData["Error"] = "Record not found";
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.TimeSheets.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TimeSheet model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please provide valid data";
                return View(model);
            }

            var existingRec = await _context.TimeSheets.FindAsync(model.Id);
            if (existingRec != null)
            {
                _context.Entry(existingRec).CurrentValues.SetValues(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Sheet updated successfully";
            }
            else
            {
                TempData["Error"] = "Record not found";
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> GeneratePdf(int id)
        {
            try
            {
                var report = await _context.TimeSheets.FindAsync(id);
                if (report == null)
                {
                    return NotFound();
                }

                var renderer = new HtmlToPdf();
                var pdf = renderer.RenderHtmlAsPdf($"<h1>{report.Module}</h1><p>{report.CommentsForAnyDealy}</p>");
                var pdfBytes = pdf.BinaryData;

                return File(pdfBytes, "application/pdf", $"TaskSheet_{id}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF.");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
