using WebTracNghiemOnline.DTO;

namespace WebTracNghiemOnline.Service
{
    public interface IExamService
    {
        Task<IEnumerable<ExamDTO>> GetAllExamsAsync();
        Task<ExamDTO?> GetExamByIdAsync(int id);
        Task<ExamDTO> CreateExamAsync(CreateExamDto createExamDto);
        Task<ExamDTO> UpdateExamAsync(int id, UpdateExamDto updateExamDto);
        Task DeleteExamAsync(int id);
    }
}
