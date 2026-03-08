using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.Comment
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public int PostId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        public string AuthorName { get; set; } = null!;
        public string? AuthorAvatar { get; set; }
    }
}
