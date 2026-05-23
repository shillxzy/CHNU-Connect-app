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
            // 1. Беремо чати користувача з репозиторію
            var chats = await _unitOfWork.ChatRepository.GetUserChatsAsync(userId);

            // 2. Перетворюємо їх у DTO з підставленими User (AuthorName, AuthorAvatar)
            var chatDtos = chats.Select(c =>
            {
                var lastMsg = c.Messages?
                    .OrderByDescending(m => m.CreatedAt)
                    .FirstOrDefault();

                return new ChatDto
                {
                    Id = c.Id,
                    Type = c.Type,
                    Title = c.Title,
                    CreatedBy = c.CreatedBy,
                    CreatedAt = c.CreatedAt,
                    Members = c.Members
                        .GroupBy(m => m.UserId)
                        .Select(g => g.First())
                        .Select(m => new ChatMemberDto
                        {
                            Id = m.Id,
                            ChatId = m.ChatId,
                            UserId = m.UserId,
                            Role = m.Role,
                            JoinedAt = m.JoinedAt,
                            LastReadMessageId = m.LastReadMessageId,
                            AuthorName = m.User.FullName,
                            AuthorAvatar = m.User.PhotoUrl
                        }).ToList(),
                    Messages = c.Messages?.Adapt<List<ChatMessageDto>>(),
                    LastMessage = lastMsg?.Content,
                    LastMessageAt = lastMsg?.CreatedAt
                };
            })
            .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
            .ToList();


            // 3. Повертаємо DTO на фронт
            return chatDtos;
        }


        public async Task<ChatDto> CreateChatAsync(CreateChatDto dto)
        {
            // -------------------- PRIVATE CHAT --------------------
            if (dto.Type == "private")
            {
                if (dto.MemberIds == null || dto.MemberIds.Count != 2)
                    throw new InvalidOperationException("Private chat must have exactly 2 members");

                var userA = dto.MemberIds[0];
                var userB = dto.MemberIds[1];

                var directKey = $"{Math.Min(userA, userB)}_{Math.Max(userA, userB)}";

                // 1️⃣ шукаємо існуючий чат
                var existingChat = await _unitOfWork.ChatRepository.GetByDirectKeyAsync(directKey);

                if (existingChat != null)
                {
                    // Підвантажуємо Members з User, щоб Mapster міг заповнити AuthorName/AuthorAvatar
                    existingChat.Members = await _unitOfWork.ChatMemberRepository
                        .GetMembersByChatIdAsync(existingChat.Id);

                    return existingChat.Adapt<ChatDto>();
                }

                // 2️⃣ створюємо новий чат
                var chat = new CHNU_Connect.DAL.Entities.Chat
                {
                    Type = "private",
                    DirectKey = directKey,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = dto.CreatedBy
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

                // 4️⃣ підвантажуємо Members з User для DTO
                chat.Members = await _unitOfWork.ChatMemberRepository
                    .GetMembersByChatIdAsync(chat.Id);

                return chat.Adapt<ChatDto>();
            }

            // -------------------- GROUP CHAT --------------------
            if (dto.MemberIds == null || dto.MemberIds.Count == 0)
                throw new InvalidOperationException("Group chat must have at least one member");

            var groupChat = new CHNU_Connect.DAL.Entities.Chat
            {
                Type = "group",
                Title = dto.Title,
                CreatedBy = dto.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ChatRepository.InsertAsync(groupChat);
            await _unitOfWork.SaveChangesAsync();

            foreach (var uid in dto.MemberIds)
            {
                await _unitOfWork.ChatMemberRepository.InsertAsync(
                    new CHNU_Connect.DAL.Entities.ChatMember
                    {
                        ChatId = groupChat.Id,
                        UserId = uid
                    });
            }

            await _unitOfWork.SaveChangesAsync();

            groupChat.Members = await _unitOfWork.ChatMemberRepository
                .GetMembersByChatIdAsync(groupChat.Id);

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

        public async Task<ChatMessageDto?> UpdateMessageAsync(int messageId, int userId, string content)
        {
            var message = await _unitOfWork.ChatMessageRepository.GetByIdAsync(messageId);

            if (message == null)
                return null;

            if (message.SenderId != userId)
                return null;

            message.Content = content;
            message.EditedAt = DateTime.UtcNow;

            _unitOfWork.ChatMessageRepository.Update(message);
            await _unitOfWork.SaveChangesAsync();

            return message.Adapt<ChatMessageDto>();
        }


        public async Task DeleteMessageAsync(int messageId, int userId)
        {
            var message = await _unitOfWork.ChatMessageRepository.GetByIdAsync(messageId);

            if (message == null)
                return;

            if (message.SenderId != userId)
                throw new UnauthorizedAccessException();

            _unitOfWork.ChatMessageRepository.Delete(message); 
            await _unitOfWork.SaveChangesAsync();
        }




    }
}
