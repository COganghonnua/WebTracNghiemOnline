using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebTracNghiemOnline.Models
{
    public class OnlineRoom
    {
        public int OnlineRoomId { get; set; }
        [Required]
        public string RoomCode { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public ICollection<Exercise> ? Exercises { get; set; } // Danh sách bài tập
        [JsonIgnore]
        public ICollection<UserOnlineRoom> ? UserOnlineRooms { get; set; }
        public ICollection<Post> Posts { get; set; }

    }


}
 