using Microsoft.EntityFrameworkCore;
using WebTracNghiemOnline.Access;
using WebTracNghiemOnline.Models;

namespace WebTracNghiemOnline.Repository
{
    public interface IOnlineRoomRepository
    {
        Task<OnlineRoom> CreateRoomAsync(OnlineRoom room);
        Task<OnlineRoom?> GetRoomByCodeAsync(string roomCode);
        Task<bool> IsUserInRoomAsync(string userId, int roomId);
        Task<UserOnlineRoom> AddUserToRoomAsync(UserOnlineRoom userOnlineRoom);

        Task<bool> LeaveRoomAsync(string userId, int roomId);
        Task<Exercise> AddExerciseAsync(Exercise exercise);
        Task<UserOnlineRoom?> GetUserInRoomAsync(string userId, int roomId);
        Task<List<UserOnlineRoom>> GetUsersInRoomAsync(int roomId);

        Task<List<OnlineRoom>> GetUserRoomsAsync(string userId);
        Task<Exercise?> GetExerciseWithQuestionsAsync(int exerciseId);

        // Lưu lịch sử bài tập
        Task<ExerciseHistory> AddExerciseHistoryAsync(ExerciseHistory exerciseHistory);

        // Lấy danh sách bài tập trong một phòng
        Task<List<Exercise>> GetExercisesInRoomAsync(int roomId);

        // Lấy lịch sử bài tập của một người dùng
        Task<List<ExerciseHistory>> GetUserExerciseHistoriesAsync(string userId, int exerciseId);
        Task<List<ExerciseQuestion>> GetQuestionsWithAnswersAsync(int exerciseId);

    }

    public class OnlineRoomRepository : IOnlineRoomRepository
    {
        private readonly ApplicationDbContext _context;

        public OnlineRoomRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OnlineRoom> CreateRoomAsync(OnlineRoom room)
        {
            _context.OnlineRooms.Add(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<OnlineRoom?> GetRoomByCodeAsync(string roomCode)
        {
            return await _context.OnlineRooms
                .Include(r => r.UserOnlineRooms) // Nếu cần thêm thông tin về người dùng trong phòng
                .FirstOrDefaultAsync(r => r.RoomCode == roomCode);
        }


        public async Task<bool> IsUserInRoomAsync(string userId, int roomId)
        {
            return await _context.UserOnlineRooms
                .AnyAsync(ur => ur.UserId == userId && ur.OnlineRoomId == roomId);
        }

        public async Task<UserOnlineRoom> AddUserToRoomAsync(UserOnlineRoom userOnlineRoom)
        {
            _context.UserOnlineRooms.Add(userOnlineRoom);
            await _context.SaveChangesAsync();
            return userOnlineRoom;
        }
        public async Task<bool> LeaveRoomAsync(string userId, int roomId)
        {
            var userRoom = await _context.UserOnlineRooms
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.OnlineRoomId == roomId);

            if (userRoom == null) return false;

            _context.UserOnlineRooms.Remove(userRoom); // Xóa bản ghi thay vì cập nhật
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<Exercise> AddExerciseAsync(Exercise exercise)
        {
            _context.Exercises.Add(exercise);
            await _context.SaveChangesAsync();
            return exercise;
        }
        public async Task<UserOnlineRoom?> GetUserInRoomAsync(string userId, int roomId)
        {
            return await _context.UserOnlineRooms
                .Include(ur => ur.OnlineRoom) // Nếu bạn cần thêm thông tin phòng học
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.OnlineRoomId == roomId);
        }
        public async Task<List<UserOnlineRoom>> GetUsersInRoomAsync(int roomId)
        {
            return await _context.UserOnlineRooms
                .Where(ur => ur.OnlineRoomId == roomId)
                .Include(ur => ur.User) // Bao gồm thông tin người dùng
                .ToListAsync();
        }

        public async Task<List<OnlineRoom>> GetUserRoomsAsync(string userId)
        {
            return await _context.UserOnlineRooms
                .Where(ur => ur.UserId == userId)
                .Include(ur => ur.OnlineRoom) // Lấy thông tin phòng
                .Select(ur => ur.OnlineRoom)
                .ToListAsync();
        }
        public async Task<Exercise?> GetExerciseWithQuestionsAsync(int exerciseId)
        {
            return await _context.Exercises
        .Include(e => e.ExerciseQuestions)
            .ThenInclude(q => q.ExerciseAnswers)
        .FirstOrDefaultAsync(e => e.ExerciseId == exerciseId);
        }

        public async Task<ExerciseHistory> AddExerciseHistoryAsync(ExerciseHistory exerciseHistory)
        {
            _context.ExerciseHistories.Add(exerciseHistory);
            await _context.SaveChangesAsync();
            return exerciseHistory;
        }

        public async Task<List<Exercise>> GetExercisesInRoomAsync(int roomId)
        {
            return await _context.Exercises
        .Where(e => e.OnlineRoomId == roomId)
        .Include(e => e.ExerciseQuestions)
        .ToListAsync();
        }

        public async Task<List<ExerciseHistory>> GetUserExerciseHistoriesAsync(string userId, int exerciseId)
        {
            return await _context.ExerciseHistories
                .Where(eh => eh.UserId == userId && eh.ExerciseId == exerciseId)
                .Include(eh => eh.Exercise) // Fix chi tiết bài tập
                .ToListAsync();
        }
        public async Task<List<ExerciseQuestion>> GetQuestionsWithAnswersAsync(int exerciseId)
        {
            return await _context.ExerciseQuestions
                .Where(q => q.ExerciseId == exerciseId)
                .Include(q => q.ExerciseAnswers)
                .ToListAsync();
        }

    


    }
}
