namespace WebTracNghiemOnline.Models
{
    public class Topic
    {
        public int TopicId { get; set; }
        public string TopicName { get; set; } // Tương đương với chủ đề trong phần "Luyện thi" web onthitracnghiem

        // Quan hệ
        public ICollection<Subject> Subjects { get; set; } // Các môn học trong chủ đề
    }
}
