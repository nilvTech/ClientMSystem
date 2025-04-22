using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Mime;
using System.ComponentModel.DataAnnotations;

namespace ClientMSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailController> _logger;

        public EmailController(IEmailService emailService, ILogger<EmailController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>
        /// Sends an email based on the provided request.
        /// </summary>
        /// <param name="request">Email details</param>
        /// <returns>Success or error result</returns>
        [HttpPost("send")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendEmailAsync([FromBody] EmailDto request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                       .SelectMany(v => v.Errors)
                                       .Select(e => e.ErrorMessage)
                                       .ToList();

                return BadRequest(new { Errors = errors });
            }

            try
            {
                await _emailService.SendEmailAsync(request); // assume async support in service
                _logger.LogInformation("Email successfully sent to {Recipient}", request.To);
                return Ok(new { Message = "Email sent successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {Recipient}", request.To);
                return StatusCode(500, new { Error = "An unexpected error occurred while sending the email." });
            }
        }
    }

    // Example DTO and service interface for context
    public class EmailDto
    {
        [Required, EmailAddress]
        public string To { get; set; } = string.Empty;

        [Required]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Body { get; set; } = string.Empty;
    }

    public interface IEmailService
    {
        Task SendEmailAsync(EmailDto email); // async version
    }
}
