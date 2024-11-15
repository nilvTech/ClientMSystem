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
            // var userId = HttpContext.Session.GetInt32("UserId");
            var data = context.signUps.FirstOrDefault(e => e.Username == model.Username);

            if (data != null)
            {
                bool isValid = (data.Username == model.Username && data.Password == model.Password);
                if (isValid)
                {
                    var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, model.Username) },
                        CookieAuthenticationDefaults.AuthenticationScheme);

                    var principal = new ClaimsPrincipal(identity);
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    HttpContext.Session.SetInt32("UserId", data.ID); // store imp info in session

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.msg = "<div class='alert alert-danger alert-dismissible fade show' role='alert'> Invalid Email Or Password!! <button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\">\r\n    <span aria-hidden=\"true\">&times;</span>\r\n  </button>\r\n</div>";
                    TempData["Errormessage"] = "Check Credentials";
                    return View(model);
                }
            }
            else
            {
                TempData["Errormessage"] = "UserName Not found";
                return View(model);
            }
        }

        public IActionResult LogOut()
        {
            
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var StoredCookies = Request.Cookies.Keys; // After Logout Delete the all cookies.
            foreach(var cookie in StoredCookies)
            {
                Response.Cookies.Delete(cookie);
            }
            return RedirectToAction("Login", "Account");
        }

        //*****************************************************************************SignUp****************************************
        [AcceptVerbs("Post", "Get")]
        public IActionResult UserNameIsExits(string Uname)
        {
            var data = context.signUps.SingleOrDefault(e => e.Username == Uname);

            if (data != null)
            {
                return Json($"Username {Uname} already exists");
            }
            else
            {
                return Json(true);
            }
        }


        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignUp(SignUp model)
        {
            if (ModelState.IsValid)
            {
                var data = new SignUp
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Username = model.Username,
                    Email = model.Email,
                    Mobile = model.Mobile,
                    Password = model.Password,
                    ConformPassword = model.ConformPassword
                };

                context.signUps.Add(data);
                context.SaveChanges();
                TempData["SuccessMessage"] = "User Registration Successfully!! Please Login!";
                return RedirectToAction("Login");
            }
            else
            {
                TempData["errorMessage"] = "Fill in all the fields.";
                return View(model);
            }
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
