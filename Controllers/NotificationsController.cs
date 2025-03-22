using Ecommerce.Repository.NotificationService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserNotifications(string userId)
        {
            var result = await _notificationService.GetUserNotifications(userId);
            return result.Status ? Ok(result) : BadRequest(result);
        }
        [HttpPut("{notificationId}")]
        public async Task<IActionResult> MarkNotificationAsRead(int notificationId)
        {
            var result = await _notificationService.MarkNotificationAsRead(notificationId);
            return result.Status ? Ok(result) : BadRequest(result);
        }
    }
}
