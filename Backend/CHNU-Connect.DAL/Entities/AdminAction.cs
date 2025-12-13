using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.DAL.Entities
{
    public class AdminAction
    {
        public int Id { get; set; }
        public int AdminId { get; set; }
        public User? Admin { get; set; }
        public int TargetUserId { get; set; }
        public User? TargetUser { get; set; }
        public string Action { get; set; } = null!; // block, unblock, delete_post...
        public string? Reason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
