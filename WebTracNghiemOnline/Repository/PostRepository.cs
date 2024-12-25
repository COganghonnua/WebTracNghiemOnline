using Microsoft.EntityFrameworkCore;
using WebTracNghiemOnline.Access;
using WebTracNghiemOnline.Models;

namespace WebTracNghiemOnline.Repository
{
    public interface IPostRepository
    {
        Task<Post> CreatePostAsync(Post post);
        Task AddFileAttachmentsAsync(List<FileAttachment> fileAttachments);
        Task<List<Post>> GetPostsByRoomAsync(int roomId);
        Task<Comment> CreateCommentAsync(Comment comment);
        Task<List<Comment>> GetCommentsByPostIdAsync(int postId);
    }
    public class PostRepository : IPostRepository
    {
        private readonly ApplicationDbContext _context;

        public PostRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Post> CreatePostAsync(Post post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task AddFileAttachmentsAsync(List<FileAttachment> fileAttachments)
        {
            _context.FileAttachments.AddRange(fileAttachments);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Post>> GetPostsByRoomAsync(int roomId)
        {
            return await _context.Posts
                .Include(p => p.User) // Lấy thông tin người tạo bài viết
                .Include(p => p.Comments) // Lấy danh sách bình luận
                    .ThenInclude(c => c.User) // Lấy thông tin người bình luận
                .Include(p => p.FileAttachments) // Bao gồm các tệp đính kèm
                .Where(p => p.OnlineRoomId == roomId && !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<Comment> CreateCommentAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<List<Comment>> GetCommentsByPostIdAsync(int postId)
        {
            return await _context.Comments
                .Where(c => c.PostId == postId && !c.IsDeleted)
                .Include(c => c.User)
                .ToListAsync();
        }
    }
}
