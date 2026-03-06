using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.Comment
{
    public class CommentResponseDto
    {
        public string Message { get; set; } = null!;
        public CommentDto? Comment { get; set; }
    }
}
