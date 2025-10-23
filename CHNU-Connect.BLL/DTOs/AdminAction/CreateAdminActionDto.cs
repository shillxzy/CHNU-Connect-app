using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.AdminAction
{
    public class CreateAdminActionDto
    {
        public int AdminId { get; set; }
        public string ActionType { get; set; } = null!;
        public string? TargetEntity { get; set; }
        public Guid? TargetId { get; set; }
        public string Action { get; set; } = null!;   
        public string? Reason { get; set; }
    }
}
