namespace WebTracNghiemOnline.DTO
{
    public class UserAnswerDto
    {
        public int QuestionId { get; set; }
        // ID của các câu trả lời người dùng chọn
        public List<int> AnswerIds { get; set; }
    }
    public class GradeResultDto
    {
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public double Score { get; set; }
    }
}
