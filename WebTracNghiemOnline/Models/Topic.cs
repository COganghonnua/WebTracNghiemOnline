using System.ComponentModel.DataAnnotations;

namespace WebTracNghiemOnline.Models
{
    public class Topic
    {
        public int TopicId { get; set; }

        [Required]
        [StringLength(100)]
        public string TopicName { get; set; } = string.Empty;

        public ICollection<Subject>? Subjects { get; set; }
    }
}
