using Microsoft.EntityFrameworkCore;
using WebTracNghiemOnline.Access;
using WebTracNghiemOnline.Models;

namespace WebTracNghiemOnline.Repository
{
        public class QuestionRepository : IQuestionRepository
        {
            private readonly ApplicationDbContext _context;

            public QuestionRepository(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<Question>> GetAllAsync()
            {
                return await _context.Questions
                    .Include(q => q.Subject)
                    .Include(q => q.Answers)
                    .ToListAsync();
            }

            public async Task<Question?> GetByIdAsync(int id)
            {
                return await _context.Questions
                    .Include(q => q.Subject)
                    .Include(q => q.Answers)
                    .FirstOrDefaultAsync(q => q.QuestionId == id);
            }

            public async Task<Question> CreateAsync(Question question)
            {
                _context.Questions.Add(question);
                await _context.SaveChangesAsync();
                return await GetByIdAsync(question.QuestionId); // Load đầy đủ thông tin
            }

            public async Task UpdateAsync(Question question)
            {
                _context.Questions.Update(question);
                await _context.SaveChangesAsync();
            }

            public async Task DeleteAsync(int id)
            {
                var question = await _context.Questions.FindAsync(id);
                if (question != null)
                {
                    _context.Questions.Remove(question);
                    await _context.SaveChangesAsync();
                }
            }

            public async Task<bool> ExistsAsync(int id)
            {
                return await _context.Questions.AnyAsync(q => q.QuestionId == id);
        }
        public async Task<List<Question>> GetQuestionsByDifficultyAsync(int subjectId, DifficultyLevel difficulty)
        {
            return await _context.Questions
                .Where(q => q.SubjectId == subjectId && q.Difficulty == difficulty)
                .ToListAsync();
        }
        // 3 biến lưu 3 list Dễ Khó Trung Bình
        public async Task<List<Question>> GetQuestionsByExamIdAsync(int examId)
        {
            return await _context.Questions
            .Where(q => q.ExamQuestions.Any(eq => eq.ExamId == examId))
            .ToListAsync();
        }

    }
}
