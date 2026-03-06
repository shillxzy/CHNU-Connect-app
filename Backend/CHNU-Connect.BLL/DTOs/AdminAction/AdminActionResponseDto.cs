using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.AdminAction
{
    public class AdminActionResponseDto
    {
        public string Message { get; set; } = null!;
        public AdminActionDto? Action { get; set; }
    }
}
