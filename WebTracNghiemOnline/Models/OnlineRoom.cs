using System.ComponentModel.DataAnnotations;

namespace WebTracNghiemOnline.Models
{
    public class OnlineRoom
    {
        public int OnlineRoomId { get; set; }

        [Required]
        public string RoomCode { get; set; }

        public DateTime CreatedAt { get; set; }

        public int HostUserId { get; set; }

        [Required]
        public User HostUser { get; set; }
        public int ExamId { get; set; }
        public Exam Exam { get; set; }

        public ICollection<UserOnlineRoom> UserOnlineRooms { get; set; }
    }


}
