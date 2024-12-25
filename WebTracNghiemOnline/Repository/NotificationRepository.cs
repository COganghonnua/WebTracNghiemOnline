using Microsoft.EntityFrameworkCore;
using WebTracNghiemOnline.Access;
using WebTracNghiemOnline.Models;

namespace WebTracNghiemOnline.Repository
{
    public interface INotificationRepository
    {
        Task AddNotificationAsync(Notification notification);
        Task AddNotificationsAsync(List<Notification> notifications); // Mới thêm
        Task<List<Notification>> GetNotificationsForUserAsync(string userId);
        Task MarkNotificationAsReadAsync(int notificationId);
    }

    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task AddNotificationsAsync(List<Notification> notifications)
        {
            _context.Notifications.AddRange(notifications); // Thêm nhiều thông báo cùng lúc
            await _context.SaveChangesAsync(); // Lưu thay đổi
        }

        public async Task<List<Notification>> GetNotificationsForUserAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkNotificationAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }
    }

}
