using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebTracNghiemOnline.DTO;
using WebTracNghiemOnline.Service;
using WebTracNghiemOnline.Services;

namespace WebTracNghiemOnline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IAuthService _authService;

        public PostController(IPostService postService, IAuthService authService)
        {
            _postService = postService;
            _authService = authService;
        }

        // Tạo bài viết mới
        [HttpPost("create")]
        public async Task<IActionResult> CreatePost([FromForm] PostRequestDto postRequestDto)
        {
            try
            {
                var token = Request.Cookies["jwt"]; // Lấy token từ cookie
                if (string.IsNullOrEmpty(token))
                    return Unauthorized(new { message = "Token not found. Please log in." });

                var user = await _authService.ValidateTokenAsync(token); // Xác thực token
                if (user == null)
                    return Unauthorized(new { message = "Invalid token. Please log in again." });

                var post = await _postService.CreatePostAsync(postRequestDto, user.Id);
                return Ok(new { message = "Post created successfully", post });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Lấy danh sách bài viết trong một phòng
        [HttpGet("{roomId}")]
        public async Task<IActionResult> GetPosts(int roomId)
        {
            try
            {
                var token = Request.Cookies["jwt"]; // Lấy token từ cookie
                if (string.IsNullOrEmpty(token))
                    return Unauthorized(new { message = "Token not found. Please log in." });

                var user = await _authService.ValidateTokenAsync(token);
                if (user == null)
                    return Unauthorized(new { message = "Invalid token. Please log in again." });

                var posts = await _postService.GetPostsByRoomAsync(roomId);
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        // Thêm bình luận cho bài viết
        [HttpPost("{postId}/comments")]
        public async Task<IActionResult> CreateComment(int postId, [FromBody] CommentRequestDto commentRequestDto)
        {
            try
            {
                var token = Request.Cookies["jwt"];
                if (string.IsNullOrEmpty(token))
                    return Unauthorized(new { message = "Token not found. Please log in." });

                var user = await _authService.ValidateTokenAsync(token);
                if (user == null)
                    return Unauthorized(new { message = "Invalid token. Please log in again." });

                commentRequestDto.PostId = postId;
                var comment = await _postService.CreateCommentAsync(commentRequestDto, user.Id);
                return Ok(new { message = "Comment added successfully", comment });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Lấy danh sách bình luận của bài viết
        [HttpGet("{postId}/comments")]
        public async Task<IActionResult> GetComments(int postId)
        {
            try
            {
                var comments = await _postService.GetCommentsByPostIdAsync(postId);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
