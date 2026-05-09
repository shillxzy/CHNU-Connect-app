using CHNU_Connect.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.Schedule
{
    public class ScheduleDto
    {
        public int Id { get; set; }

        public int GroupId { get; set; }

        public int? SubGroupId { get; set; }
        public string? SubGroupName { get; set; }

        public LessonType Type { get; set; }
        public WeekType Week { get; set; }

        public DayOfWeek Day { get; set; }

        public int SlotId { get; set; }
        public int PairNumber { get; set; }

        public string SubjectName { get; set; }
        public string TeacherName { get; set; }
        public string? Location { get; set; }
    }
}
