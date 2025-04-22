using ClientMSystem.Data;
using ClientMSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ClientMSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IActionResult Index()
        {
            _logger.LogInformation("User {User} accessed the Home Index at {Time}", 
                User.Identity?.Name ?? "Unknown", DateTime.UtcNow);

            // Optionally, check for admin and customize the view
            // if (User.IsInRole("Admin")) { return View("AdminDashboard"); }

            return View();
        }

        public IActionResult Privacy()
        {
            _logger.LogInformation("User {User} accessed the Privacy page.", 
                User.Identity?.Name ?? "Unknown");

            return View();
        }

        /// <summary>
        /// Returns a diagnostic summary — can be styled for internal users.
        /// </summary>
        [Authorize(Roles = "Admin")]
        public IActionResult SystemInfo()
        {
            var info = new
            {
                MachineName = Environment.MachineName,
                OS = Environment.OSVersion.ToString(),
                Runtime = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
                CurrentTime = DateTime.Now,
                AppUptime = (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss"),
                LoggedUser = User.Identity?.Name
            };

            _logger.LogInformation("SystemInfo requested by {User}", User.Identity?.Name);

            return Json(info);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            _logger.LogError("Error occurred for user {User}. Request ID: {RequestId}", 
                User.Identity?.Name ?? "Unknown", requestId);

            return View(new ErrorViewModel { RequestId = requestId });
        }
    }
}
