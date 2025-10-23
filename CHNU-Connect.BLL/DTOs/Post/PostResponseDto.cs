using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.Post
{
    public class PostResponseDto
    {
        public string Message { get; set; } = null!;
        public PostDto? Post { get; set; }
    }
}
