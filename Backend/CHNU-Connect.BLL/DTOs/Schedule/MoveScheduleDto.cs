using CHNU_Connect.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.Schedule
{
    public class MoveScheduleDto
    {
        public int ScheduleId { get; set; }

        public DayOfWeek NewDay { get; set; }
        public int NewSlotId { get; set; }
        public WeekType NewWeek { get; set; }
    }

}
