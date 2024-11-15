using ClientMSystem.Models;
using ClientMSystem.Services.EmailService;
using Microsoft.AspNetCore.Mvc;

namespace ClientMSystem.Controllers
{
    public class EmailController : Controller
    {

        private readonly IEmailService _emailService;
        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SendEmail(EmailDto request)
        {
            _emailService.SendEmail(request);
            return View();

        }
    }
}
