using System.ComponentModel.DataAnnotations;

namespace WebTracNghiemOnline.Models
{
    public class Transaction
    {
        [Key]
        public string TransactionId { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; } // Succeess, Fail, Waiting
    }
}
