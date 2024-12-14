using AutoMapper;
using WebTracNghiemOnline.DTO;
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
        Task<Exercise> CreateExerciseAsync(string userId, int roomId, CreateExerciseDto request);
        Task<List<OnlineRoom>> GetUserRoomsAsync(string userId);

    }

    public class OnlineRoomService : IOnlineRoomService
    {
        private readonly IOnlineRoomRepository _repository;
        private readonly IMapper _mapper;


        public OnlineRoomService(IOnlineRoomRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
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
        public async Task<List<OnlineRoom>> GetUserRoomsAsync(string userId)
        {
            return await _repository.GetUserRoomsAsync(userId);
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


        public async Task<Exercise> CreateExerciseAsync(string userId, int roomId, CreateExerciseDto request)
        {
            // Kiểm tra người dùng có trong phòng và có vai trò là Owner không
            var userRoom = await _repository.GetUserInRoomAsync(userId, roomId);
            if (userRoom == null || userRoom.Role != UserRole.Owner)
            {
                throw new UnauthorizedAccessException("Only the owner can create exercises.");
            }

            // Kiểm tra thời gian bắt đầu và kết thúc
            if (request.EndTime <= request.StartTime)
            {
                throw new ArgumentException("The end time must be greater than the start time of the exercise.");
            }

            // Ánh xạ DTO thành Entity
            var exercise = _mapper.Map<Exercise>(request);
            exercise.OnlineRoomId = roomId;

            return await _repository.AddExerciseAsync(exercise);
        }

    }
}
