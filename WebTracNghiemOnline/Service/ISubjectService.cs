using WebTracNghiemOnline.DTO;

namespace WebTracNghiemOnline.Service
{
    public interface ISubjectService
    {
        Task<IEnumerable<SubjectDTO>> GetAllSubjectsAsync();
        Task<SubjectDTO?> GetSubjectByIdAsync(int id);
        Task<SubjectDTO> CreateSubjectAsync(CreateSubjectDto createSubjectDto);
        Task UpdateSubjectAsync(int id, UpdateSubjectDto updateSubjectDto);
        Task DeleteSubjectAsync(int id);
    }
}
