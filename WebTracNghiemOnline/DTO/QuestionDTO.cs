using WebTracNghiemOnline.Models;

namespace WebTracNghiemOnline.DTO
{
    public class QuestionDTO
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string Explanation { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } // Tên Subject
        public ICollection<AnswerDTO>? Answers { get; set; }
    }

    public class CreateQuestionDto
    {
        public string QuestionText { get; set; }
        public string Explanation { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public int SubjectId { get; set; }
        public ICollection<CreateAnswerDto> Answers { get; set; } // Danh sách đáp án kèm theo
    }

    public class UpdateQuestionDto
    {
        public string QuestionText { get; set; }
        public string Explanation { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public int SubjectId { get; set; }
        public ICollection<UpdateAnswerDto> Answers { get; set; }
    }
}
