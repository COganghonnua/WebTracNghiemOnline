using WebTracNghiemOnline.Models;

namespace WebTracNghiemOnline.DTO
{
    public class TopicDTO
    {
        public int TopicId { get; set; }
        public string TopicName { get; set; } = string.Empty;
        public ICollection<SubjectDTO> ListSubjectDTO { get; set; }

    }
    public class CreateTopicDto
    {
        public string TopicName { get; set; } = string.Empty;
    }

    public class UpdateTopicDto
    {
        public string TopicName { get; set; } = string.Empty;
    }
}
