using Notifications.Domain;

namespace Notifications.Application
{
    public interface IEmailService
    {
        Task<bool> SendContactEmailAsync(EmailRequestDto emailRequestDto);
    }

}
