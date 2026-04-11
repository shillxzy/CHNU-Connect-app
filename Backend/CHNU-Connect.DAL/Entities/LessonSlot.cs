using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.DAL.Entities
{
    public class LessonSlot
    {
        public int Id { get; set; }

        public int PairNumber { get; set; } // 1,2,3...
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
