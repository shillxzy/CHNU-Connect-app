using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.PostLike
{
    public class CreatePostLikeDto
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
    }
}
