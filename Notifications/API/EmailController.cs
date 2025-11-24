using Microsoft.AspNetCore.Mvc;
using Notifications.Application;
using Notifications.Domain;
using System.Net.Sockets;
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

        [HttpGet("diag/smtp")]
        public async Task<IActionResult> TestSmtp()
        {
            try
            {
                using var tcp = new TcpClient();
                var task = tcp.ConnectAsync("smtp-relay.brevo.com", 2525);
                if (!task.Wait(TimeSpan.FromSeconds(5)))
                    return StatusCode(500, "Port blocked or timed out");
                return Ok("Port 2525 is OPEN");
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
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
