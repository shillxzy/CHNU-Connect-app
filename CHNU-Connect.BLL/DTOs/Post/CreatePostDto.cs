using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.Post
{
    public class CreatePostDto
    {
        public int AuthorId { get; set; }
        public string Content { get; set; } = null!;
        public Guid? GroupId { get; set; }
        public string? ImageUrl { get; set; }
    }
}
