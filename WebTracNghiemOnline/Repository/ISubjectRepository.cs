using WebTracNghiemOnline.Models;

namespace WebTracNghiemOnline.Repository
{
    public interface ISubjectRepository
    {
        Task<IEnumerable<Subject>> GetAllAsync();
        Task<Subject?> GetByIdAsync(int id);
        Task<Subject> CreateAsync(Subject subject);
        Task UpdateAsync(Subject subject);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
