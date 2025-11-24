using Notifications.Application;
using Notifications.Domain;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Notifications.Infrastructure
{
    public class EmailClient : IEmailClient
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ILogger<EmailClient> _logger;

        public EmailClient(SmtpSettings smtpSettings, ILogger<EmailClient> logger)
        {
            _smtpSettings = smtpSettings;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(EmailRequest emailRequest)
        {
            _logger.LogInformation(
                "EmailClient: Preparing email → To: {ToEmail}, Subject: {Subject}, Host: {Host}, Port: {Port}",
                emailRequest.ToEmail,
                emailRequest.Subject,
                _smtpSettings.Host,
                _smtpSettings.Port
            );

            try
            {
                using (var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port))
                {
                    // Use Brevo key from env var if Password missing
                    var smtpPassword = string.IsNullOrWhiteSpace(_smtpSettings.Password)
                        ? Environment.GetEnvironmentVariable("BREVO_SMTP_KEY")
                        : _smtpSettings.Password;

                    client.Credentials = new NetworkCredential(_smtpSettings.Username, smtpPassword);
                    client.EnableSsl = _smtpSettings.EnableSsl;
                    client.Timeout = 10000; // 10 seconds

                    _logger.LogDebug(
                        "EmailClient: SMTP Credentials set. EnableSSL={EnableSsl}, Username={Username}",
                        _smtpSettings.EnableSsl,
                        _smtpSettings.Username
                    );

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
                        Subject = emailRequest.Subject,
                        Body = emailRequest.Body,
                        IsBodyHtml = emailRequest.IsHtml
                    };

                    mailMessage.To.Add(emailRequest.ToEmail);

                    _logger.LogInformation("EmailClient: Sending email to {ToEmail}", emailRequest.ToEmail);

                    await client.SendMailAsync(mailMessage);

                    _logger.LogInformation("EmailClient: Email successfully sent to {ToEmail}", emailRequest.ToEmail);

                    return true;
                }
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError(
                    smtpEx,
                    "EmailClient: SMTP error while sending email to {ToEmail}. StatusCode={StatusCode}",
                    emailRequest.ToEmail,
                    smtpEx.StatusCode
                );
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "EmailClient: Unexpected error while sending email to {ToEmail}",
                    emailRequest.ToEmail
                );
                return false;
            }
        }
    }
}
