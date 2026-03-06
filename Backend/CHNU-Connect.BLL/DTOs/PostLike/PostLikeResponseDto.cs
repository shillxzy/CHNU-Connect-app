using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.PostLike
{
    public class PostLikeResponseDto
    {
        public string Message { get; set; } = null!;
        public PostLikeDto? Like { get; set; }
    }
}
