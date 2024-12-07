namespace WebTracNghiemOnline.Models
{
    public class Exercise
    {
        public int ExerciseId { get; set; }
        public string ExerciseName { get; set; } // Tên bài tập
        public DateTime StartTime { get; set; } // Thời gian bắt đầu
        public DateTime EndTime { get; set; } // Thời gian kết thúc
        public int OnlineRoomId { get; set; } // Liên kết lớp học
        public OnlineRoom OnlineRoom { get; set; }
        public ICollection<ExerciseQuestion>? ExerciseQuestions { get; set; } // Các câu hỏi
        public ICollection<ExerciseHistory>? ExerciseHistories { get; set; } // Lưu lịch sử làm bài tập
    }

}
