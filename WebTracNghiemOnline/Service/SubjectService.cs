using WebTracNghiemOnline.DTO;
using WebTracNghiemOnline.Models;
using WebTracNghiemOnline.Repository;
using AutoMapper;

namespace WebTracNghiemOnline.Service
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly IMapper _mapper;

        public SubjectService(ISubjectRepository subjectRepository, IMapper mapper)
        {
            _subjectRepository = subjectRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SubjectDTO>> GetAllSubjectsAsync()
        {
            var subjects = await _subjectRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<SubjectDTO>>(subjects);
        }

        public async Task<SubjectDTO?> GetSubjectByIdAsync(int id)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);
            return subject == null ? null : _mapper.Map<SubjectDTO>(subject);
        }

        public async Task<SubjectDTO> CreateSubjectAsync(CreateSubjectDto createSubjectDto)
        {
            var subject = _mapper.Map<Subject>(createSubjectDto);
            var createdSubject = await _subjectRepository.CreateAsync(subject);
            return _mapper.Map<SubjectDTO>(createdSubject);
        }

        public async Task UpdateSubjectAsync(int id, UpdateSubjectDto updateSubjectDto)
        {
            var existingSubject = await _subjectRepository.GetByIdAsync(id); // null 
            if (existingSubject == null)
            {
                throw new KeyNotFoundException($"Subject with ID {id} not found."); 
            }

            _mapper.Map(updateSubjectDto, existingSubject);
            await _subjectRepository.UpdateAsync(existingSubject);
        }

        public async Task DeleteSubjectAsync(int id)
        {
            if (!await _subjectRepository.ExistsAsync(id))
            {
                throw new KeyNotFoundException($"Subject with ID {id} not found.");
            }

            await _subjectRepository.DeleteAsync(id);
        }
    }
}
