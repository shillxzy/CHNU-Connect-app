using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.DTOs.Notification
{
	public class NotificationDto
	{
		public int Id { get; set; }
		public string Type { get; set; } = null!;
		public int? EntityId { get; set; }
		public bool IsRead { get; set; }
		public DateTime CreatedAt { get; set; }
		public int? ActorId { get; set; }
		public string? ActorName { get; set; }
		public string? ActorAvatar { get; set; }
	}

}
