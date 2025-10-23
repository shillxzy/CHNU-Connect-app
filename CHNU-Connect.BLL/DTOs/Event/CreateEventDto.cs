using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.Event
{
    public class CreateEventDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Guid CreatedById { get; set; }
    }
}
