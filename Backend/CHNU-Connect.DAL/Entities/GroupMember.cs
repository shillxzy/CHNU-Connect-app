using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.DAL.Entities
{
    public class GroupMember
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public Group? Group { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public string Role { get; set; } 
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
