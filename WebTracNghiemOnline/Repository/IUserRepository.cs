using Microsoft.AspNetCore.Identity;
using WebTracNghiemOnline.Models;

namespace WebTracNghiemOnline.Repository
{
    public interface IUserRepository
    {
        Task<User> FindByEmailandUsernameAsync(string email,string username);
        Task<bool> CreateAsync(User user, string password);
        Task<User> FindByEmaiAsync(string email);
        Task<IdentityResult> AddToRoleAsync(User user, string v);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<IList<string>> GetRolesAsync(User user);
        Task<User> GetByIdAsync(string userId);
    }
}
