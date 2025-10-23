using CHNU_Connect.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CHNU_Connect.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly ILogger<MessageController> _logger;

        public MessageController(IMessageService messageService, ILogger<MessageController> logger)
        {
            _messageService = messageService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMessage(int id)
        {
            try
            {
                var message = await _messageService.GetByIdAsync(id);
                if (message == null)
                    return NotFound(new { message = "Message not found." });

                return Ok(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting message: {MessageId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the message." });
            }
        }

        [HttpGet("conversation/{userId}")]
        public async Task<IActionResult> GetConversation(int userId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized();

                var messages = await _messageService.GetConversationAsync(currentUserId.Value, userId);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting conversation between users: {UserId1} and {UserId2}", GetCurrentUserId(), userId);
                return StatusCode(500, new { message = "An error occurred while retrieving the conversation." });
            }
        }

        [HttpGet("my-messages")]
        public async Task<IActionResult> GetMyMessages()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized();

                var messages = await _messageService.GetUserMessagesAsync(currentUserId.Value);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting messages for user: {UserId}", GetCurrentUserId());
                return StatusCode(500, new { message = "An error occurred while retrieving your messages." });
            }
        }

        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadMessages()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized();

                var messages = await _messageService.GetUnreadMessagesAsync(currentUserId.Value);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread messages for user: {UserId}", GetCurrentUserId());
                return StatusCode(500, new { message = "An error occurred while retrieving unread messages." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized();

                if (string.IsNullOrWhiteSpace(request.Content))
                    return BadRequest(new { message = "Message content cannot be empty." });

                var message = await _messageService.CreateMessageAsync(currentUserId.Value, request.ReceiverId, request.Content);
                
                _logger.LogInformation("Message sent from user: {SenderId} to user: {ReceiverId}", currentUserId, request.ReceiverId);
                return CreatedAtAction(nameof(GetMessage), new { id = ((dynamic)message).Id }, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message from user: {SenderId}", GetCurrentUserId());
                return StatusCode(500, new { message = "An error occurred while sending the message." });
            }
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized();

                var success = await _messageService.MarkAsReadAsync(id);
                if (!success)
                    return BadRequest(new { message = "Failed to mark message as read." });

                _logger.LogInformation("Message marked as read: {MessageId} by user: {UserId}", id, currentUserId);
                return Ok(new { message = "Message marked as read." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking message as read: {MessageId}", id);
                return StatusCode(500, new { message = "An error occurred while marking the message as read." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized();

                var success = await _messageService.DeleteMessageAsync(id);
                if (!success)
                    return BadRequest(new { message = "Failed to delete message." });

                _logger.LogInformation("Message deleted: {MessageId} by user: {UserId}", id, currentUserId);
                return Ok(new { message = "Message deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting message: {MessageId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the message." });
            }
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }

    public class SendMessageDto
    {
        public int ReceiverId { get; set; }
        public string Content { get; set; } = null!;
    }
}
