using Microsoft.Extensions.Logging;
using Auth.Core.Abstractions;
using Auth.Core.Models;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace Auth.Core.Services
{
    /// <summary>
    /// Service de gestion des utilisateurs.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<UserService> _logger;
        private readonly PasswordPolicy _passwordPolicy;

        public UserService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ILogger<UserService> logger,
            IOptions<PasswordPolicy> passwordPolicy)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
            _passwordPolicy = passwordPolicy.Value;
        }

        public async Task<ApplicationUser?> GetByIdAsync(string userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<ApplicationUser?> GetByUsernameAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username);
        }

        public async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<ApplicationUser> CreateUserAsync(ApplicationUser user, string password)
        {
            // Validation
            if (await _userRepository.ExistsByEmailAsync(user.Email))
                throw new InvalidOperationException("Cet email est déjà utilisé");

            if (await _userRepository.ExistsByUsernameAsync(user.Username))
                throw new InvalidOperationException("Ce nom d'utilisateur est déjà utilisé");

            // Hash du mot de passe
            user.PasswordHash = _passwordHasher.Hash(password);

            // Token de confirmation d'email
            user.EmailConfirmationToken = GenerateSecureToken();
            user.EmailConfirmationTokenExpires = DateTime.UtcNow.AddHours(24);

            // Création
            var createdUser = await _userRepository.CreateAsync(user);

            _logger.LogInformation("Nouvel utilisateur créé: {Username}", user.Username);

            return createdUser;
        }

        public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            // Vérifier l'ancien mot de passe
            if (!_passwordHasher.Verify(currentPassword, user.PasswordHash))
                return false;

            // Changer le mot de passe
            user.PasswordHash = _passwordHasher.Hash(newPassword);
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("Mot de passe changé pour l'utilisateur {UserId}", userId);

            return true;
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                // SÉCURITÉ: Ne pas révéler si l'email existe
                _logger.LogWarning("Tentative de reset pour email inexistant: {Email}", email);
                return string.Empty;
            }

            var token = GenerateSecureToken();
            user.PasswordResetToken = token;
            user.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(1); // 1 heure

            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("Token de reset généré pour {Email}", email);

            return token;
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            var user = await _userRepository.GetByPasswordResetTokenAsync(token);

            if (user == null ||
                !user.PasswordResetTokenExpires.HasValue ||
                user.PasswordResetTokenExpires < DateTime.UtcNow)
            {
                return false;
            }

            // Réinitialiser le mot de passe
            user.PasswordHash = _passwordHasher.Hash(newPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpires = null;
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;

            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("Mot de passe réinitialisé pour l'utilisateur {UserId}", user.Id);

            return true;
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("Utilisateur non trouvé");

            var token = GenerateSecureToken();
            user.EmailConfirmationToken = token;
            user.EmailConfirmationTokenExpires = DateTime.UtcNow.AddHours(24);

            await _userRepository.UpdateAsync(user);

            return token;
        }

        public async Task<bool> ConfirmEmailAsync(string token)
        {
            var user = await _userRepository.GetByEmailConfirmationTokenAsync(token);

            if (user == null ||
                !user.EmailConfirmationTokenExpires.HasValue ||
                user.EmailConfirmationTokenExpires < DateTime.UtcNow)
            {
                return false;
            }

            user.EmailConfirmed = true;
            user.EmailConfirmationToken = null;
            user.EmailConfirmationTokenExpires = null;

            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("Email confirmé pour l'utilisateur {UserId}", user.Id);

            return true;
        }

        public async Task LockAccountAsync(string userId, TimeSpan duration)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return;

            user.LockoutEnd = DateTime.UtcNow.Add(duration);
            await _userRepository.UpdateAsync(user);

            _logger.LogWarning("Compte verrouillé pour {Duration} minutes: {UserId}",
                duration.TotalMinutes, userId);
        }

        public async Task UnlockAccountAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return;

            user.LockoutEnd = null;
            user.FailedLoginAttempts = 0;
            await _userRepository.UpdateAsync(user);
        }

        public async Task IncrementFailedLoginAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return;

            user.FailedLoginAttempts++;

            // Verrouiller si trop de tentatives
            if (user.FailedLoginAttempts >= _passwordPolicy.MaxFailedAttempts)
            {
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(_passwordPolicy.LockoutDurationMinutes);
                _logger.LogWarning("Compte verrouillé après {Attempts} tentatives: {UserId}",
                    user.FailedLoginAttempts, userId);
            }

            await _userRepository.UpdateAsync(user);
        }

        public async Task ResetFailedLoginAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return;

            user.FailedLoginAttempts = 0;
            await _userRepository.UpdateAsync(user);
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            return !await _userRepository.ExistsByEmailAsync(email);
        }

        public async Task<bool> IsUsernameUniqueAsync(string username)
        {
            return !await _userRepository.ExistsByUsernameAsync(username);
        }

        /// <summary>
        /// Génère un token sécurisé de 32 bytes.
        /// </summary>
        private static string GenerateSecureToken()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public async Task<List<ApplicationUser>> GetAllAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public Task UpdateAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }


        // ═══════════════════════════════════════════════════════
        // STATISTIQUES
        // ═══════════════════════════════════════════════════════

        public async Task<int> GetTotalUsersCountAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Count;
        }

        public async Task<int> GetActiveUsersCountAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Count(u => !u.IsLockedOut && u.EmailConfirmed);
        }

        public async Task<int> GetNewUsersThisWeekCountAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var oneWeekAgo = DateTime.UtcNow.AddDays(-7);
            return users.Count(u => u.CreatedAt >= oneWeekAgo);
        }

        public async Task<int> GetPendingEmailConfirmationCountAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Count(u => !u.EmailConfirmed);
        }

        public async Task<int> GetLockedOutUsersCountAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Count(u => u.IsLockedOut);
        }

        public async Task<List<ApplicationUser>> GetRecentUsersAsync(int count = 10)
        {
            var users = await _userRepository.GetAllAsync();
            return users
                .OrderByDescending(u => u.CreatedAt)
                .Take(count)
                .ToList();
        }
    }
}
