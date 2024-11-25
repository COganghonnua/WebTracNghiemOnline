using WebTracNghiemOnline.Models;

namespace WebTracNghiemOnline.DTO
{
    public class AnswerDTO
    {
        public int AnswerId { get; set; }
        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class CreateAnswerDto
    {
        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class UpdateAnswerDto
    {
        public int AnswerId { get; set; }
        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; }
    }
}
