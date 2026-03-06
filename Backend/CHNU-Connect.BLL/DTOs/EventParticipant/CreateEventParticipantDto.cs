using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.EventParticipant
{
    public class CreateEventParticipantDto
    {
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
    }
}
