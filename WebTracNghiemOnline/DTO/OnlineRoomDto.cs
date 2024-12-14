using WebTracNghiemOnline.Models;

namespace WebTracNghiemOnline.DTO
{
    public class OnlineRoomDto
    {
        public int OnlineRoomId { get; set; }
        public string RoomCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public string RoomName { get; set; }
    }
    public class UserOnlineRoomDto
    {
        public int UserOnlineRoomId { get; set; }
        public string UserId { get; set; }
        public int OnlineRoomId { get; set; }
        public DateTime JoinedAt { get; set; }
        public UserRole Role { get; set; } // Sử dụng enum trực tiếp
    }

    public class CreateRoomRequest
    {
        public string RoomName { get; set; }
    }

}
