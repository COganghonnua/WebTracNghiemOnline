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
        public int Duration { get; set; }
    }

    public class CreateExamDto
    {
        public string ExamName { get; set; } = string.Empty;
        public decimal Fee { get; set; }
        public int SubjectId { get; set; }
        public int Duration { get; set; }
    }


    public class UpdateExamDto
    {
        public string ExamName { get; set; } = string.Empty;
        public decimal Fee { get; set; }
        public int SubjectId { get; set; }
        public int Duration { get; set; }
    }
    public class ExamWithQuestionsDto
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; }
        public decimal Fee { get; set; }
        public string SubjectName { get; set; } // Tên Subject
        public int Duration { get; set; }

        public List<QuestionDTO> Questions { get; set; } = new();
    }
    public class SubmitExamDto
    {
        public int ExamId { get; set; }
        public Dictionary<int, List<int>> UserAnswers { get; set; } // Câu hỏi và danh sách các đáp án được chọn
        public TimeSpan TimeTaken { get; set; } // Thời gian hoàn thành bài thi
    }
    public class ExamSubmissionDto
    {
        public int ExamId { get; set; }
        public List<UserAnswerDto> Answers { get; set; }
    }

}
