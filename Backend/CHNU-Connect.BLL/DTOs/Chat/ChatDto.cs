using CHNU_Connect.BLL.DTOs.ChatMember;
using CHNU_Connect.BLL.DTOs.ChatMessage;


namespace CHNU_Connect.BLL.DTOs.Chat
{
    public class ChatDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = null!;
        public string? Title { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public IEnumerable<ChatMemberDto>? Members { get; set; }
        public IEnumerable<ChatMessageDto>? Messages { get; set; }
    }
}
