using WebTracNghiemOnline.Models;

namespace WebTracNghiemOnline.Repository
{
    public interface IExamRepository
    {
        Task<IEnumerable<Exam>> GetAllAsync();
        Task<Exam?> GetByIdAsync(int id);
        Task<Exam> CreateAsync(Exam exam);
        Task UpdateAsync(Exam exam);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
