using ClientMSystem.Controllers;
using ClientMSystem.Data;
using ClientMSystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ClientMSystem.Controllers
{
   
    public class AccountController : Controller
    {
        private readonly ApplicationContext context;


        public AccountController(ApplicationContext context)
        {
            this.context = context;
        }

      
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
    public IActionResult Login(SignUp model)
    {
        try
        {
            var user = context.signUps.FirstOrDefault(e => e.Username == model.Username);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Username not found.";
                return View(model);
            }

            bool isValidPassword = BCrypt.Net.BCrypt.Verify(model.Password, user.Password);
            if (!isValidPassword)
            {
                TempData["ErrorMessage"] = "Invalid email or password!";
                return View(model);
            }

            // Create Authentication Cookie
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("UserId", user.ID.ToString())
            }, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Store user ID in session
            HttpContext.Session.SetInt32("UserId", user.ID);

            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "An error occurred during login.";
            return View(model);
        }
    }

    // ********************************* LOGOUT *********************************
    public IActionResult LogOut()
    {
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // Delete all cookies after logout
        foreach (var cookie in Request.Cookies.Keys)
        {
            Response.Cookies.Delete(cookie);
        }

        return RedirectToAction("Login", "Account");
    }

    // ********************************* CHECK IF USERNAME EXISTS *********************************
    [AcceptVerbs("Post", "Get")]
    public IActionResult UserNameIsExists(string Uname)
    {
        bool exists = context.signUps.Any(e => e.Username == Uname);
        return exists ? Json($"Username '{Uname}' already exists") : Json(true);
    }

    // ********************************* SIGNUP *********************************
    public IActionResult SignUp()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SignUp(SignUp model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Please fill in all required fields.";
            return View(model);
        }

        try
        {
            // Check if username or email already exists
            if (context.signUps.Any(e => e.Username == model.Username || e.Email == model.Email))
            {
                TempData["ErrorMessage"] = "Username or email already exists.";
                return View(model);
            }

            // Hash the password before saving
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            var newUser = new SignUp
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Username = model.Username,
                Email = model.Email,
                Mobile = model.Mobile,
                Password = hashedPassword, // Store the hashed password
                ConformPassword = hashedPassword // Store the hashed password
            };

            context.signUps.Add(newUser);
            context.SaveChanges();

            TempData["SuccessMessage"] = "Registration successful! Please log in.";
            return RedirectToAction("Login");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "An error occurred during registration.";
            return View(model);
        }
   
    }
}

//Admin Controller

    public class AdminController : Controller
    {
        private readonly ApplicationContext context;

        public AdminController(ApplicationContext context)
        {
            this.context = context;
        }

        public IActionResult AdminLogin()
        {
            return View();
        }

    [HttpPost]
    
    public IActionResult AdminLogin(AdminModel model)
    {
        if (ModelState.IsValid)
        {
            var adData = context.adminModel.FirstOrDefault(a => a.Username == model.Username && a.Password == model.Password);
            if (adData != null)
            {
                var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, model.Username) },
                     CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                HttpContext.Session.SetInt32("UserId", adData.Id);  // store imp info in session


                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.msg = "<div class='alert alert-danger alert-dismissible fade show' role='alert'> Invalid Email Or Password!! <button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\">\r\n    <span aria-hidden=\"true\">&times;</span>\r\n  </button>\r\n</div>";
                return View(model);
            }
        }
        return View();
    }

    public IActionResult LogOut()
    {

        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        var StoredCookies = Request.Cookies.Keys; // After Logout Delete the all cookies.
        foreach (var cookie in StoredCookies)
        {
            Response.Cookies.Delete(cookie);
        }
        return RedirectToAction("Login", "Account");
    }


}
