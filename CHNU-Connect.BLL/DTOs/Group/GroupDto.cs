using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.Group
{
    public class GroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int CreatorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedById { get; set; }
    }
}
