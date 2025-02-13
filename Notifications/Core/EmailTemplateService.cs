using Notifications.Application;
using Notifications.Domain;
using System.Collections.Generic;
using System.Linq;

namespace Notifications.Core
{
    public class EmailTemplateService : IEmailTemplateService
    {
        public string GenerateEmailBody(string template, List<EmailTemplatePlaceholder> placeholders)
        {
            foreach (var placeholder in placeholders)
            {
                template = template.Replace(placeholder.Placeholder, placeholder.Value);
            }
            return template;
        }
    }
}
