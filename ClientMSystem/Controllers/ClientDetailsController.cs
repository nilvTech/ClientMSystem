using ClientMSystem.Data;
using ClientMSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientMSystem.Controllers
{
    public class ClientDetailsController : Controller
    {
        private readonly ApplicationContext context;

        public ClientDetailsController(ApplicationContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return View();
            }
            else
            {
                var result = context.clientDetails.Where(x => x.UserId == userId).ToList();
                return View(result);
            }
            
        }

        //Create


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ClientDetail model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (ModelState.IsValid && userId.HasValue)
            {
                var rec = new ClientDetail()
                {
                    UserId = userId.Value,
                    Name = model.Name,
                    ClientName = model.ClientName,
                    IssuedDate = model.IssuedDate,
                    DomainName = model.DomainName,
                    Technology = model.Technology,
                    Assigned = model.Assigned,
                };
                context.clientDetails.Add(rec);
                context.SaveChanges();
                TempData["Message"] = "Sheet Updated Successfully";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Enter All Details";
                return View(model);
            }
        }

        public IActionResult Delete(int id)
        {
            var rec = context.clientDetails.SingleOrDefault(e => e.Id == id);

            if (rec == null)
            {
                TempData["Error"] = "Record not found";
                return RedirectToAction("Index");
            }

            context.clientDetails.Remove(rec);
            context.SaveChanges();

            TempData["Error"] = "Record deleted successfully";
            return RedirectToAction("Index");
        }


        public IActionResult Edit(int id)
        {
            var model = context.clientDetails.SingleOrDefault(e => e.Id == id);
            var result = new ClientDetail()
            {
                UserId = model.UserId,
                Name = model.Name,
                ClientName = model.ClientName,
                IssuedDate = model.IssuedDate,
                DomainName = model.DomainName,
                Technology = model.Technology,
                Assigned = model.Assigned,

            };
            return View(result);
        }
        [HttpPost]
        public IActionResult Edit(ClientDetail model)
        {
            if (!ModelState.IsValid)
            {
                // Handle the case when model validation fails
                TempData["Error"] = "Please correct the errors and try again.";
                return View(model);
            }

            var rec = context.clientDetails.SingleOrDefault(e => e.Id == model.Id);
            if (rec == null)
            {
                TempData["Error"] = "Record not found";
                return RedirectToAction("Index");
            }

            // Update the existing record with the new values
            rec.UserId = model.UserId;
            rec.Name = model.Name;
            rec.ClientName = model.ClientName;
            rec.IssuedDate = model.IssuedDate;
            rec.DomainName = model.DomainName;
            rec.Technology = model.Technology;
            rec.Assigned = model.Assigned;

            context.clientDetails.Update(rec);
            context.SaveChanges();

            TempData["Error"] = "Sheet Updated Successfully";
            return RedirectToAction("Index");
        }

    }

}
