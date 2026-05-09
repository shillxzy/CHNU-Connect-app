using CHNU_Connect.DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.Schedule
{
    public class CreateScheduleDto
    {
        public int GroupId { get; set; }
        public int? SubGroupId { get; set; }

        public LessonType Type { get; set; }
        public WeekType Week { get; set; }

        public DayOfWeek Day { get; set; }

        public int SlotId { get; set; }

        public string SubjectName { get; set; }
        public string TeacherName { get; set; }
        public string? Location { get; set; }
    }

}
