namespace WebTracNghiemOnline.Models
{
    public class Exam
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; } // Tên đề thi
        public int SubjectId { get; set; }
        public Subject Subject { get; set; } // Môn học của đề thi
        public decimal Fee { get; set; } // Phí tham gia đề thi

        // Quan hệ
        public ICollection<Question> Questions { get; set; } // Các câu hỏi trong đề thi
        public ICollection<ExamHistory> ExamHistories { get; set; } // Lịch sử thi của người dùng
    }
}
