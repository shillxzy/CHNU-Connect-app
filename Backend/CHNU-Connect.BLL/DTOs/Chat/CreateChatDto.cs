using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.Chat
{
    public class CreateChatDto
    {
        public string Type { get; set; } = null!; 
        public string? Title { get; set; }
        public int CreatedBy { get; set; }
    }
}
