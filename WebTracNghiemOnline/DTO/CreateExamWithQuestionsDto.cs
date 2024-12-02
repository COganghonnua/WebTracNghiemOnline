namespace WebTracNghiemOnline.DTO
{
    public class CreateExamWithQuestionsDto
    {
        public string ExamName { get; set; }
        public decimal Fee { get; set; }
        public int SubjectId { get; set; }
        public int Duration { get; set; }

        public NumberOfQuestionsDto NumberOfQuestions { get; set; }
    }
}
