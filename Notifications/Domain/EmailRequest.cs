namespace Notifications.Domain
{
    public class EmailRequest
    {
        public string ToEmail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsHtml { get; set; } = true;
    }
    public class EmailTemplate
    {
        public string Subject { get; set; }
        public string Message { get; set; }
        public string HtmlBody { get; set; }
    }
}
