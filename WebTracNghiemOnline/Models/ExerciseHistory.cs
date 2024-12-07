namespace WebTracNghiemOnline.Models
{
    public class ExerciseHistory
    {
        public int ExerciseHistoryId { get; set; }

        public int ExerciseId { get; set; }
        public Exercise Exercise { get; set; } // Bài tập liên quan

        public string UserId { get; set; }
        public User User { get; set; } // Người dùng làm bài


        // Điểm số tổng kết bài tập
        public int Score { get; set; } = 0;

        // Thời gian làm bài tập
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        // Trạng thái hoàn thành bài tập
        public bool IsCompleted { get; set; } = false;

    }


}
