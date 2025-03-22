using Ecommerce.Helper;
using Ecommerce.Models;

namespace Ecommerce.Repository.NotificationService
{
    public interface INotificationService
    {
        Task SendNotification(string message, string userId);
        Task<ResponseResult> GetUserNotifications(string userId);
        Task<ResponseResult> MarkNotificationAsRead(int notificationId);
    }
}
