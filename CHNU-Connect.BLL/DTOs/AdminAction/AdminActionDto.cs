using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.AdminAction
{
    public class AdminActionDto
    {
        public Guid Id { get; set; }
        public Guid AdminId { get; set; }
        public string ActionType { get; set; } = null!;
        public string? TargetEntity { get; set; }
        public Guid? TargetId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
