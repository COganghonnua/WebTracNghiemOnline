using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebTracNghiemOnline.DTO;
using WebTracNghiemOnline.Models;
using WebTracNghiemOnline.Configuration;
using WebTracNghiemOnline.Repository;
using AutoMapper;
using Microsoft.Extensions.Options;
using WebTracNghiemOnline.Access;

namespace WebTracNghiemOnline.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterUserDto model);
        Task<string> LoginAsync(LoginUserDto model);
        Task<User> GetUserByIdAsync(string userId);
    }
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtConfig _jwtConfig;
        private readonly IMapper _mapper;

        public AuthService(IUserRepository userRepository, IOptions<JwtConfig> jwtConfig, IMapper mapper)
        {
            _userRepository = userRepository;
            _jwtConfig = jwtConfig.Value;
            _mapper = mapper;
        }

        public async Task<string> RegisterAsync(RegisterUserDto model)
        {
            var userExists = await _userRepository.FindByEmailandUsernameAsync(model.Email, model.Username);
            if (userExists != null)
                return "User with this email or username already exists.";

            var user = _mapper.Map<User>(model);
            var result = await _userRepository.CreateAsync(user, model.Password);

            if (!result)
                return "Failed to create user.";

            await _userRepository.AddToRoleAsync(user, "User");
            return "User registered successfully.";
        }

        public async Task<string> LoginAsync(LoginUserDto model)
        {
            var user = await _userRepository.FindByEmaiAsync(model.Email);
            if (user == null || !await _userRepository.CheckPasswordAsync(user, model.Password))
                return null;

            return GenerateJwtToken(user);
        }
        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }
        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.Email)
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtConfig.ExpiresInMinutes),
                Issuer = _jwtConfig.Issuer,
                Audience = _jwtConfig.Audience,
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }

    }
}
