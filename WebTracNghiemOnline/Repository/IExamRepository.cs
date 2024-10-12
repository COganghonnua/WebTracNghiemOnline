using WebTracNghiemOnline.Models;

namespace WebTracNghiemOnline.Repository
{
    public interface IExamRepository
    {
        Task<IEnumerable<Exam>> GetAllAsync();
        Task<Exam?> GetByIdAsync(int id);
        Task<Exam> CreateAsync(Exam exam);
        Task<Exam?> UpdateAsync(int id, Exam exam);
        Task<Exam?> DeleteAsync(int id);
    }
}
