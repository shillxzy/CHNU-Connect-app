using CHNU_Connect.BLL.DTOs.Chat;
using CHNU_Connect.BLL.DTOs.ChatMember;
using CHNU_Connect.BLL.DTOs.ChatMessage;
using CHNU_Connect.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using CHNU_Connect.API.Hubs;
using Microsoft.AspNetCore.SignalR;


namespace CHNU_Connect.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(IChatService chatService,IHubContext<ChatHub> hubContext)
        {
            _chatService = chatService;
            _hubContext = hubContext;
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
        public async Task<IActionResult> CreateChat([FromBody] CreateChatDto createChatDto)
        {
            var chat = await _chatService.CreateChatAsync(createChatDto);
            return CreatedAtAction(nameof(GetChat), new { chatId = chat.Id }, chat);
        }

        // -------------------- Учасники --------------------

        [HttpPost("{chatId}/members")]
        public async Task<IActionResult> AddMember(int chatId, [FromBody] CreateChatMemberDto createMemberDto)
        {
            if (chatId != createMemberDto.ChatId)
                return BadRequest("ChatId mismatch");

            var member = await _chatService.AddMemberAsync(createMemberDto);
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
        public async Task<IActionResult> SendMessage(int chatId,[FromBody] CreateChatMessageDto createMessageDto)
        {
            if (chatId != createMessageDto.ChatId)
                return BadRequest("ChatId mismatch");

            var message = await _chatService.SendMessageAsync(createMessageDto);

            await _hubContext.Clients
                .Group($"chat-{chatId}")
                .SendAsync("ReceiveMessage", message);

            return Ok(message);
        }


        [HttpPost("{chatId}/messages/{messageId}/read/{userId}")]
        public async Task<IActionResult> MarkMessageAsRead(int chatId, int messageId, int userId)
        {
            await _chatService.MarkMessageAsReadAsync(chatId, userId, messageId);
            return NoContent();
        }
    }
}
