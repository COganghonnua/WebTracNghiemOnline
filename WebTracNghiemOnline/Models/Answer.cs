namespace WebTracNghiemOnline.Models
{
    public class Answer
    {
        public int AnswerId { get; set; }
        public string AnswerText { get; set; } // Nội dung đáp án
        public bool IsCorrect { get; set; } // Đánh dấu nếu đây là đáp án đúng
        public int QuestionId { get; set; }
        public Question Question { get; set; } // Câu hỏi mà đáp án này thuộc về
    }
}
