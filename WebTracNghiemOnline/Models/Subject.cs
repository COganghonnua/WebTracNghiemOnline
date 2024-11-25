using System.ComponentModel.DataAnnotations;

namespace WebTracNghiemOnline.Models
{
    public class Subject
    {
        public int SubjectId { get; set; }

        [Required]
        [StringLength(100)]
        public string SubjectName { get; set; } = string.Empty;

        public int TopicId { get; set; }
        public Topic? Topic { get; set; }
        public ICollection<Exam>? Exams { get; set; }
        public ICollection<Question>? Questions { get; set; }
    }
}
