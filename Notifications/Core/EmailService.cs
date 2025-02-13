using Notifications.Application;
using Notifications.Domain;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace Notifications.Core
{
    public class EmailService : IEmailService
    {
        private readonly IEmailClient _emailClient;
        private readonly Dictionary<string, EmailTemplate> _emailTemplates;
        private readonly SmtpSettings _smtp;

        public EmailService(IEmailClient emailClient, Dictionary<string, EmailTemplate> emailTemplates, SmtpSettings smtpSettings)
        {
            _emailClient = emailClient;
            _emailTemplates = emailTemplates;
            _smtp = smtpSettings;
        }

        public async Task<bool> SendContactEmailAsync(EmailRequestDto emailRequestDto)
        {
            if (!_emailTemplates.TryGetValue("ContactForm", out var contactTemplate) ||
                !_emailTemplates.TryGetValue("ThankYouEmail", out var thankYouTemplate))
            {
                return false;
            }

            var adminEmail = new EmailRequest
            {
                ToEmail = _smtp.FromEmail,
                Subject = contactTemplate.Subject,
                Body = contactTemplate.HtmlBody
                    .Replace("{FullName}", emailRequestDto.FullName)
                    .Replace("{Email}", emailRequestDto.Email)
                    .Replace("{PhoneNumber}", emailRequestDto.Phone)
                    .Replace("{Message}", emailRequestDto.Message),
                IsHtml = true
            };

            bool adminEmailSent = await _emailClient.SendEmailAsync(adminEmail);
            if (!adminEmailSent) return false;

            var thankYouEmail = new EmailRequest
            {
                ToEmail = emailRequestDto.Email,
                Subject = thankYouTemplate.Subject,
                Body = thankYouTemplate.HtmlBody
                    .Replace("{FullName}", emailRequestDto.FullName)
                    .Replace("{TeamName}", "Maddy Tech"),
                IsHtml = true
            };

            return await _emailClient.SendEmailAsync(thankYouEmail);
        }
    }

}
