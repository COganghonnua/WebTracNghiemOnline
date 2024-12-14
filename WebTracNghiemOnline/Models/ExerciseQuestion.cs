namespace WebTracNghiemOnline.Models
{
    public class ExerciseQuestion
    {
        public int ExerciseQuestionId { get; set; }
        public string QuestionText { get; set; } // Nội dung câu hỏi
        public string ? Explanation { get; set; } // Giải thích nếu sai
        public int ExerciseId { get; set; } // Liên kết với bài tập
        public Exercise  Exercise { get; set; } // Điều hướng bài tập
        public ICollection<ExerciseAnswer> ? ExerciseAnswers { get; set; }
    }
}
