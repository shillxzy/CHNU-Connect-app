using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.Group
{
    public class CreateGroupDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public Guid CreatorId { get; set; }
        public int CreatedById { get; set; }
    }
}
