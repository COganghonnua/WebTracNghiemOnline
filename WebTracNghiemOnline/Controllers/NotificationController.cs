using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using WebTracNghiemOnline.DTO;
using WebTracNghiemOnline.Service;
using WebTracNghiemOnline.Services;

namespace WebTracNghiemOnline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IAuthService _authService;

        public NotificationController(INotificationService notificationService, IAuthService authService)
        {
            _notificationService = notificationService;
            _authService = authService;
        }

        [HttpPost("send-to-class")]
        public async Task<IActionResult> SendToClass([FromBody] NotificationRequestDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Content))
            {
                return BadRequest(new { message = "Content is required." });
            }

            try
            {
                await _notificationService.SendNotificationToClassAsync(dto.RoomId, dto.Content);
                return Ok(new { message = "Notification sent successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUserNotifications()
        {
            var token = Request.Cookies["jwt"];
            if (string.IsNullOrEmpty(token))
                return Unauthorized();

            var user = await _authService.ValidateTokenAsync(token);
            var notifications = await _notificationService.GetUserNotificationsAsync(user.Id);

            return Ok(notifications);
        }

        [HttpPost("mark-as-read/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkNotificationAsReadAsync(id);
            return Ok(new { message = "Notification marked as read." });
        }
    }

}
