using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebTracNghiemOnline.Models
{
    public class FileAttachment
    {
        public int FileAttachmentId { get; set; }

        [Required]
        public string FileName { get; set; } // Tên file (hiển thị)

        [Required]
        public string FilePath { get; set; } // Đường dẫn đến file trên server

        public string FileType { get; set; } // Loại file: image, video, pdf, etc.

        public int PostId { get; set; }
        [JsonIgnore]
        public Post Post { get; set; }
    }
}
