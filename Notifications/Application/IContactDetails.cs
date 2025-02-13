namespace Notifications.Application
{
    public interface IContactDetails
    {
        string FullName { get; set; }
        string Email { get; set; }
        string Phone { get; set; }
        string Message { get; set; }
    }
}
