namespace WebTracNghiemOnline.Models
{
    public class Exam
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; } // Tên đề thi
        public int SubjectId { get; set; }
        public Subject Subject { get; set; } // Môn học của đề thi
        public decimal Fee { get; set; } // Phí tham gia đề thi
        public int Duration { get; set; }
        // Quan hệ
        public ICollection<ExamQuestion> ? ExamQuestions { get; set; } = new List<ExamQuestion>();
        public ICollection<ExamHistory> ?  ExamHistories { get; set; } = new List<ExamHistory>();
        
    }
}
