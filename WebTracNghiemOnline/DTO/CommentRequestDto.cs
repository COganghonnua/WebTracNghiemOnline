namespace WebTracNghiemOnline.DTO
{
    public class CommentRequestDto
    {
        public string Content { get; set; } // Nội dung bình luận
        public int PostId { get; set; }     // ID bài viết
    }
    public class CommentResponseDto
    {
        public int CommentId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
        public string UserFullName { get; set; }
    }
}
