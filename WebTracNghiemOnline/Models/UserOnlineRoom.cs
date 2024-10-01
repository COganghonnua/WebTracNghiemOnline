namespace WebTracNghiemOnline.Models
{
    public class UserOnlineRoom
    {
        public int UserOnlineRoomId { get; set; }
        public int UserId { get; set; }
        public int OnlineRoomId { get; set; }
        public DateTime JoinedAt { get; set; }

        // Navigation Properties
        public User User { get; set; }
        public OnlineRoom OnlineRoom { get; set; }
    }
}
