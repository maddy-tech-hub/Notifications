using Notifications.Domain;

namespace Notifications.Application
{
    public interface IEmailClient
    {
        Task<bool> SendEmailAsync(EmailRequest emailRequest);
    }
}
