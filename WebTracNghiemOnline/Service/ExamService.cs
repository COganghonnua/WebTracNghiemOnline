using AutoMapper;
using WebTracNghiemOnline.DTO;
using WebTracNghiemOnline.Models;
using WebTracNghiemOnline.Repository;

namespace WebTracNghiemOnline.Service
{
    public interface IExamService
    {
        Task<IEnumerable<ExamDTO>> GetAllExamsAsync();
        Task<(bool Success, string Message)> DeleteExamAsync(int id);
        Task<(bool Success, string Message)> UpdateExamAsync(int id, UpdateExamDto updateExamDto);
        Task<(bool Success, string Message, object Data)> CreateRandomExamAsync(CreateExamDto dto, NumberOfQuestionsDto numberOfQuestions);
    }

    public class ExamService : IExamService
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IExamRepository _examRepository;
        private readonly IMapper _mapper;

        public ExamService(IQuestionRepository questionRepository, IExamRepository examRepository, IMapper mapper)
        {
            _questionRepository = questionRepository;
            _examRepository = examRepository;
            _mapper = mapper;
        }

        public async Task<(bool Success, string Message, object Data)> CreateRandomExamAsync(CreateExamDto dto, NumberOfQuestionsDto numberOfQuestions)
        {
            // Lấy danh sách câu hỏi từ repository
            var easyQuestions = await _questionRepository.GetQuestionsByDifficultyAsync(dto.SubjectId, DifficultyLevel.Easy);
            var mediumQuestions = await _questionRepository.GetQuestionsByDifficultyAsync(dto.SubjectId, DifficultyLevel.Medium);
            var hardQuestions = await _questionRepository.GetQuestionsByDifficultyAsync(dto.SubjectId, DifficultyLevel.Hard);

            // Kiểm tra số lượng câu hỏi
            if (easyQuestions.Count < numberOfQuestions.Easy ||
                mediumQuestions.Count < numberOfQuestions.Medium ||
                hardQuestions.Count < numberOfQuestions.Hard)
            {
                return (false, "Not enough questions available", new
                {
                    easy = new { requested = numberOfQuestions.Easy, available = easyQuestions.Count },
                    medium = new { requested = numberOfQuestions.Medium, available = mediumQuestions.Count },
                    hard = new { requested = numberOfQuestions.Hard, available = hardQuestions.Count }
                });
            }

            // Random câu hỏi
            var random = new Random();
            var selectedEasy = easyQuestions.OrderBy(x => random.Next()).Take(numberOfQuestions.Easy).ToList();
            var selectedMedium = mediumQuestions.OrderBy(x => random.Next()).Take(numberOfQuestions.Medium).ToList();
            var selectedHard = hardQuestions.OrderBy(x => random.Next()).Take(numberOfQuestions.Hard).ToList();

            // Tạo exam entity
            var exam = new Exam
            {
                ExamName = dto.ExamName,
                SubjectId = dto.SubjectId,
                Fee = dto.Fee,
                ExamQuestions = selectedEasy.Concat(selectedMedium).Concat(selectedHard)
                    .Select(q => new ExamQuestion { QuestionId = q.QuestionId })
                    .ToList()
            };

            // Lưu vào database thông qua ExamRepository
            await _examRepository.CreateExamAsync(exam);

            // Chuyển đổi sang DTO
            var resultDto = _mapper.Map<ExamWithQuestionsDto>(exam);

            return (true, "Exam created successfully", resultDto);
        }
        public async Task<IEnumerable<ExamDTO>> GetAllExamsAsync()
        {
            var exams = await _examRepository.GetAllExamsAsync();
            return _mapper.Map<IEnumerable<ExamDTO>>(exams);
        }

        public async Task<(bool Success, string Message)> DeleteExamAsync(int id)
        {
            var exam = await _examRepository.GetExamByIdAsync(id);
            if (exam == null)
            {
                return (false, "Exam not found.");
            }

            await _examRepository.DeleteExamAsync(exam);
            return (true, "Exam deleted successfully.");
        }

        public async Task<(bool Success, string Message)> UpdateExamAsync(int id, UpdateExamDto updateExamDto)
        {
            var exam = await _examRepository.GetExamByIdAsync(id);
            if (exam == null)
            {
                return (false, "Exam not found.");
            }

            exam.ExamName = updateExamDto.ExamName;
            exam.Fee = updateExamDto.Fee;
            exam.SubjectId = updateExamDto.SubjectId;

            await _examRepository.UpdateExamAsync(exam);
            return (true, "Exam updated successfully.");
        }

    }

}
