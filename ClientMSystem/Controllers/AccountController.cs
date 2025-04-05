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

            try
            {
                var user = await _context.signUps.FirstOrDefaultAsync(e => e.Username == model.Username);
                if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    TempData["ErrorMessage"] = "Invalid username or password.";
                    return View(model);
                }

                var claims = new[] { new Claim(ClaimTypes.Name, user.Username), new Claim("UserId", user.ID.ToString()) };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                HttpContext.Session.SetInt32("UserId", user.ID);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error");
                TempData["ErrorMessage"] = "An error occurred during login.";
                return View(model);
            }
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            foreach (var cookie in Request.Cookies.Keys)
                Response.Cookies.Delete(cookie);

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
            {
                TempData["ErrorMessage"] = "Please fill in all required fields.";
                return View(model);
            }

            try
            {
                if (await _context.signUps.AnyAsync(e => e.Username == model.Username || e.Email == model.Email))
                {
                    TempData["ErrorMessage"] = "Username or email already exists.";
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Signup error");
                TempData["ErrorMessage"] = "An error occurred during registration.";
                return View(model);
            }
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

            try
            {
                var admin = await _context.adminModel.FirstOrDefaultAsync(a => a.Username == model.Username && a.Password == model.Password);
                if (admin == null)
                {
                    ViewBag.msg = "<div class='alert alert-danger'>Invalid Email Or Password!</div>";
                    return View(model);
                }

                var claims = new[] { new Claim(ClaimTypes.Name, model.Username) };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                HttpContext.Session.SetInt32("UserId", admin.Id);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin login error");
                ViewBag.msg = "<div class='alert alert-danger'>An error occurred.</div>";
                return View(model);
            }
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            foreach (var cookie in Request.Cookies.Keys)
                Response.Cookies.Delete(cookie);

            return RedirectToAction("Login", "Account");
        }
    }
}
