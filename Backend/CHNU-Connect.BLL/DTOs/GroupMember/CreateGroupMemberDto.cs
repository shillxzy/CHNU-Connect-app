using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.GroupMember
{
    public class CreateGroupMemberDto
    {
        public Guid GroupId { get; set; }
        public Guid UserId { get; set; }
    }
}
