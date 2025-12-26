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

        public async Task<ChatDto> CreateChatAsync(CreateChatDto dto)
        {
            // PRIVATE CHAT
            if (dto.Type == "private")
            {
                if (dto.MemberIds == null || dto.MemberIds.Count != 2)
                    throw new InvalidOperationException("Private chat must have exactly 2 members");

                var userA = dto.MemberIds[0];
                var userB = dto.MemberIds[1];

                var directKey = $"{Math.Min(userA, userB)}_{Math.Max(userA, userB)}";

                // 1️⃣ шукаємо існуючий чат
                var existingChat = await _unitOfWork.ChatRepository
                    .GetByDirectKeyAsync(directKey);

                if (existingChat != null)
                    return existingChat.Adapt<ChatDto>();

                // 2️⃣ створюємо новий
                var chat = new CHNU_Connect.DAL.Entities.Chat
                {
                    Type = "private",
                    DirectKey = directKey,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.ChatRepository.InsertAsync(chat);
                await _unitOfWork.SaveChangesAsync();

                // 3️⃣ додаємо учасників
                foreach (var userId in dto.MemberIds)
                {
                    await _unitOfWork.ChatMemberRepository.InsertAsync(
                        new CHNU_Connect.DAL.Entities.ChatMember
                        {
                            ChatId = chat.Id,
                            UserId = userId
                        });
                }

                await _unitOfWork.SaveChangesAsync();
                return chat.Adapt<ChatDto>();
            }

            // GROUP CHAT
            var groupChat = dto.Adapt<CHNU_Connect.DAL.Entities.Chat>();
            await _unitOfWork.ChatRepository.InsertAsync(groupChat);
            await _unitOfWork.SaveChangesAsync();

            return groupChat.Adapt<ChatDto>();
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

        public async Task<ChatMessageDto> SendMessageAsync(CreateChatMessageDto dto)
        {
            var message = dto.Adapt<CHNU_Connect.DAL.Entities.ChatMessage>();
            await _unitOfWork.ChatMessageRepository.InsertAsync(message);
            await _unitOfWork.SaveChangesAsync();

            var members = await _unitOfWork.ChatMemberRepository
                .GetMembersByChatIdAsync(dto.ChatId);

            foreach (var member in members.Where(m => m.UserId != dto.SenderId))
            {
                await _unitOfWork.NotificationRepository.InsertAsync(
                    new CHNU_Connect.DAL.Entities.Notification
                    {
                        UserId = member.UserId,
                        Type = "message",
                        EntityId = message.Id
                    });
            }

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
