namespace WebTracNghiemOnline.Models
{
    public class ExamHistoryAnswer
    {
        public int ExamHistoryAnswerId { get; set; }
        public int ExamHistoryId { get; set; }
        public ExamHistory ExamHistory { get; set; }
        public int QuestionId { get; set; }
        public string SelectedAnswerIds { get; set; } // Chuỗi chứa ID đáp án (VD: "1,2,3")
        public bool IsCorrect { get; set; } // Đáp án đúng/sai
    }
}
