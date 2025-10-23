using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.PostLike
{
    public class PostLikeDto
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
    }
}
