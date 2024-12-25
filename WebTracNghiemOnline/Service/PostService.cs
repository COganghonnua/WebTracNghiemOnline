using AutoMapper;
using WebTracNghiemOnline.DTO;
using WebTracNghiemOnline.Models;
using WebTracNghiemOnline.Repository;

namespace WebTracNghiemOnline.Service
{
    public interface IPostService
    {
        Task<PostResponseDto> CreatePostAsync(PostRequestDto postRequestDto, string userId);
        Task<List<PostResponseDto>> GetPostsByRoomAsync(int roomId);
        Task<CommentResponseDto> CreateCommentAsync(CommentRequestDto commentRequestDto, string userId);
        Task<List<CommentResponseDto>> GetCommentsByPostIdAsync(int postId);
    }
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PostService(IPostRepository postRepository, IWebHostEnvironment environment, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _postRepository = postRepository;
            _environment = environment;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PostResponseDto> CreatePostAsync(PostRequestDto postRequestDto, string userId)
        {
            var post = new Post
            {
                Content = postRequestDto.Content,
                OnlineRoomId = postRequestDto.OnlineRoomId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            var createdPost = await _postRepository.CreatePostAsync(post);

            if (postRequestDto.Attachments != null && postRequestDto.Attachments.Any())
            {
                var fileAttachments = new List<FileAttachment>();
                foreach (var file in postRequestDto.Attachments)
                {
                    // Tạo thư mục "uploads" nếu chưa tồn tại
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    // Lưu file vào thư mục "uploads"
                    var filePath = Path.Combine(uploadsFolder, file.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Tạo đường dẫn đầy đủ cho file
                    var request = _httpContextAccessor.HttpContext.Request;
                    var fullPath = $"{request.Scheme}://{request.Host}/uploads/{file.FileName}";

                    fileAttachments.Add(new FileAttachment
                    {
                        FileName = file.FileName,
                        FilePath = fullPath, // Đường dẫn đầy đủ
                        FileType = file.ContentType,
                        PostId = createdPost.PostId
                    });
                }

                // Lưu thông tin tệp đính kèm vào database
                await _postRepository.AddFileAttachmentsAsync(fileAttachments);
            }

            // Trả về DTO của bài viết
            return _mapper.Map<PostResponseDto>(createdPost);
        }




        public async Task<List<PostResponseDto>> GetPostsByRoomAsync(int roomId)
        {
            var posts = await _postRepository.GetPostsByRoomAsync(roomId);
            return _mapper.Map<List<PostResponseDto>>(posts); // Dùng AutoMapper
        }


        public async Task<CommentResponseDto> CreateCommentAsync(CommentRequestDto commentRequestDto, string userId)
        {
            var comment = new Comment
            {
                Content = commentRequestDto.Content,
                PostId = commentRequestDto.PostId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            var createdComment = await _postRepository.CreateCommentAsync(comment);

            // Trả về DTO
            return new CommentResponseDto
            {
                CommentId = createdComment.CommentId,
                Content = createdComment.Content,
                CreatedAt = createdComment.CreatedAt,
                UserId = createdComment.UserId,
                UserFullName = createdComment.User.FullName
            };
        }

        public async Task<List<CommentResponseDto>> GetCommentsByPostIdAsync(int postId)
        {
            var comments = await _postRepository.GetCommentsByPostIdAsync(postId);

            return comments.Select(c => new CommentResponseDto
            {
                CommentId = c.CommentId,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                UserId = c.UserId,
                UserFullName = c.User.FullName
            }).ToList();
        }

    }
}
