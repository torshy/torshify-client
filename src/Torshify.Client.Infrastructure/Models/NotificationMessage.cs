namespace Torshify.Client.Infrastructure.Models
{
    public class NotificationMessage
    {
        public NotificationMessage(string message)
        {
            Message = message;
        }

        public string Message
        {
            get; 
            private set;
        }
    }
}