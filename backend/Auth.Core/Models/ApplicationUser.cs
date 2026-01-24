namespace Auth.Core.Models
{
    /// <summary>
    /// Entité utilisateur complète pour l'application.
    /// </summary>
    public class ApplicationUser
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        // Confirmation d'email
        public bool EmailConfirmed { get; set; }
        public string? EmailConfirmationToken { get; set; }
        public DateTime? EmailConfirmationTokenExpires { get; set; }

        // Réinitialisation de mot de passe
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpires { get; set; }

        // Sécurité
        public int FailedLoginAttempts { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public bool IsLockedOut => LockoutEnd.HasValue && LockoutEnd > DateTime.UtcNow;

        // Audit
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public string? LastLoginIp { get; set; }

        // ✅ ROLES - Navigation property
        public List<UserRole> UserRoles { get; set; } = new();
        public string? AvatarUrl { get; set; }
        // ✅ Helper pour récupérer les noms de rôles facilement
        public List<string> GetRoleNames() => UserRoles.Select(ur => ur.Role.Name).ToList();
    }
}
