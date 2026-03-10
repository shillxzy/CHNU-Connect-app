using CHNU_Connect.BLL.DTOs.Notification;
using CHNU_Connect.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.Services.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task MarkNotificationAsReadAsync(int notificationId);
    }
}
