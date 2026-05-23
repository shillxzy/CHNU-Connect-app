using CHNU_Connect.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CHNU_Connect.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationController(INotificationService service)
        {
            _service = service;
        }

        // GET /api/Notification/user/{userId} — список непрочитаних
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUnread(int userId)
        {
            var list = await _service.GetUnreadNotificationsAsync(userId);
            return Ok(list);
        }

        // GET /api/Notification/count/{userId} — кількість непрочитаних
        [HttpGet("count/{userId}")]
        public async Task<IActionResult> GetCount(int userId)
        {
            var count = await _service.GetUnreadCountAsync(userId);
            return Ok(count);
        }

        // POST /api/Notification/{id}/read — позначити одну як прочитану
        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _service.MarkNotificationAsReadAsync(id);
            return NoContent();
        }

        // GET /api/Notification/all/{userId} — всі сповіщення (50 останніх)
        [HttpGet("all/{userId}")]
        public async Task<IActionResult> GetAll(int userId)
        {
            var list = await _service.GetAllNotificationsAsync(userId);
            return Ok(list);
        }

        // POST /api/Notification/read-all — позначити всі як прочитані
        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();
            await _service.MarkAllAsReadAsync(userId.Value);
            return NoContent();
        }

        private int? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }
    }
}
