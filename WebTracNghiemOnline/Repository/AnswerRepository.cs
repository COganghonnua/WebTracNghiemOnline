using Microsoft.EntityFrameworkCore;
using WebTracNghiemOnline.Access;
using WebTracNghiemOnline.Models;

namespace WebTracNghiemOnline.Repository
{
    public class AnswerRepository : IAnswerRepository
    {
        private readonly ApplicationDbContext dbContext;

        public AnswerRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Answer> CreateAsync(Answer answer)
        {
            dbContext.Answers.Add(answer);
            await dbContext.SaveChangesAsync();
            return answer;
        }

        public async Task DeleteAsync(int id)
        {
            var answer = dbContext.Answers.FirstOrDefault(x => x.AnswerId == id);
            if (answer != null)
            {
                dbContext.Answers.Remove(answer);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await dbContext.Answers.AnyAsync(s => s.AnswerId == id);
        }

        public async Task<IEnumerable<Answer>> GetAllAsync()
        {
            return await dbContext.Answers.Include(x => x.Question).ToListAsync();
        }

        public async Task<Answer?> GetByIdAsync(int id)
        {
            return await dbContext.Answers
                .Include(x =>x.Question)
                .FirstOrDefaultAsync(x => x.AnswerId == id);
        }

        public async Task UpdateAsync(Answer answer)
        {
            dbContext.Entry(answer).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }
        public async Task<List<int>> GetCorrectAnswersByQuestionIdAsync(int questionId)
        {
            return await dbContext.Answers
            .Where(a => a.QuestionId == questionId && a.IsCorrect == true)
            .Select(a => a.AnswerId)
            .ToListAsync();
        }

    }
}
