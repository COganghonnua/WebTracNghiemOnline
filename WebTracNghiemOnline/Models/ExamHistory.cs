namespace WebTracNghiemOnline.Models
{
    public class ExamHistory
    {
        public int ExamHistoryId { get; set; }
        public string UserId { get; set; }
        public User User { get; set; } // Người dùng thực hiện bài thi
        public int ExamId { get; set; }
        public Exam Exam { get; set; } // Đề thi mà người dùng tham gia
        public DateTime ExamDate { get; set; } // Ngày thi
        public int Score { get; set; } // Điểm số người dùng đạt được
        public TimeSpan Duration { get; set; } // Thời gian hoàn thành bài thi

        public ICollection<ExamHistoryAnswer> ExamHistoryAnswers { get; set; } = new List<ExamHistoryAnswer>();
    }

}
