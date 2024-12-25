using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebTracNghiemOnline.Models
{
    public class Post
    {
        public int PostId { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;


        public string UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }

        public int OnlineRoomId { get; set; }
        [JsonIgnore]
        public OnlineRoom OnlineRoom { get; set; }

        public ICollection<Comment> Comments { get; set; }
        public ICollection<FileAttachment> FileAttachments { get; set; }
    }
}
