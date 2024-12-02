namespace WebTracNghiemOnline.DTO
{
    public class UserAnswerDto
    {
        public int QuestionId { get; set; }
        // ID của các câu trả lời người dùng chọn
        public List<int> AnswerIds { get; set; }
    }
}
