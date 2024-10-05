using Microsoft.EntityFrameworkCore;
using WebTracNghiemOnline.Access;
using WebTracNghiemOnline.Models;

namespace WebTracNghiemOnline.Repository
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ApplicationDbContext dbContext;

        public QuestionRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Question> CreateAsync(Question question)
        {
            await dbContext.Questions.AddAsync(question);
            await dbContext.SaveChangesAsync();
            return question;
        }

        public async Task<Question?> DeleteAsync(int id)
        {
            var existingQuestion = await dbContext.Questions.FirstOrDefaultAsync(x => x.QuestionId == id);
            if (existingQuestion == null)
            {
                return null;
            }
            dbContext.Questions.Remove(existingQuestion);
            await dbContext.SaveChangesAsync();
            return existingQuestion;
        }

        public async Task<IEnumerable<Question>> GetAllAsync()
        {
            return await dbContext.Questions
                .Include(x => x.Exam)
                .Include(x => x.Answers)
                .ToListAsync();
        }

        public async Task<Question?> GetByIdAsync(int id)
        {
            return await dbContext.Questions
                .Include(x => x.Exam)
                .Include(x => x.Answers)
                .FirstOrDefaultAsync(x => x.QuestionId == id);
        }

        public async Task<Question?> UpdateAsync(int id, Question question)
        {
            var existingQuestion = await dbContext.Questions
                .Include(x => x.Exam)
                .Include(x => x.Answers)
                .FirstOrDefaultAsync(x => x.QuestionId == id);

            if (existingQuestion == null) 
            {
                return null;
            }

            existingQuestion.QuestionText = question.QuestionText;
            existingQuestion.Explanation = question.Explanation;
            existingQuestion.ExamId = question.ExamId;
            
            await dbContext.SaveChangesAsync();
            return existingQuestion;
        }
    }
}
