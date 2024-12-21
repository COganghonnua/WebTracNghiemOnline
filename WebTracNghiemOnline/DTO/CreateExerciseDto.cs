namespace WebTracNghiemOnline.DTO
{
    public class CreateExerciseDto
    {
        public string ExerciseName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<CreateExerciseQuestionDto> Questions { get; set; } = new List<CreateExerciseQuestionDto>();
    }

    public class CreateExerciseQuestionDto
    {
        public string QuestionText { get; set; }
        public string Explanation { get; set; } = string.Empty; // Giá trị mặc định
        public List<CreateExerciseAnswerDto> Answers { get; set; }
    }

    public class CreateExerciseAnswerDto
    {
        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class ExerciseDto
    {
        public int ExerciseId { get; set; }
        public string ExerciseName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<ExerciseQuestionDto> Questions { get; set; } = new List<ExerciseQuestionDto>();
    }

    public class ExerciseQuestionDto
    {
        public int ExerciseQuestionId { get; set; }
        public string QuestionText { get; set; }
        public List<ExerciseAnswerDto> Answers { get; set; } = new List<ExerciseAnswerDto>();
    }

    public class ExerciseAnswerDto
    {
        public int ExerciseAnswerId { get; set; }
        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; }
    }
    public class SimpleExerciseDto
    {
        public int ExerciseId { get; set; }
        public string ExerciseName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

}
