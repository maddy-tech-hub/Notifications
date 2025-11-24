using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Notifications.Application;
using Notifications.Domain;

namespace Notifications.Core
{
    public class EmailService : IEmailService
    {
        private readonly IEmailClient _emailClient;
        private readonly Dictionary<string, EmailTemplate> _emailTemplates;
        private readonly SmtpSettings _smtp;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IEmailClient emailClient,
            Dictionary<string, EmailTemplate> emailTemplates,
            SmtpSettings smtpSettings,
            ILogger<EmailService> logger)
        {
            _emailClient = emailClient ?? throw new ArgumentNullException(nameof(emailClient));
            _emailTemplates = emailTemplates ?? new Dictionary<string, EmailTemplate>();
            _smtp = smtpSettings ?? throw new ArgumentNullException(nameof(smtpSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> SendContactEmailAsync(EmailRequestDto emailRequestDto)
        {
            if (emailRequestDto == null)
            {
                _logger.LogWarning("SendContactEmailAsync called with null request.");
                return false;
            }

            _logger.LogInformation("SendContactEmailAsync: Starting for {Email} (Name: {FullName})", emailRequestDto.Email, emailRequestDto.FullName);

            if (!_emailTemplates.TryGetValue("ContactForm", out var contactTemplate))
            {
                _logger.LogWarning("Template 'ContactForm' not found.");
                return false;
            }

            if (!_emailTemplates.TryGetValue("ThankYouEmail", out var thankYouTemplate))
            {
                _logger.LogWarning("Template 'ThankYouEmail' not found.");
                return false;
            }

            var adminBody = contactTemplate.HtmlBody
                .Replace("{FullName}", emailRequestDto.FullName)
                .Replace("{Email}", emailRequestDto.Email)
                .Replace("{PhoneNumber}", emailRequestDto.Phone)
                .Replace("{Message}", emailRequestDto.Message);

            var adminEmail = new EmailRequest
            {
                ToEmail = _smtp.FromEmail,
                Subject = contactTemplate.Subject,
                Body = adminBody,
                IsHtml = true
            };

            _logger.LogInformation("Sending admin email to {AdminEmail} (Subject: {Subject})", adminEmail.ToEmail, adminEmail.Subject);
            bool adminSent;
            try
            {
                adminSent = await _emailClient.SendEmailAsync(adminEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while sending admin email to {AdminEmail}", adminEmail.ToEmail);
                return false;
            }

            if (!adminSent)
            {
                _logger.LogWarning("Admin email failed to send to {AdminEmail}", adminEmail.ToEmail);
                return false;
            }

            var thankYouBody = thankYouTemplate.HtmlBody
                .Replace("{FullName}", emailRequestDto.FullName)
                .Replace("{TeamName}", "Maddy Tech");

            var thankYouEmail = new EmailRequest
            {
                ToEmail = emailRequestDto.Email,
                Subject = thankYouTemplate.Subject.Replace("{TeamName}", "Maddy Tech"),
                Body = thankYouBody,
                IsHtml = true
            };

            _logger.LogInformation("Sending thank-you email to {Recipient}", thankYouEmail.ToEmail);
            bool thankYouSent;
            try
            {
                thankYouSent = await _emailClient.SendEmailAsync(thankYouEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while sending thank-you email to {Recipient}", thankYouEmail.ToEmail);
                return false;
            }

            if (!thankYouSent)
            {
                _logger.LogWarning("Thank-you email failed to send to {Recipient}", thankYouEmail.ToEmail);
            }

            _logger.LogInformation("SendContactEmailAsync finished for {Email}. adminSent={AdminSent}, thankYouSent={ThankYouSent}", emailRequestDto.Email, adminSent, thankYouSent);

            return thankYouSent;
        }
    }
}
