using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.Event
{
    public class EventResponseDto
    {
        public string Message { get; set; } = null!;
        public EventDto? Event { get; set; }
    }
}
