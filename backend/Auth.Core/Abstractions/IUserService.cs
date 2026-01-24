using Auth.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Core.Abstractions
{
    /// <summary>
    /// Service de gestion des utilisateurs.
    /// </summary>
    public interface IUserService
    {
        // Création et récupération
        Task<ApplicationUser?> GetByIdAsync(string userId);
        Task<ApplicationUser?> GetByUsernameAsync(string username);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<ApplicationUser> CreateUserAsync(ApplicationUser user, string password);

        // Gestion du mot de passe
        Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<string> GeneratePasswordResetTokenAsync(string email);
        Task<bool> ResetPasswordAsync(string token, string newPassword);

        // Confirmation d'email
        Task<string> GenerateEmailConfirmationTokenAsync(string userId);
        Task<bool> ConfirmEmailAsync(string token);

        // Verrouillage du compte
        Task LockAccountAsync(string userId, TimeSpan duration);
        Task UnlockAccountAsync(string userId);
        Task IncrementFailedLoginAsync(string userId);
        Task ResetFailedLoginAsync(string userId);

        // Validation
        Task<bool> IsEmailUniqueAsync(string email);
        Task<bool> IsUsernameUniqueAsync(string username);
        Task<List<ApplicationUser>> GetAllAsync();
        Task UpdateAsync(ApplicationUser user);

        // Statistiques
        Task<int> GetTotalUsersCountAsync();
        Task<int> GetActiveUsersCountAsync();
        Task<int> GetNewUsersThisWeekCountAsync();
        Task<int> GetPendingEmailConfirmationCountAsync();
        Task<int> GetLockedOutUsersCountAsync();
        Task<List<ApplicationUser>> GetRecentUsersAsync(int count = 10);
    }
}
