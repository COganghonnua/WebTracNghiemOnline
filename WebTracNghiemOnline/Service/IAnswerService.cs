using WebTracNghiemOnline.DTO;

namespace WebTracNghiemOnline.Service
{
    public interface IAnswerService
    {
        Task<IEnumerable<AnswerDTO>> GetAllAnswersAsync();
        Task<AnswerDTO?> GetAnswerByIdAsync(int id);
        Task<AnswerDTO> CreateAnswerAsync(CreateAnswerDto createAnswerDto);
        Task UpdateAnswerAsync(int id, UpdateAnswerDto updateAnswerDto);
        Task DeleteAnswerAsync(int id);
    }
}
