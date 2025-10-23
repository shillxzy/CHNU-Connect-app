using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.EventParticipant
{
    public class EventParticipantResponseDto
    {
        public string Message { get; set; } = null!;
        public EventParticipantDto? Participant { get; set; }
    }
}
