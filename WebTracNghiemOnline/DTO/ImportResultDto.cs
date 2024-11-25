namespace WebTracNghiemOnline.DTO
{
    public class ImportResultDto
    {
        public List<ImportSuccessDto> Success { get; set; } = new();
        public List<ImportErrorDto> Errors { get; set; } = new();
    }

    public class ImportSuccessDto
    {
        public int Row { get; set; }
        public string QuestionText { get; set; }
    }

    public class ImportErrorDto
    {
        public int Row { get; set; }
        public string Error { get; set; }
    }
}
