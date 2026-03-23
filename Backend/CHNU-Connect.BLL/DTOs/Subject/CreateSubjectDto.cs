using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.Subject
{
    public class CreateSubjectDto
    {
        public string Name { get; set; } = null!;
        public int GroupId { get; set; }
        public int? TeacherId { get; set; }
        public string? MoodleLink { get; set; }
    }
}
