using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.ChatMessage
{
    public class CreateChatMessageDto
    {
        public int ChatId { get; set; }
        public int SenderId { get; set; }
        public string Content { get; set; } = null!;
        public string MessageType { get; set; } = "text";
    }
}
