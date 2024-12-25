using WebTracNghiemOnline.Models;
using WebTracNghiemOnline.Repository;

namespace WebTracNghiemOnline.Service
{
    public interface INotificationService
    {
        Task SendNotificationToClassAsync(int roomId, string content);
        Task<List<Notification>> GetUserNotificationsAsync(string userId);
        Task MarkNotificationAsReadAsync(int notificationId);
    }
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;
        private readonly IOnlineRoomRepository _roomRepository;

        public NotificationService(INotificationRepository repository, IOnlineRoomRepository roomRepository)
        {
            _repository = repository;
            _roomRepository = roomRepository;
        }

        public async Task SendNotificationToClassAsync(int roomId, string content)
        {
            // Lấy danh sách thành viên trong phòng
            var users = await _roomRepository.GetUsersInRoomAsync(roomId);

            if (users == null || !users.Any())
            {
                throw new ArgumentException("Room does not exist or has no members.");
            }

            // Log số lượng thành viên
            Console.WriteLine($"Số lượng thành viên trong phòng {roomId}: {users.Count}");

            // Tạo danh sách thông báo
            var notifications = users.Select(user => new Notification
            {
                UserId = user.UserId,
                Content = content,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            // Lưu tất cả thông báo cùng lúc
            await _repository.AddNotificationsAsync(notifications);
        }



        public async Task<List<Notification>> GetUserNotificationsAsync(string userId)
        {
            return await _repository.GetNotificationsForUserAsync(userId);
        }

        public async Task MarkNotificationAsReadAsync(int notificationId)
        {
            await _repository.MarkNotificationAsReadAsync(notificationId);
        }
    }
}
