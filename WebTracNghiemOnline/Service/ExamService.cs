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
        Task<ExamWithQuestionsDto?> GetExamWithQuestionsAsync(int examId);
        Task<(bool Success, string Message, dynamic Data)> SubmitExamAsync(int examId, string userId, SubmitExamDto submitExamDto);
        Task<(bool Success, string Message, int Score)>CheckExamAnswersAsync(ExamSubmissionDto examSubmission);
        Task<(bool Success, string Message, object? Data)> GetExamHistoryDetailsAsync(int examHistoryId, string userId);
    }

    public class ExamService : IExamService
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IExamRepository _examRepository;
        private readonly IAnswerRepository _answerRepository;
        private readonly IMapper _mapper;

        public ExamService(IQuestionRepository questionRepository, IExamRepository examRepository,IAnswerRepository answerRepository, IMapper mapper)
        {
            _questionRepository = questionRepository;
            _examRepository = examRepository;
            _answerRepository = answerRepository;
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
                Duration = dto.Duration,
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
        public async Task<ExamWithQuestionsDto?> GetExamWithQuestionsAsync(int examId)
        {
            // Lấy thông tin bài thi từ repository
            var exam = await _examRepository.GetExamWithQuestionsAsync(examId);
            if (exam == null) return null;

            // Map thông tin sang DTO
            return _mapper.Map<ExamWithQuestionsDto>(exam);
        }
        public async Task<(bool Success, string Message, dynamic Data)> SubmitExamAsync(int examId, string userId, SubmitExamDto submitExamDto)
        {
            // Lấy thông tin bài thi từ cơ sở dữ liệu
            var exam = await _examRepository.GetExamWithQuestionsAsync(examId);
            if (exam == null)
                return (false, "Exam not found.", null);

            // Lấy danh sách đáp án đúng cho các câu hỏi
            var correctAnswers = exam.ExamQuestions
                .SelectMany(eq => eq.Question.Answers)
                .Where(a => a.IsCorrect)
                .ToDictionary(a => a.QuestionId, a => a);

            int totalQuestions = exam.ExamQuestions.Count;
            int correctCount = 0;

            // Danh sách lưu lịch sử câu trả lời
            var historyAnswers = new List<ExamHistoryAnswer>();

            foreach (var userAnswer in submitExamDto.UserAnswers)
            {
                var questionId = userAnswer.Key;
                var selectedAnswers = userAnswer.Value;

                // Lấy đáp án đúng của câu hỏi hiện tại
                var correctAnswerIds = correctAnswers
                    .Where(c => c.Key == questionId)
                    .Select(c => c.Value.AnswerId)
                    .ToList();

                // Kiểm tra đúng/sai
                bool isCorrect = !correctAnswerIds.Except(selectedAnswers).Any() &&
                                 !selectedAnswers.Except(correctAnswerIds).Any();
                if (isCorrect)
                    correctCount++;

                // Thêm vào danh sách lịch sử câu trả lời
                historyAnswers.Add(new ExamHistoryAnswer
                {
                    QuestionId = questionId,
                    SelectedAnswerIds = string.Join(",", selectedAnswers),
                    IsCorrect = isCorrect
                });
            }

            // Tính điểm
            int score = (int)Math.Round((double)correctCount / totalQuestions * 10, 2);

            // Tạo lịch sử bài thi
            var examHistory = new ExamHistory
            {
                ExamId = examId,
                UserId = userId,
                ExamDate = DateTime.UtcNow,
                Score = score,
                Duration = submitExamDto.TimeTaken
            };

            // Lưu ExamHistory trước để lấy ID
            await _examRepository.SaveExamHistoryAsync(examHistory);

            // Cập nhật ExamHistoryId vào danh sách câu trả lời
            foreach (var answer in historyAnswers)
            {
                answer.ExamHistoryId = examHistory.ExamHistoryId;
            }

            // Lưu danh sách lịch sử câu trả lời
            await _examRepository.SaveExamHistoryAnswersAsync(historyAnswers);

            // Trả về kết quả
            return (true, "Exam submitted successfully.", new
            {
                Score = score,
                TotalQuestions = totalQuestions,
                CorrectAnswers = correctCount
            });
        }



        public async Task<(bool Success, string Message, int Score)>CheckExamAnswersAsync(ExamSubmissionDto examSubmission)
        {
            // Lấy thông tin exam từ ExamRepository
            var exam = await _examRepository.GetExamByIdAsync(examSubmission.ExamId);
            if (exam == null)
            {
                return (false, "Exam not found.", 0);
            }
            // Lấy danh sách câu hỏi của exam
            var questions = await
           _questionRepository.GetQuestionsByExamIdAsync(examSubmission.ExamId);
            // Khởi tạo điểm số
            int score = 0;
            // Kiểm tra câu trả lời cho từng câu hỏi
            foreach (var userAnswer in examSubmission.Answers)
            {
                var question = questions.FirstOrDefault(q => q.QuestionId ==
               userAnswer.QuestionId);
                if (question == null)
                {
                    continue; // Nếu không tìm thấy câu hỏi, bỏ qua
                }
                // Lấy các câu trả lời đúng của câu hỏi từ database
                var correctAnswers = await
               _answerRepository.GetCorrectAnswersByQuestionIdAsync(userAnswer.QuestionId);
                // So sánh câu trả lời người dùng với câu trả lời đúng
                if (correctAnswers.Count == userAnswer.AnswerIds.Count &&
                !correctAnswers.Except(userAnswer.AnswerIds).Any())
                {
                    score++; // Nếu đúng, cộng điểm
                }
            }
            return (true, "Answers checked successfully.", score);
        }
        public async Task<(bool Success, string Message, object? Data)> GetExamHistoryDetailsAsync(int examHistoryId, string userId)
        {
            var history = await _examRepository.GetExamHistoryDetailsAsync(examHistoryId, userId);
            if (history == null) return (false, "Exam history not found.", null);

            // Xử lý câu hỏi và đáp án trong bài thi
            var questions = history.Exam.ExamQuestions.Select(eq => new
            {
                eq.QuestionId,
                QuestionText = eq.Question.QuestionText,
                SelectedAnswers = history.ExamHistoryAnswers
                    .Where(a => a.QuestionId == eq.QuestionId)
                    .SelectMany(a => a.SelectedAnswerIds.Split(",").Select(int.Parse)).ToList(),
                IsCorrect = history.ExamHistoryAnswers
                    .Where(a => a.QuestionId == eq.QuestionId)
                    .Select(a => a.IsCorrect).FirstOrDefault(),
                CorrectAnswers = eq.Question.Answers
                    .Where(ans => ans.IsCorrect)
                    .Select(ans => ans.AnswerId).ToList()
            }).ToList();

            var result = new
            {
                ExamName = history.Exam.ExamName,
                Score = history.Score,
                ExamDate = history.ExamDate,
                Questions = questions
            };

            return (true, "History retrieved successfully.", result);
        }






    }

}
