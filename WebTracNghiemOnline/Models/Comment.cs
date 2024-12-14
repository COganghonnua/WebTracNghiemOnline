using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebTracNghiemOnline.Models
{
    public class Comment
    {
        public int CommentId { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;


        public string UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }

        public int PostId { get; set; }
        [JsonIgnore]
        public Post Post { get; set; }
    }
}
