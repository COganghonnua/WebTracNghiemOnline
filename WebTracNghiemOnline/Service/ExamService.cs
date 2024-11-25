using AutoMapper;
using WebTracNghiemOnline.DTO;
using WebTracNghiemOnline.Models;
using WebTracNghiemOnline.Repository;

namespace WebTracNghiemOnline.Service
{
    public class ExamService : IExamService
    {
        private readonly IExamRepository _examRepository;
        private readonly IMapper _mapper;

        public ExamService(IExamRepository examRepository, IMapper mapper)
        {
            _examRepository = examRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ExamDTO>> GetAllExamsAsync()
        {
            var exams = await _examRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ExamDTO>>(exams);
        }

        public async Task<ExamDTO?> GetExamByIdAsync(int id)
        {
            var exam = await _examRepository.GetByIdAsync(id);
            if (exam == null) return null;

            return _mapper.Map<ExamDTO>(exam);
        }

        public async Task<ExamDTO> CreateExamAsync(CreateExamDto createExamDto)
        {
            var exam = _mapper.Map<Exam>(createExamDto);
            var createdExam = await _examRepository.CreateAsync(exam);
            return _mapper.Map<ExamDTO>(createdExam);
        }

        public async Task<ExamDTO> UpdateExamAsync(int id, UpdateExamDto updateExamDto)
        {
            var existingExam = await _examRepository.GetByIdAsync(id);
            if (existingExam == null)
            {
                throw new KeyNotFoundException($"Exam with ID {id} not found.");
            }

            _mapper.Map(updateExamDto, existingExam);
            await _examRepository.UpdateAsync(existingExam);

            // Tải lại thực thể đầy đủ
            var updatedExam = await _examRepository.GetByIdAsync(id);
            return _mapper.Map<ExamDTO>(updatedExam);
        }


        public async Task DeleteExamAsync(int id)
        {
            if (!await _examRepository.ExistsAsync(id))
            {
                throw new KeyNotFoundException($"Exam with ID {id} not found.");
            }

            await _examRepository.DeleteAsync(id);
        }
    }
}
