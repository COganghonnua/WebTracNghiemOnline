using WebTracNghiemOnline.Exceptions;
using WebTracNghiemOnline.Models;
using WebTracNghiemOnline.Repository;

namespace WebTracNghiemOnline.Service
{

    public interface IOnlineRoomService
    {
        Task<OnlineRoom> CreateRoomAsync(string hostUserId, string roomName);
        Task<UserOnlineRoom> JoinRoomAsync(string userId, string roomCode);
        Task<bool> LeaveRoomAsync(string userId, string roomCode);
        Task<OnlineRoom?> GetRoomByCodeAsync(string roomCode);
    }

    public class OnlineRoomService : IOnlineRoomService
    {
        private readonly IOnlineRoomRepository _repository;

        public OnlineRoomService(IOnlineRoomRepository repository)
        {
            _repository = repository;
        }

        public async Task<OnlineRoom> CreateRoomAsync(string hostUserId, string roomName)
        {
            // Generate unique room code
            string roomCode;
            do
            {
                roomCode = GenerateRoomCode();
            } while (await _repository.GetRoomByCodeAsync(roomCode) != null);

            // Create new room
            var room = new OnlineRoom
            {
                RoomCode = roomCode,
                RoomName = roomName, // Sử dụng roomName từ người dùng
                CreatedAt = DateTime.UtcNow
            };

            var createdRoom = await _repository.CreateRoomAsync(room);

            // Add owner to the room
            var ownerEntry = new UserOnlineRoom
            {
                UserId = hostUserId,
                OnlineRoomId = createdRoom.OnlineRoomId,
                Role = UserRole.Owner,
                JoinedAt = DateTime.UtcNow
            };
            await _repository.AddUserToRoomAsync(ownerEntry);

            return createdRoom;
        }


        public async Task<UserOnlineRoom> JoinRoomAsync(string userId, string roomCode)
        {
            // Find room by code
            var room = await _repository.GetRoomByCodeAsync(roomCode);
            if (room == null)
                throw new RoomNotFoundException();

            // Check if user is already in the room
            if (await _repository.IsUserInRoomAsync(userId, room.OnlineRoomId))
                throw new UserAlreadyInRoomException();

            // Add user to room
            var userOnlineRoom = new UserOnlineRoom
            {
                UserId = userId,
                OnlineRoomId = room.OnlineRoomId,
                Role = UserRole.Member,
                JoinedAt = DateTime.UtcNow
            };

            return await _repository.AddUserToRoomAsync(userOnlineRoom);
        }



        private string GenerateRoomCode()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
        }

        public async Task<bool> LeaveRoomAsync(string userId, string roomCode)
        {
            var room = await _repository.GetRoomByCodeAsync(roomCode);
            if (room == null)
                throw new RoomNotFoundException();

            return await _repository.LeaveRoomAsync(userId, room.OnlineRoomId);
        }
        public async Task<OnlineRoom?> GetRoomByCodeAsync(string roomCode)
        {
            return await _repository.GetRoomByCodeAsync(roomCode);
        }

    }
}
