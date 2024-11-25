using WebTracNghiemOnline.Models;

namespace WebTracNghiemOnline.DTO
{
    public class ExamDTO
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; }
        public decimal Fee { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } // Hiển thị tên môn học thay vì chỉ SubjectId
    }

    public class CreateExamDto
    {
        public string ExamName { get; set; } = string.Empty;
        public decimal Fee { get; set; }
        public int SubjectId { get; set; }
    }


    public class UpdateExamDto
    {
        public string ExamName { get; set; } = string.Empty;
        public decimal Fee { get; set; }
        public int SubjectId { get; set; }
    }
}
