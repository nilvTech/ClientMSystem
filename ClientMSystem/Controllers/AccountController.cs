using ClientMSystem.Data;
using ClientMSystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ClientMSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ApplicationContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(SignUp model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.signUps
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Username == model.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(model);
            }

            await SignInUser(user.Username, user.ID);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> LogOut()
        {
            await SignOutUser();
            return RedirectToAction("Login", "Account");
        }

        [AcceptVerbs("Post", "Get")]
        public async Task<IActionResult> UserNameIsExists(string Uname)
        {
            bool exists = await _context.signUps.AnyAsync(e => e.Username == Uname);
            return exists ? Json($"Username '{Uname}' already exists") : Json(true);
        }

        public IActionResult SignUp() => View();

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUp model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (await _context.signUps.AnyAsync(e => e.Username == model.Username || e.Email == model.Email))
            {
                ModelState.AddModelError("", "Username or email already exists.");
                return View(model);
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
            model.Password = hashedPassword;
            model.ConformPassword = hashedPassword;

            _context.signUps.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Registration successful! Please log in.";
            return RedirectToAction("Login");
        }

        private async Task SignInUser(string username, int userId)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim("UserId", userId.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            HttpContext.Session.SetInt32("UserId", userId);
        }

        private async Task SignOutUser()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            foreach (var cookie in Request.Cookies.Keys)
                Response.Cookies.Delete(cookie);
        }
    }

    public class AdminController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApplicationContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult AdminLogin() => View();

        [HttpPost]
        public async Task<IActionResult> AdminLogin(AdminModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var admin = await _context.adminModel
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Username == model.Username);

            if (admin == null || !BCrypt.Net.BCrypt.Verify(model.Password, admin.Password))
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(model);
            }

            await SignInAdmin(admin.Username, admin.Id);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> LogOut()
        {
            await SignOutAdmin();
            return RedirectToAction("AdminLogin", "Admin");
        }

        private async Task SignInAdmin(string username, int adminId)
        {
            var claims = new[] { new Claim(ClaimTypes.Name, username) };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            HttpContext.Session.SetInt32("UserId", adminId);
        }

        private async Task SignOutAdmin()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            foreach (var cookie in Request.Cookies.Keys)
                Response.Cookies.Delete(cookie);
        }
    }
}
