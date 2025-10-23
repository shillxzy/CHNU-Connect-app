using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.AdminAction
{
    public class CreateAdminActionDto
    {
        public Guid AdminId { get; set; }
        public string ActionType { get; set; } = null!;
        public string? TargetEntity { get; set; }
        public Guid? TargetId { get; set; }
    }
}
