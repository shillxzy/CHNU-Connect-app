using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.User
{
    public class UpdateProfileDto
    {
        public string? FullName { get; set; }
        public string? Faculty { get; set; }
        public int? Course { get; set; }
        public string? Bio { get; set; }
        public string? PhotoUrl { get; set; }
    }
}
