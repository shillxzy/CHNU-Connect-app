using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.GroupMember
{
    public class GroupMemberResponseDto
    {
        public string Message { get; set; } = null!;
        public GroupMemberDto? Member { get; set; }
    }
}
