using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.Group
{
    public class GroupResponseDto
    {
        public string Message { get; set; } = null!;
        public GroupDto? Group { get; set; }
    }
}
