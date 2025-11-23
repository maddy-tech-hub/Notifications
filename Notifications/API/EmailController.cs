using Microsoft.AspNetCore.Mvc;
using Notifications.Application;
using Notifications.Domain;
using System.Threading.Tasks;

namespace Notifications.API
{
    [ApiController]
    [Route("api/email")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

         [HttpGet("sent")]
        public async Task<IActionResult> Sent()
        {
                return Ok(new { message = "Email sent successfully." });
        }

        [HttpPost("send-contact-email")]
        public async Task<IActionResult> SendContactEmail([FromBody] EmailRequestDto emailRequestDto)
        {
            var result = await _emailService.SendContactEmailAsync(emailRequestDto);
            if (result)
                return Ok(new { message = "Email sent successfully." });
            else
                return BadRequest(new { message = "Email sending failed." });
        }
    }
}
