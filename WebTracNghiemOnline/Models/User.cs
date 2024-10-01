using System.ComponentModel.DataAnnotations;

namespace WebTracNghiemOnline.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Balance { get; set; }

        [Required]
        public string Role { get; set; }

        public ICollection<ExamHistory> ExamHistories { get; set; }
        public ICollection<OnlineRoom> CreatedRooms { get; set; }
        public ICollection<UserOnlineRoom> UserOnlineRooms { get; set; }
    }

}
