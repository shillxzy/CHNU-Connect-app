using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.User
{
    public class UserResponseDto
    {
        public string Message { get; set; } = null!;
        public UserDto? User { get; set; }
    }
}
