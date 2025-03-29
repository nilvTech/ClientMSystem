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
            if (userId == null)
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
