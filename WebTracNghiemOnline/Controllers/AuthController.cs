using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebTracNghiemOnline.DTO;
using WebTracNghiemOnline.Repository;
using WebTracNghiemOnline.Services;

namespace WebTracNghiemOnline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;
        public AuthController(IAuthService authService, IUserRepository userRepository)
        {
            _authService = authService;
            _userRepository = userRepository;
        }
        

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto model)
        {
            var result = await _authService.RegisterAsync(model);
            if (result == "User registered successfully.")
                return Ok(new { message = result });

            return BadRequest(new { message = result });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDto model)
        {
            var token = await _authService.LoginAsync(model);
            if (token == null)
                return Unauthorized(new { message = "Email hoặc mật khẩu không đúng." });

            // Gửi token qua cookie
            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None, // Nếu chạy localhost:5173 và localhost:7253
                Secure = true, // Sử dụng HTTPS
                Expires = DateTime.UtcNow.AddMinutes(60)
            });

            return Ok(new { token }); // Trả token trong response body (nếu cần test qua header)
        }

        /* [HttpGet("me")]
         public async Task<IActionResult> GetCurrentUser()
         {
             try
             {
                 // Lấy token từ cookie
                 var token = Request.Cookies["jwt"];
                 Console.WriteLine(token);
                 if (string.IsNullOrEmpty(token))
                     return Unauthorized(new { message = "Không tìm thấy token. Vui lòng đăng nhập lại." });

                 // Giải mã token
                 var handler = new JwtSecurityTokenHandler();
                 var jwtToken = handler.ReadJwtToken(token);

                 // Kiểm tra Issuer và Audience
                 if (jwtToken.Issuer != "Dung" || !jwtToken.Audiences.Contains("client"))
                     return Unauthorized(new { message = "Token không hợp lệ. Vui lòng đăng nhập lại." });

                 foreach (var claim in jwtToken.Claims)
                 {
                     Console.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
                 }

                 // Lấy UserId từ Claim (nameid)
                 var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
                 if (string.IsNullOrEmpty(userId))
                     return Unauthorized(new { message = "Token không hợp lệ." });

                 // Tìm thông tin người dùng từ database
                 var user = await _authService.GetUserByIdAsync(userId);
                 if (user == null)
                     return Unauthorized(new { message = "Không tìm thấy người dùng. Vui lòng đăng nhập lại." });

                 // Trả về thông tin người dùng
                 return Ok(new
                 {
                     email = user.Email,
                     fullName = user.FullName,
                     balance = user.Balance
                 });
             }
             catch (Exception ex)
             {
                 Console.WriteLine("Lỗi khi xử lý token: " + ex.Message);
                 return StatusCode(500, new { message = "Có lỗi xảy ra.", details = ex.Message });
             }
         }*/

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var token = Request.Cookies["jwt"];
                if (string.IsNullOrEmpty(token))
                    return Unauthorized(new { message = "Không tìm thấy token. Vui lòng đăng nhập lại." });

                var user = await _authService.ValidateTokenAsync(token);
                var roles = await _userRepository.GetRolesAsync(user);

                return Ok(new
                {
                    email = user.Email,
                    fullName = user.FullName,
                    balance = user.Balance,
                    roles // Trả về vai trò
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra.", details = ex.Message });
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Append("jwt", "", new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
                Expires = DateTime.UtcNow.AddDays(-1) // Xóa cookie
            });
            return Ok(new { message = "Logged out successfully." });
        }

    }
}
