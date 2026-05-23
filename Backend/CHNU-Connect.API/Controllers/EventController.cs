using CHNU_Connect.API.Hubs;
using CHNU_Connect.BLL.DTOs.Event;
using CHNU_Connect.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace CHNU_Connect.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IAdminPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ILogger<EventController> _logger;

        public EventController(
            IEventService eventService,
            IAdminPermissionService permissionService,
            INotificationService notificationService,
            IHubContext<ChatHub> hubContext,
            ILogger<EventController> logger)
        {
            _eventService = eventService;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _hubContext = hubContext;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var events = await _eventService.GetAllAsync(currentUserId);
                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all events");
                return StatusCode(500, new { message = "An error occurred while retrieving events." });
            }
        }

        [HttpGet("public")]
        public async Task<IActionResult> GetPublicEvents()
        {
            try
            {
                var events = await _eventService.GetPublicEventsAsync();
                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting public events");
                return StatusCode(500, new { message = "An error occurred while retrieving public events." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var eventEntity = await _eventService.GetByIdAsync(id, currentUserId);
                if (eventEntity == null)
                    return NotFound(new { message = "Event not found." });

                return Ok(eventEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting event: {EventId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the event." });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetEventsByUser(int userId)
        {
            try
            {
                var events = await _eventService.GetByCreatorIdAsync(userId);
                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting events for user: {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred while retrieving user events." });
            }
        }

        [HttpGet("my-events")]
        public async Task<IActionResult> GetMyEvents()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null) return Unauthorized();

                var events = await _eventService.GetUserEventsAsync(currentUserId.Value);
                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting my events");
                return StatusCode(500, new { message = "An error occurred while retrieving your events." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null) return Unauthorized();

                if (!await CanManageEvents())
                    return StatusCode(403, new { message = "You need ManageEvents permission to create events." });

                request.CreatedById = currentUserId.Value;

                var eventEntity = await _eventService.CreateEventAsync(request);
                return CreatedAtAction(nameof(GetEvent), new { id = eventEntity.Id }, eventEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event");
                return StatusCode(500, new { message = "An error occurred while creating the event." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] CreateEventDto request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null) return Unauthorized();

                var eventEntity = await _eventService.GetByIdAsync(id);
                if (eventEntity == null)
                    return NotFound(new { message = "Event not found." });

                var isAdmin = User.IsInRole("admin") || User.IsInRole("superAdmin");
                if (eventEntity.CreatedById != currentUserId.Value && !isAdmin)
                    return StatusCode(403, new { message = "You can only edit events you created." });

                if (isAdmin && !await CanManageEvents())
                    return StatusCode(403, new { message = "You need ManageEvents permission to edit events." });

                var updatedEvent = await _eventService.UpdateEventAsync(id, request);

                _logger.LogInformation("Event updated: {EventId} by user: {UserId}", id, currentUserId);
                return Ok(updatedEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating event: {EventId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the event." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null) return Unauthorized();

                var eventEntity = await _eventService.GetByIdAsync(id);
                if (eventEntity == null)
                    return NotFound(new { message = "Event not found." });

                var isAdmin = User.IsInRole("admin") || User.IsInRole("superAdmin");
                if (eventEntity.CreatedById != currentUserId.Value && !isAdmin)
                    return StatusCode(403, new { message = "You can only delete events you created." });

                if (isAdmin && !await CanManageEvents())
                    return StatusCode(403, new { message = "You need ManageEvents permission to delete events." });

                var success = await _eventService.DeleteEventAsync(id);
                if (!success)
                    return BadRequest(new { message = "Failed to delete event." });

                _logger.LogInformation("Event deleted: {EventId} by user: {UserId}", id, currentUserId);
                return Ok(new { message = "Event deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting event: {EventId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the event." });
            }
        }

        [HttpPost("{id}/join")]
        public async Task<IActionResult> JoinEvent(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null) return Unauthorized();

                var eventEntity = await _eventService.GetByIdAsync(id);
                if (eventEntity == null)
                    return NotFound(new { message = "Event not found." });

                var success = await _eventService.JoinEventAsync(id, currentUserId.Value);
                if (!success)
                    return BadRequest(new { message = "Already joined this event or event not found." });

                if (eventEntity.CreatedById != currentUserId.Value)
                {
                    var notification = await _notificationService.CreateAsync(
                        eventEntity.CreatedById, "event", id, actorId: currentUserId.Value,
                        body: $"Хтось приєднався до події «{eventEntity.Title}»");
                    await _hubContext.Clients
                        .Group($"user-{eventEntity.CreatedById}")
                        .SendAsync("ReceiveNotification", notification);
                }

                _logger.LogInformation("User joined event: {EventId} by user: {UserId}", id, currentUserId);
                return Ok(new { message = "Successfully joined the event." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining event: {EventId}", id);
                return StatusCode(500, new { message = "An error occurred while joining the event." });
            }
        }

        [HttpDelete("{id}/leave")]
        public async Task<IActionResult> LeaveEvent(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null) return Unauthorized();

                var success = await _eventService.LeaveEventAsync(id, currentUserId.Value);
                if (!success)
                    return BadRequest(new { message = "Not joined this event or event not found." });

                _logger.LogInformation("User left event: {EventId} by user: {UserId}", id, currentUserId);
                return Ok(new { message = "Successfully left the event." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error leaving event: {EventId}", id);
                return StatusCode(500, new { message = "An error occurred while leaving the event." });
            }
        }

        [HttpPut("{id}/image")]
        public async Task<IActionResult> UploadEventImage(int id, IFormFile image)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null) return Unauthorized();

                var eventEntity = await _eventService.GetByIdAsync(id);
                if (eventEntity == null)
                    return NotFound(new { message = "Event not found." });

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}_{image.FileName}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                var imageUrl = $"/uploads/{fileName}";
                await _eventService.UpdateEventImageAsync(id, imageUrl);

                return Ok(new { imageUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image for event: {EventId}", id);
                return StatusCode(500, new { message = "An error occurred while uploading the event image." });
            }
        }

        // ==================== HELPERS ====================

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        [HttpPost("{id}/invite")]
        public async Task<IActionResult> InviteUser(int id, [FromBody] int targetUserId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null) return Unauthorized();

                var eventEntity = await _eventService.GetByIdAsync(id);
                if (eventEntity == null)
                    return NotFound(new { message = "Event not found." });

                var isAdmin = User.IsInRole("admin") || User.IsInRole("superAdmin");
                var isTeacher = User.IsInRole("teacher");
                if (eventEntity.CreatedById != currentUserId.Value && !isAdmin && !isTeacher)
                    return StatusCode(403, new { message = "Only the event creator, admin, or teacher can invite users." });

                var success = await _eventService.JoinEventAsync(id, targetUserId);
                if (!success)
                    return BadRequest(new { message = "User is already a participant." });

                var notification = await _notificationService.CreateAsync(
                    targetUserId, "event", id, actorId: currentUserId.Value,
                    body: $"Вас запрошено до події «{eventEntity.Title}»");
                await _hubContext.Clients
                    .Group($"user-{targetUserId}")
                    .SendAsync("ReceiveNotification", notification);

                return Ok(new { message = "User invited successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inviting user to event: {EventId}", id);
                return StatusCode(500, new { message = "An error occurred while inviting the user." });
            }
        }

        private async Task<bool> CanManageEvents()
        {
            if (User.IsInRole("superAdmin") || User.IsInRole("admin") || User.IsInRole("teacher")) return true;
            var userId = GetCurrentUserId();
            if (userId == null) return false;
            return await _permissionService.HasPermissionAsync(userId.Value, "ManageEvents");
        }
    }
}
