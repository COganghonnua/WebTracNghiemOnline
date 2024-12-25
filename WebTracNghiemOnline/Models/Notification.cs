namespace WebTracNghiemOnline.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public string Content { get; set; } // Nội dung thông báo
        public string UserId { get; set; } // Người nhận thông báo
        public bool IsRead { get; set; } = false; // Đánh dấu đã đọc hay chưa
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Property
        public User User { get; set; }
    }
}
