using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebTracNghiemOnline.Models;

namespace WebTracNghiemOnline.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;

        public UserRepository(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<User> FindByEmailandUsernameAsync(string email, string username)
        {
            // Kiểm tra email
            var accountByEmail = await _userManager.FindByEmailAsync(email);
            if (accountByEmail != null && accountByEmail.UserName == username)
            {
                return accountByEmail; // Nếu cả email và username đều khớp
            }

            // Kiểm tra username
            var accountByUsername = await _userManager.FindByNameAsync(username);
            if (accountByUsername != null && accountByUsername.Email == email)
            {
                return accountByUsername; // Nếu cả username và email đều khớp
            }

            return null; 

        }

        public async Task<bool> CreateAsync(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            return result.Succeeded;
        }

        public async Task<User> FindByEmaiAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
        public async Task<IdentityResult> AddToRoleAsync(User user, string role)
        {
            return await _userManager.AddToRoleAsync(user, role);

        }
        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }
        public async Task<IList<string>> GetRolesAsync(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }
        public async Task<User> GetByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }
        public async Task<bool> UpdateAsync(User user)
        {
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                Console.WriteLine("Failed to update user:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"Code: {error.Code}, Description: {error.Description}");
                }
            }

            return result.Succeeded;
        }

    }
}
