using Auth.Core.Models;

namespace Auth.Core.Abstractions
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(string userId);
        Task<ApplicationUser?> GetByUsernameAsync(string username);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<ApplicationUser?> GetByPasswordResetTokenAsync(string token);
        Task<ApplicationUser?> GetByEmailConfirmationTokenAsync(string token);

        Task<ApplicationUser> CreateAsync(ApplicationUser user);
        Task UpdateAsync(ApplicationUser user);
        Task DeleteAsync(string userId);

        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByUsernameAsync(string username);

        Task<List<ApplicationUser>> GetAllAsync();
    }
}
