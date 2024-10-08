using Microsoft.EntityFrameworkCore;
using WebTracNghiemOnline.Access;
using WebTracNghiemOnline.Models;

namespace WebTracNghiemOnline.Repository
{
    public class ExamRepository : IExamRepository
    {
        private readonly ApplicationDbContext dbContext;

        public ExamRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Exam> CreateAsync(Exam exam)
        {
            await dbContext.Exams.AddAsync(exam);
            await dbContext.SaveChangesAsync();
            return exam;
        }

        public async Task<Exam?> DeleteAsync(int id)
        {
            var existingExam = await dbContext.Exams.FirstOrDefaultAsync(x => x.ExamId == id);

            if (existingExam == null) 
            {
                return null;
            }

            dbContext.Exams.Remove(existingExam);
            await dbContext.SaveChangesAsync();

            return existingExam;
        }

        public async Task<IEnumerable<Exam>> GetAllAsync()
        {
            return await dbContext.Exams
                .Include(x => x.Subject)
                .ToListAsync();
        }

        public async Task<Exam?> GetByIdAsync(int id)
        {
            return await dbContext.Exams
                .Include(x => x.Subject)
                .FirstOrDefaultAsync(x => x.ExamId == id);
        }

        public async Task<Exam?> UpdateAsync(int id, Exam exam)
        {
            var existingExam = await dbContext.Exams
                .Include(x => x.Subject)
                .Include (x => x.Questions)
                .ThenInclude (x => x.Answers)
                .FirstOrDefaultAsync(x => x.ExamId == id);

            if (existingExam == null) 
            {
                return null;
            }

            existingExam.ExamName = exam.ExamName;
            existingExam.SubjectId = exam.SubjectId;
            existingExam.Fee = exam.Fee;

            await dbContext.SaveChangesAsync();

            return existingExam;
        }
    }
}
