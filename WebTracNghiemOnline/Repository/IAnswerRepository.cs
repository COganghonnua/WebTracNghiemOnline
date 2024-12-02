using WebTracNghiemOnline.Models;

namespace WebTracNghiemOnline.Repository
{
    public interface IAnswerRepository
    {
        Task<IEnumerable<Answer>> GetAllAsync();
        Task<Answer?> GetByIdAsync(int id); // 
        Task<Answer> CreateAsync(Answer answer);
        Task UpdateAsync(Answer answer);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<List<int>> GetCorrectAnswersByQuestionIdAsync(int questionId);

    }
}
