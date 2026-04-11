using CHNU_Connect.DAL.Entities;
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

        public LessonType Type { get; set; }
        public WeekType Week { get; set; }

        public DayOfWeek Day { get; set; }

        public int SlotId { get; set; }

        public string? Location { get; set; }
    }

}
