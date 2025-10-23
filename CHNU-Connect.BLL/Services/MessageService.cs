using CHNU_Connect.BLL.Services.Interfaces;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Mapster;

namespace CHNU_Connect.BLL.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;

        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<object> CreateMessageAsync(int senderId, int receiverId, string content)
        {
            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                IsRead = false,
                SentAt = DateTime.UtcNow
            };

            var createdMessage = await _messageRepository.AddAsync(message);
            await _messageRepository.SaveChangesAsync();
            return createdMessage;
        }

        public async Task<object?> GetByIdAsync(int id)
        {
            var message = await _messageRepository.GetByIdAsync(id);
            return message;
        }

        public async Task<IEnumerable<object>> GetConversationAsync(int userId1, int userId2)
        {
            var messages = await _messageRepository.GetAllAsync();
            var conversation = messages.Where(m => 
                (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                (m.SenderId == userId2 && m.ReceiverId == userId1))
                .OrderBy(m => m.SentAt);
            return conversation;
        }

        public async Task<IEnumerable<object>> GetUserMessagesAsync(int userId)
        {
            var messages = await _messageRepository.GetAllAsync();
            var userMessages = messages.Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .OrderByDescending(m => m.SentAt);
            return userMessages;
        }

        public async Task<IEnumerable<object>> GetUnreadMessagesAsync(int userId)
        {
            var messages = await _messageRepository.GetAllAsync();
            var unreadMessages = messages.Where(m => m.ReceiverId == userId && !m.IsRead)
                .OrderByDescending(m => m.SentAt);
            return unreadMessages;
        }

        public async Task<bool> MarkAsReadAsync(int messageId)
        {
            var message = await _messageRepository.GetByIdAsync(messageId);
            if (message == null)
                return false;

            message.IsRead = true;
            await _messageRepository.UpdateAsync(message);
            await _messageRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMessageAsync(int id)
        {
            var message = await _messageRepository.GetByIdAsync(id);
            if (message == null)
                return false;

            await _messageRepository.DeleteAsync(message);
            await _messageRepository.SaveChangesAsync();
            return true;
        }
    }
}
