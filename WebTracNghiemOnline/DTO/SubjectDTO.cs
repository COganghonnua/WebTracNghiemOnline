using WebTracNghiemOnline.Models;

namespace WebTracNghiemOnline.DTO
{
    public class SubjectDTO
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public int TopicId { get; set; }
    }
}
