namespace WebTracNghiemOnline.Models
{
    public class Question
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } // Nội dung câu hỏi
        public string Explanation { get; set; } // Giải thích khi người dùng trả lời sai
        public DifficultyLevel Difficulty { get; set; } //đánh giá độ khó câu hỏi
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
        public ICollection<Answer> ? Answers { get; set; } // Các đáp án cho câu hỏi này
        public ICollection<ExamQuestion> ? ExamQuestions { get; set; }
    }
}
