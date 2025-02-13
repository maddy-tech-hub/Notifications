using Notifications.Application;
using Notifications.Domain;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Notifications.Infrastructure
{
    public class EmailClient : IEmailClient
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailClient(SmtpSettings smtpSettings)
        {
            _smtpSettings = smtpSettings;
        }

        public async Task<bool> SendEmailAsync(EmailRequest emailRequest)
        {
            try
            {
                using (var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port))
                {
                    client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                    client.EnableSsl = _smtpSettings.EnableSsl;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
                        Subject = emailRequest.Subject,
                        Body = emailRequest.Body,
                        IsBodyHtml = emailRequest.IsHtml
                    };

                    mailMessage.To.Add(emailRequest.ToEmail);
                    await client.SendMailAsync(mailMessage);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
                return false;
            }
        }
    }
}
