namespace WebTracNghiemOnline.Models
{
    public class Subject
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } // Tên môn học

        public int TopicId { get; set; }
        public Topic Topic { get; set; } 
        public ICollection<Exam> Exams { get; set; } 
    }
}
