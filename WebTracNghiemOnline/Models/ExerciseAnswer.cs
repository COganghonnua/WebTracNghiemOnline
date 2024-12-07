namespace WebTracNghiemOnline.Models
{
    public class ExerciseAnswer
    {
        public int ExerciseAnswerId { get; set; }
        public string AnswerText { get; set; } // Nội dung đáp án
        public bool IsCorrect { get; set; } // Có đúng không
        public int ExerciseQuestionId { get; set; }
        public ExerciseQuestion  ExerciseQuestion { get; set; } // Câu hỏi liên quan
    }
}
