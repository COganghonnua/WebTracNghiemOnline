using WebTracNghiemOnline.Models;

namespace WebTracNghiemOnline.DTO
{
    public class ExamDTO
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; }
        public int SubjectId { get; set; }
        public decimal Fee { get; set; }
    }

    public class CreateExamRequestDto
    {
        public string ExamName { get; set; }
        public int SubjectId { get; set; }
        public decimal Fee { get; set; }
    }

    public class UpdateExamRequestDto
    {
        public string ExamName { get; set; }
        public int SubjectId { get; set; }
        public decimal Fee { get; set; }
    }
}
