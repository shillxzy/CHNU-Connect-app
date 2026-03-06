using CHNU_Connect.BLL.DTOs.Chat;
using CHNU_Connect.BLL.DTOs.ChatMember;
using CHNU_Connect.BLL.DTOs.ChatMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.Services.Interfaces
{
    public interface IChatService
    {
        // Чати
        Task<ChatDto?> GetChatByIdAsync(int chatId);
        Task<IEnumerable<ChatDto>> GetUserChatsAsync(int userId);
        Task<ChatDto> CreateChatAsync(CreateChatDto createChatDto);

        // Учасники
        Task<ChatMemberDto> AddMemberAsync(CreateChatMemberDto createMemberDto);
        Task<ChatMemberDto?> GetMemberAsync(int chatId, int userId);

        // Повідомлення
        Task<IEnumerable<ChatMessageDto>> GetMessagesAsync(int chatId, int limit = 50);
        Task<ChatMessageDto> SendMessageAsync(CreateChatMessageDto createMessageDto);
        Task MarkMessageAsReadAsync(int chatId, int userId, int messageId);
    }
}
