using Ecommerce.Models;

namespace Ecommerce.DTO_s.Notification
{
    public class NotificationGetDTO: AbstractModel
    {
        public string UserId { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
    }
}
