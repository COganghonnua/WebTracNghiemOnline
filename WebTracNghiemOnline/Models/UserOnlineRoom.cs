using System.Text.Json.Serialization;

namespace WebTracNghiemOnline.Models
{
    public class UserOnlineRoom
    {
        public int UserOnlineRoomId { get; set; }
        public string UserId { get; set; }
        public int OnlineRoomId { get; set; }
        public DateTime JoinedAt { get; set; }

        // Vai trò người dùng trong lớp học
        public UserRole Role { get; set; } = UserRole.Member;


        // Thời gian rời khỏi lớp học (nếu có)
        public DateTime? LeftAt { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public User User { get; set; }
        [JsonIgnore]
        public OnlineRoom OnlineRoom { get; set; }
    }
    public enum UserRole
    {
        Owner,      // Chủ phòng
        Member      // Thành viên
    }
}
