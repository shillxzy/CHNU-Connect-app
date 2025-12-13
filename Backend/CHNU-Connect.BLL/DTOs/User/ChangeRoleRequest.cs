using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.User
{
    public class ChangeRoleRequest
    {
        public int UserId { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}
