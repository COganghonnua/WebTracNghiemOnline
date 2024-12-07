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


    }
}
