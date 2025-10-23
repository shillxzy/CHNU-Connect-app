using CHNU_Connect.BLL.DTOs.Event;
using CHNU_Connect.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CHNU_Connect.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly ILogger<EventController> _logger;

        public EventController(IEventService eventService, ILogger<EventController> logger)
        {
            _eventService = eventService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            try
            {
                var events = await _eventService.GetAllAsync();
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
                var eventEntity = await _eventService.GetByIdAsync(id);
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
                if (currentUserId == null)
                    return Unauthorized();

                var events = await _eventService.GetUserEventsAsync(currentUserId.Value);
                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting my events for user: {UserId}", GetCurrentUserId());
                return StatusCode(500, new { message = "An error occurred while retrieving your events." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized();

                request.CreatedById = currentUserId.Value;
                var eventEntity = await _eventService.CreateEventAsync(request);
                
                _logger.LogInformation("Event created by user: {UserId}", currentUserId);
                return CreatedAtAction(nameof(GetEvent), new { id = eventEntity.Id }, eventEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event for user: {UserId}", GetCurrentUserId());
                return StatusCode(500, new { message = "An error occurred while creating the event." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] CreateEventDto request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized();

                var eventEntity = await _eventService.GetByIdAsync(id);
                if (eventEntity == null)
                    return NotFound(new { message = "Event not found." });

                // Check if user is the creator
                if (eventEntity.CreatedById != currentUserId.Value)
                    return Forbid("You can only edit events you created.");

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
                if (currentUserId == null)
                    return Unauthorized();

                var eventEntity = await _eventService.GetByIdAsync(id);
                if (eventEntity == null)
                    return NotFound(new { message = "Event not found." });

                // Check if user is the creator
                if (eventEntity.CreatedById != currentUserId.Value)
                    return Forbid("You can only delete events you created.");

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
                if (currentUserId == null)
                    return Unauthorized();

                var success = await _eventService.JoinEventAsync(id, currentUserId.Value);
                if (!success)
                    return BadRequest(new { message = "Already joined this event or event not found." });

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
                if (currentUserId == null)
                    return Unauthorized();

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

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }
}
