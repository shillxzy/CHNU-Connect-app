using CHNU_Connect.DAL.Data;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CHNU_Connect.DAL.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task InsertAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

		public async Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(int userId)
		{
			return await _context.Notifications
				.Include(n => n.Actor)
				.Where(n => n.UserId == userId && !n.IsRead)
				.OrderByDescending(n => n.CreatedAt)
				.ToListAsync();
		}

		public async Task<IEnumerable<Notification>> GetAllByUserIdAsync(int userId)
		{
			return await _context.Notifications
				.Include(n => n.Actor)
				.Where(n => n.UserId == userId)
				.OrderByDescending(n => n.CreatedAt)
				.Take(50)
				.ToListAsync();
		}

		public async Task MarkAsReadAsync(int notificationId)
        {
            var n = await _context.Notifications.FirstOrDefaultAsync(x => x.Id == notificationId);
            if (n != null)
            {
                n.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(int userId)
        {
            var list = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var n in list) n.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }
}
