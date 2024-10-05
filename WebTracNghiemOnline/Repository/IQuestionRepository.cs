using WebTracNghiemOnline.Models;

namespace WebTracNghiemOnline.Repository
{
    public interface IQuestionRepository
    {
        Task<IEnumerable<Question>> GetAllAsync();
        Task<Question?> GetByIdAsync(int id);
        Task<Question> CreateAsync(Question question);
        Task<Question?> UpdateAsync(int id, Question question);
        Task<Question?> DeleteAsync(int id);
    }
}
