using Microsoft.EntityFrameworkCore;
using WebTracNghiemOnline.Access;
using WebTracNghiemOnline.Models;

namespace WebTracNghiemOnline.Repository
{
    public class TopicRepository : ITopicRepository
    {
        
        private readonly ApplicationDbContext _context;

        public TopicRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Topic>> GetAllAsync()
        {
            return await _context.Topics.Include(t => t.Subjects).ToListAsync();
        }

        public async Task<Topic?> GetByIdAsync(int id)
        {
            return await _context.Topics
                .Include(t => t.Subjects)
                .FirstOrDefaultAsync(t => t.TopicId == id);
        }

        public async Task<Topic> CreateAsync(Topic topic)
        {
            _context.Topics.Add(topic);
            await _context.SaveChangesAsync();
            return topic;
        }

        public async Task<Topic> UpdateAsync(Topic topic)
        {
            _context.Entry(topic).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            // Load lại dữ liệu từ database để đảm bảo dữ liệu mới nhất
            return await _context.Topics
                .FirstOrDefaultAsync(t => t.TopicId == topic.TopicId);
        }


        public async Task DeleteAsync(int id)
        {
            var topic = await _context.Topics.FindAsync(id); 
            if (topic != null)
            {
                _context.Topics.Remove(topic);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Topics.AnyAsync(t => t.TopicId == id);
        }
    }
}
