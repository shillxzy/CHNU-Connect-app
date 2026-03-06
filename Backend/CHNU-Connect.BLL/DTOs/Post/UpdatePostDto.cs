using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.Post
{
    public class UpdatePostDto
    {
        public string Content { get; set; } = null!;
        public string? ImageUrl { get; set; }
    }
}