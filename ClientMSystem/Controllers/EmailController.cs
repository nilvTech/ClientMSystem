using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ClientMSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailController> _logger;

        public EmailController(IEmailService emailService, ILogger<EmailController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("send")]
        public IActionResult SendEmail([FromBody] EmailDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _emailService.SendEmail(request);
                return Ok(new { Message = "Email sent successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email.");
                return StatusCode(500, new { Error = "An error occurred while sending the email." });
            }
        }
    }
}
