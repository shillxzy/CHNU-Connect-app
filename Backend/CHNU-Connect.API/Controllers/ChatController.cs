using CHNU_Connect.API.Hubs;
using CHNU_Connect.BLL.DTOs.Chat;
using CHNU_Connect.BLL.DTOs.ChatMember;
using CHNU_Connect.BLL.DTOs.ChatMessage;
using CHNU_Connect.BLL.DTOs.Notification;
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
    public class ChatController : ControllerBase
    {
        private readonly IChatService             _chatService;
        private readonly INotificationService     _notificationService;
        private readonly IHubContext<ChatHub>     _hubContext;

        public ChatController(
            IChatService         chatService,
            INotificationService notificationService,
            IHubContext<ChatHub> hubContext)
        {
            _chatService         = chatService;
            _notificationService = notificationService;
            _hubContext          = hubContext;
        }

        // -------------------- Чати --------------------

        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetChat(int chatId)
        {
            var chat = await _chatService.GetChatByIdAsync(chatId);
            if (chat == null) return NotFound();
            return Ok(chat);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserChats(int userId)
        {
            var chats = await _chatService.GetUserChatsAsync(userId);
            return Ok(chats);
        }

        [HttpPost]
        public async Task<IActionResult> CreateChat([FromBody] CreateChatDto dto)
        {
            var chat = await _chatService.CreateChatAsync(dto);
            return CreatedAtAction(nameof(GetChat), new { chatId = chat.Id }, chat);
        }

        // -------------------- Учасники --------------------

        [HttpPost("{chatId}/members")]
        public async Task<IActionResult> AddMember(int chatId, [FromBody] CreateChatMemberDto dto)
        {
            if (chatId != dto.ChatId) return BadRequest("ChatId mismatch");
            var member = await _chatService.AddMemberAsync(dto);
            return Ok(member);
        }

        // -------------------- Повідомлення --------------------

        [HttpGet("{chatId}/messages")]
        public async Task<IActionResult> GetMessages(int chatId, [FromQuery] int limit = 50)
        {
            var messages = await _chatService.GetMessagesAsync(chatId, limit);
            return Ok(messages);
        }

        [HttpPost("{chatId}/messages")]
        public async Task<IActionResult> SendMessage(int chatId, [FromBody] CreateChatMessageDto dto)
        {
            if (chatId != dto.ChatId) return BadRequest("ChatId mismatch");

            var message = await _chatService.SendMessageAsync(dto);

            // 1. Відправити повідомлення всім у чаті через SignalR
            await _hubContext.Clients
                .Group($"chat-{chatId}")
                .SendAsync("ReceiveMessage", message);

            // 2. Відправити notification через SignalR кожному учаснику (крім відправника)
            var chat = await _chatService.GetChatByIdAsync(chatId);
            if (chat?.Members != null)
            {
                foreach (var member in chat.Members.Where(m => m.UserId != dto.SenderId))
                {
                    var notification = await _notificationService.CreateAsync(
                        member.UserId, "message", message.Id);

                    // Push через SignalR якщо юзер онлайн
                    await _hubContext.Clients
                        .Group($"user-{member.UserId}")
                        .SendAsync("ReceiveNotification", notification);
                }
            }

            return Ok(message);
        }

        [HttpPost("{chatId}/messages/{messageId}/read/{userId}")]
        public async Task<IActionResult> MarkMessageAsRead(int chatId, int messageId, int userId)
        {
            await _chatService.MarkMessageAsReadAsync(chatId, userId, messageId);
            return NoContent();
        }

        [HttpPut("{chatId}/messages/{messageId}")]
        public async Task<IActionResult> UpdateMessage(int chatId, int messageId, [FromBody] UpdateChatMessageDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            var result = await _chatService.UpdateMessageAsync(messageId, userId, dto.Content);
            if (result == null) return NotFound();

            // Сповістити інших учасників про редагування
            await _hubContext.Clients
                .Group($"chat-{chatId}")
                .SendAsync("MessageUpdated", result);

            return Ok(result);
        }

        [HttpDelete("{chatId}/messages/{messageId}")]
        public async Task<IActionResult> DeleteMessage(int chatId, int messageId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _chatService.DeleteMessageAsync(messageId, userId);

            // Сповістити інших учасників про видалення
            await _hubContext.Clients
                .Group($"chat-{chatId}")
                .SendAsync("MessageDeleted", messageId);

            return NoContent();
        }
    }
}
