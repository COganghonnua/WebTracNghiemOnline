using Microsoft.AspNetCore.Http;

namespace WebTracNghiemOnline.DTO
{
    public class PostRequestDto
    {
        public string Content { get; set; } // Nội dung bài viết
        public int OnlineRoomId { get; set; } // ID phòng học
        public List<IFormFile>? Attachments { get; set; } // Danh sách file tải lên
    }
    public class PostResponseDto
    {
        public int PostId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } // Thông tin người tạo bài viết
        public List<FileAttachmentDto> Attachments { get; set; }
        public List<CommentResponseDto> Comments { get; set; } // Danh sách bình luận
    }


    public class FileAttachmentDto
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
    }
}
