using Notifications.Domain;

namespace Notifications.Application
{
    public interface IEmailTemplateService
    {
        string GenerateEmailBody(string template, List<EmailTemplatePlaceholder> placeholders);
    }
}
