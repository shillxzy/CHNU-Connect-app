using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.DAL.Entities
{
    public class SubGroup
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        // Наприклад: 144(1)


        public int GroupId { get; set; }
        public Group? Group { get; set; }


        public ICollection<Schedule>? Schedules { get; set; }
        public ICollection<User>? Users { get; set; }
    }

}
