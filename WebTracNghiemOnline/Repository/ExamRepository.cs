using Microsoft.EntityFrameworkCore;
using WebTracNghiemOnline.Access;
using WebTracNghiemOnline.Models;

namespace WebTracNghiemOnline.Repository
{
    public interface IExamRepository
    {
        Task CreateExamAsync(Exam exam);
        Task<IEnumerable<Exam>> GetAllExamsAsync();
        Task<Exam> GetExamByIdAsync(int id);
        Task DeleteExamAsync(Exam exam);
        Task UpdateExamAsync(Exam exam);
        Task<Exam?> GetExamWithQuestionsAsync(int id);
        Task SaveExamHistoryAsync(ExamHistory examHistory);
        Task SaveExamHistoryAsync(ExamHistory examHistory, List<ExamHistoryAnswer> historyAnswers);
        Task<ExamHistory?> GetExamHistoryDetailsAsync(int examHistoryId, string userId);
        Task SaveExamHistoryAnswersAsync(List<ExamHistoryAnswer> historyAnswers);
    }

    public class ExamRepository : IExamRepository
    {
        private readonly ApplicationDbContext _context;

        public ExamRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateExamAsync(Exam exam)
        {
            _context.Exams.Add(exam);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Exam>> GetAllExamsAsync()
        {
            return await _context.Exams.Include(e => e.Subject).ToListAsync();
        }

        public async Task<Exam> GetExamByIdAsync(int id)
        {
            return await _context.Exams.Include(e => e.Subject).FirstOrDefaultAsync(e => e.ExamId == id);
        }

        public async Task DeleteExamAsync(Exam exam)
        {
            _context.Exams.Remove(exam);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateExamAsync(Exam exam)
        {
            _context.Exams.Update(exam);
            await _context.SaveChangesAsync();
        }
        public async Task<Exam?> GetExamWithQuestionsAsync(int id)
        {
            return await _context.Exams
                .Include(e => e.ExamQuestions) // Bao gồm bảng trung gian ExamQuestions
                    .ThenInclude(eq => eq.Question) // Bao gồm bảng Question
                        .ThenInclude(q => q.Answers) // Bao gồm bảng Answer
                .FirstOrDefaultAsync(e => e.ExamId == id);
        }
        public async Task SaveExamHistoryAsync(ExamHistory examHistory)
        {
            _context.ExamHistories.Add(examHistory);
            await _context.SaveChangesAsync();
        }
        public async Task SaveExamHistoryAsync(ExamHistory examHistory, List<ExamHistoryAnswer> historyAnswers)
        {
            _context.ExamHistories.Add(examHistory);
            _context.ExamHistoryAnswers.AddRange(historyAnswers);
            await _context.SaveChangesAsync();
        }
        public async Task<ExamHistory?> GetExamHistoryDetailsAsync(int examHistoryId, string userId)
        {
            return await _context.ExamHistories
                .Include(eh => eh.Exam) // Bao gồm bài thi
                    .ThenInclude(ex => ex.ExamQuestions) // Bao gồm câu hỏi trong bài thi
                        .ThenInclude(eq => eq.Question) // Bao gồm câu hỏi
                            .ThenInclude(q => q.Answers) // Bao gồm đáp án của câu hỏi
                .Include(eh => eh.ExamHistoryAnswers) // Bao gồm lịch sử câu trả lời
                .FirstOrDefaultAsync(eh => eh.ExamHistoryId == examHistoryId && eh.UserId == userId);
        }




        public async Task SaveExamHistoryAnswersAsync(List<ExamHistoryAnswer> historyAnswers)
        {
            _context.ExamHistoryAnswers.AddRange(historyAnswers);
            await _context.SaveChangesAsync();
        }



    }

}
