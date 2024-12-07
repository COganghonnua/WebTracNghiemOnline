using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebTracNghiemOnline.DTO;
using WebTracNghiemOnline.Exceptions;
using WebTracNghiemOnline.Service;
using WebTracNghiemOnline.Services;

namespace WebTracNghiemOnline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OnlineRoomController : ControllerBase
    {
        private readonly IOnlineRoomService _onlineRoomService;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public OnlineRoomController(IOnlineRoomService onlineRoomService, IAuthService authService,IMapper mapper)
        {
            _onlineRoomService = onlineRoomService;
            _authService = authService;
            _mapper = mapper;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomRequest request)
        {
            try
            {
                var token = Request.Cookies["jwt"];
                if (string.IsNullOrEmpty(token))
                    return Unauthorized(new { message = "Token not found. Please log in." });

                var user = await _authService.ValidateTokenAsync(token);
                var room = await _onlineRoomService.CreateRoomAsync(user.Id, request.RoomName);

                var roomDto = _mapper.Map<OnlineRoomDto>(room);
                return Ok(new { message = "Room created successfully", room = roomDto });
            }
            catch (RoomNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UserAlreadyInRoomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }



        [HttpPost("join")]
        public async Task<IActionResult> JoinRoom([FromBody] JoinRoomRequest request)
        {
            try
            {
                var token = Request.Cookies["jwt"];
                if (string.IsNullOrEmpty(token))
                    return Unauthorized(new { message = "Token not found. Please log in." });

                var user = await _authService.ValidateTokenAsync(token);
                var userRoom = await _onlineRoomService.JoinRoomAsync(user.Id, request.RoomCode);

                var room = await _onlineRoomService.GetRoomByCodeAsync(request.RoomCode); // Lấy thông tin phòng
                var userRoomDto = _mapper.Map<UserOnlineRoomDto>(userRoom);

                return Ok(new
                {
                    message = "Joined room successfully",
                    userRoom = userRoomDto,
                    roomName = room.RoomName // Thêm tên phòng vào response
                });
            }
            catch (RoomNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UserAlreadyInRoomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("leave")]
        public async Task<IActionResult> LeaveRoom([FromBody] JoinRoomRequest request)
        {
            try
            {
                var token = Request.Cookies["jwt"];
                if (string.IsNullOrEmpty(token))
                    return Unauthorized(new { message = "Token not found. Please log in." });

                var user = await _authService.ValidateTokenAsync(token);
                var success = await _onlineRoomService.LeaveRoomAsync(user.Id, request.RoomCode);

                if (!success) return NotFound(new { message = "User not in the room" });

                return Ok(new { message = "Left room successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }

    public class JoinRoomRequest
    {
        public string RoomCode { get; set; }
    }

}
