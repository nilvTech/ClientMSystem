//using ClientMSystem.Models;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;
//using ClientMSystem.Data;
//using Microsoft.AspNetCore.Authorization;

//namespace ClientMSystem.Controllers
//{
    
//    public class AdminController : Controller
//    {
//        private readonly ApplicationContext context;

//        public AdminController(ApplicationContext context)
//        {
//            this.context = context;
//        }

//        public IActionResult Login()
//        {
//            return View();
//        }

//        [HttpPost]
//        //[Authorize(Roles = "Admin")]
//        public IActionResult Login(AdminModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                var adData = context.adminModel.FirstOrDefault(a => a.Username == model.Username && a.Password == model.Password);
//                if (adData != null)
//                {
//                    var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, model.Username) },
//                         CookieAuthenticationDefaults.AuthenticationScheme);

//                    var principal = new ClaimsPrincipal(identity);
//                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
//                    //HttpContext.Session["UserName"] = adData.Username; // store imp info in session


//                    return RedirectToAction("Index", "Home");
//                }
//                else
//                {
//                    ViewBag.msg = "<div class='alert alert-danger alert-dismissible fade show' role='alert'> Invalid Email Or Password!! <button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\">\r\n    <span aria-hidden=\"true\">&times;</span>\r\n  </button>\r\n</div>";
//                    return View(model);
//                }
//            }
//            return View();
//        }



//    }


//}

//// GET: AdminController
////[Authorize(Roles = "Admin")]
////public ActionResult Index()
////{
////    return View();
////}

////[HttpPost]
////public IActionResult Index(AdminModel model)
////{
////    try
////    {
////        var data = context.adminModel.FirstOrDefault(a => a.Username == model.Username);

////        if (data != null)
////        {
////            bool isValid = (data.Username == model.Username && data.Password == model.Password);
////            if (isValid)
////            {
////                var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, model.Username) },
////                CookieAuthenticationDefaults.AuthenticationScheme);

////                var principal = new ClaimsPrincipal(identity);
////                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
////               /* HttpContext.Session.SetInt32("UserId", data.ID);*/ // store imp info in session

////                return RedirectToAction("Index", "Home");
////            }
////            else
////            {

////                TempData["Errormessage"] = "Check Credentials";
////                return View(model);

////            }
////        }
////        else
////        {
////            TempData["Errormessage"] = "UserName Not found";
////            return View(model);

////        }

////    }
////    catch (Exception ex)
////    {
////        throw ex;
////    }

////}
