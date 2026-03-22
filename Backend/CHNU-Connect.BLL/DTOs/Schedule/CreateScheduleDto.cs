using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.Schedule
{
    public class CreateScheduleDto
    {
        public int GroupId { get; set; }
        public int SubjectId { get; set; }

        public int? SubGroupId { get; set; }

        public DayOfWeek Day { get; set; }

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public string? Location { get; set; }
    }
}
