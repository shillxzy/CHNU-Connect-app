using CHNU_Connect.BLL.DTOs.Chat;
using CHNU_Connect.BLL.DTOs.ChatMember;
using CHNU_Connect.BLL.DTOs.ChatMessage;
using CHNU_Connect.BLL.Services.Interfaces;
using CHNU_Connect.DAL.UOW;
using Mapster;


namespace CHNU_Connect.BLL.Services
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChatService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // -------------------- Чати --------------------
        public async Task<ChatDto?> GetChatByIdAsync(int chatId)
        {
            var chat = await _unitOfWork.ChatRepository.GetChatWithMembersAsync(chatId);
            return chat?.Adapt<ChatDto>();
        }

        public async Task<IEnumerable<ChatDto>> GetUserChatsAsync(int userId)
        {
            var chats = await _unitOfWork.ChatRepository.GetUserChatsAsync(userId);
            return chats.Adapt<IEnumerable<ChatDto>>();
        }

        public async Task<ChatDto> CreateChatAsync(CreateChatDto createChatDto)
        {
            var chat = createChatDto.Adapt<CHNU_Connect.DAL.Entities.Chat>();
            await _unitOfWork.ChatRepository.InsertAsync(chat);
            await _unitOfWork.SaveChangesAsync();
            return chat.Adapt<ChatDto>();
        }

        // -------------------- Учасники --------------------
        public async Task<ChatMemberDto> AddMemberAsync(CreateChatMemberDto createMemberDto)
        {
            var member = createMemberDto.Adapt<CHNU_Connect.DAL.Entities.ChatMember>();
            await _unitOfWork.ChatMemberRepository.InsertAsync(member);
            await _unitOfWork.SaveChangesAsync();
            return member.Adapt<ChatMemberDto>();
        }

        public async Task<ChatMemberDto?> GetMemberAsync(int chatId, int userId)
        {
            var member = await _unitOfWork.ChatMemberRepository.GetMemberAsync(chatId, userId);
            return member?.Adapt<ChatMemberDto>();
        }

        // -------------------- Повідомлення --------------------
        public async Task<IEnumerable<ChatMessageDto>> GetMessagesAsync(int chatId, int limit = 50)
        {
            var messages = await _unitOfWork.ChatMessageRepository.GetMessagesByChatIdAsync(chatId, limit);
            return messages.Adapt<IEnumerable<ChatMessageDto>>();
        }

        public async Task<ChatMessageDto> SendMessageAsync(CreateChatMessageDto createMessageDto)
        {
            var message = createMessageDto.Adapt<CHNU_Connect.DAL.Entities.ChatMessage>();
            await _unitOfWork.ChatMessageRepository.InsertAsync(message);
            await _unitOfWork.SaveChangesAsync();
            return message.Adapt<ChatMessageDto>();
        }

        public async Task MarkMessageAsReadAsync(int chatId, int userId, int messageId)
        {
            var member = await _unitOfWork.ChatMemberRepository.GetMemberAsync(chatId, userId);
            if (member != null)
            {
                member.LastReadMessageId = messageId;
                _unitOfWork.ChatMemberRepository.Update(member);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
