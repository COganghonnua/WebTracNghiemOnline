using WebTracNghiemOnline.Models;

namespace WebTracNghiemOnline.DTO
{
    public class QuestionDTO
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string Explanation { get; set; } 
        public int ExamId { get; set; }

        public ICollection<AnswerDTO> Answers { get; set; }
    }

    public class CreateQuestionRequestDto
    {
        public string QuestionText { get; set; }
        public string Explanation { get; set; }
        public int ExamId { get; set; }
    }

    public class UpdateQuestionRequestDto
    {
        public string QuestionText { get; set; }
        public string Explanation { get; set; }
        public int ExamId { get; set; }
    }
}
