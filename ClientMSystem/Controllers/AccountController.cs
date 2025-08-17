using ClientMSystem.Data;
using ClientMSystem.Models; // Consider splitting into Entities and ViewModels
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ClientMSystem.Controllers
{
    // ---- ViewModels (keep these separate from your EF entities) ----
    public sealed class LoginVm
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }

    public sealed class SignUpVm
    {
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string ConfirmPassword { get; set; } = "";
    }

    public sealed class AdminLoginVm
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }

    [Authorize]
    public class AccountController : Controller
    {
        private const string AuthScheme = CookieAuthenticationDefaults.AuthenticationScheme;

        private readonly ApplicationContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ApplicationContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? returnUrl = null) =>
            View(new LoginVm { ReturnUrl = returnUrl });

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVm model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(model);

            var username = (model.Username ?? string.Empty).Trim();
            var normalized = username.ToLowerInvariant();

            var user = await _context.signUps
                .AsNoTracking()
                .SingleOrDefaultAsync(e => e.Username.ToLower() == normalized, ct);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password ?? string.Empty, user.Password))
            {
                _logger.LogWarning("Failed login for username: {Username}", username);
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }

            await SignInAsync(
                subjectId: user.ID.ToString(),
                username: user.Username,
                role: "User",
                rememberMe: model.RememberMe);

            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return LocalRedirect(model.ReturnUrl);

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult SignUp() => View(new SignUpVm());

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpVm model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Basic confirm check (add password policy validation server-side too)
            if (!string.Equals(model.Password, model.ConfirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError(nameof(model.ConfirmPassword), "Passwords do not match.");
                return View(model);
            }

            var username = (model.Username ?? string.Empty).Trim();
            var email = (model.Email ?? string.Empty).Trim();

            var normalizedUsername = username.ToLowerInvariant();
            var normalizedEmail = email.ToLowerInvariant();

            var exists = await _context.signUps.AsNoTracking().AnyAsync(
                e => e.Username.ToLower() == normalizedUsername || e.Email.ToLower() == normalizedEmail, ct);

            if (exists)
            {
                ModelState.AddModelError(string.Empty, "Username or email already exists.");
                return View(model);
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // Map to your entity
            var entity = new SignUp
            {
                Username = username,
                Email = email,
                Password = hashedPassword,
                // DO NOT store ConfirmPassword
            };

            await _context.signUps.AddAsync(entity, ct);
            await _context.SaveChangesAsync(ct);

            TempData["SuccessMessage"] = "Registration successful! Please log in.";
            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(AuthScheme);
            // Do NOT delete all cookies; you may remove only your auth/session cookie if needed.
            // Response.Cookies.Delete(".AspNetCore.Cookies"); // optional if you need to force-delete
            return RedirectToAction(nameof(Login), "Account");
        }

        // Remote validator endpoint for username existence
        [AllowAnonymous]
        [AcceptVerbs("GET")]
        public async Task<IActionResult> UsernameExists(string uname, CancellationToken ct)
        {
            var normalized = (uname ?? string.Empty).Trim().ToLowerInvariant();
            bool exists = await _context.signUps.AsNoTracking().AnyAsync(e => e.Username.ToLower() == normalized, ct);
            // jQuery Validate expects "true" for OK (i.e., valid), or a string message for error
            return exists ? Json($"Username '{uname}' is already taken.") : Json(true);
        }

        // ---- Helpers ----
        private async Task SignInAsync(string subjectId, string username, string role, bool rememberMe)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, subjectId),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, AuthScheme);
            var principal = new ClaimsPrincipal(identity);

            var props = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                AllowRefresh = true,
                ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(2)
            };

            await HttpContext.SignInAsync(AuthScheme, principal, props);
        }
    }

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private const string AuthScheme = CookieAuthenticationDefaults.AuthenticationScheme;

        private readonly ApplicationContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApplicationContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AdminLogin(string? returnUrl = null) =>
            View(new AdminLoginVm { ReturnUrl = returnUrl });

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminLogin(AdminLoginVm model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(model);

            var username = (model.Username ?? string.Empty).Trim();
            var normalized = username.ToLowerInvariant();

            var admin = await _context.adminModel
                .AsNoTracking()
                .SingleOrDefaultAsync(a => a.Username.ToLower() == normalized, ct);

            if (admin == null || !BCrypt.Net.BCrypt.Verify(model.Password ?? string.Empty, admin.Password))
            {
                _logger.LogWarning("Failed admin login for username: {Username}", username);
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }

            await SignInAdminAsync(admin.Id.ToString(), admin.Username, model.RememberMe);

            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return LocalRedirect(model.ReturnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(AuthScheme);
            return RedirectToAction(nameof(AdminLogin));
        }

        private async Task SignInAdminAsync(string subjectId, string username, bool rememberMe)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, subjectId),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var identity = new ClaimsIdentity(claims, AuthScheme);
            var principal = new ClaimsPrincipal(identity);

            var props = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                AllowRefresh = true,
                ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(2)
            };

            await HttpContext.SignInAsync(AuthScheme, principal, props);
        }
    }
}
