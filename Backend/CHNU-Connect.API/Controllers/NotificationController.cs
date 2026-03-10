using CHNU_Connect.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CHNU_Connect.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("unread/{userId}")]
        public async Task<IActionResult> GetUnreadCount(int userId)
        {
            var count = await _notificationService.GetUnreadCountAsync(userId);
            return Ok(count);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUnreadNotifications(int userId)
        {
            var notifications = await _notificationService.GetUnreadNotificationsAsync(userId);
            return Ok(notifications);
        }

        [HttpPost("{notificationId}/read")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            await _notificationService.MarkNotificationAsReadAsync(notificationId);
            return NoContent();
        }
    }
}
