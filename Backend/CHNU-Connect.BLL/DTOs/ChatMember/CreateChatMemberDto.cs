using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.ChatMember
{
    public class CreateChatMemberDto
    {
        public int ChatId { get; set; }
        public int UserId { get; set; }
        public string Role { get; set; } = "member";
    }
}
