using System.ComponentModel.DataAnnotations;

namespace TesterLab.Models.ViewModels
{
  public class UserSettingsViewModel
  {
    public PersonalInfoViewModel PersonalInfo { get; set; } = new();
    public ChangePasswordViewModel ChangePassword { get; set; } = new();
    public SecuritySettingsViewModel Security { get; set; } = new();
    public NotificationSettingsViewModel Notifications { get; set; } = new();
  }

  // ═══════════════════════════════════════════════════════
  // INFORMATIONS PERSONNELLES
  // ═══════════════════════════════════════════════════════

  public class PersonalInfoViewModel
  {
    public string UserId { get; set; } = string.Empty;

    [Display(Name = "Nom d'utilisateur")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'email est requis")]
    [EmailAddress(ErrorMessage = "Email invalide")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [StringLength(50)]
    [Display(Name = "Prénom")]
    public string? FirstName { get; set; }

    [StringLength(50)]
    [Display(Name = "Nom")]
    public string? LastName { get; set; }

    public bool EmailConfirmed { get; set; }
  }

  // ═══════════════════════════════════════════════════════
  // CHANGEMENT DE MOT DE PASSE
  // ═══════════════════════════════════════════════════════

  public class ChangePasswordViewModel
  {
    [Required(ErrorMessage = "Le mot de passe actuel est requis")]
    [DataType(DataType.Password)]
    [Display(Name = "Mot de passe actuel")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le nouveau mot de passe est requis")]
    [StringLength(100, MinimumLength = 12, ErrorMessage = "Le mot de passe doit contenir au moins 12 caractères")]
    [DataType(DataType.Password)]
    [Display(Name = "Nouveau mot de passe")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "La confirmation est requise")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Les mots de passe ne correspondent pas")]
    [Display(Name = "Confirmer le mot de passe")]
    public string ConfirmPassword { get; set; } = string.Empty;
  }

  // ═══════════════════════════════════════════════════════
  // SÉCURITÉ
  // ═══════════════════════════════════════════════════════

  public class SecuritySettingsViewModel
  {
    public bool TwoFactorEnabled { get; set; }
    public int ActiveSessionsCount { get; set; }
    public DateTime? LastPasswordChange { get; set; }
    public List<ActiveSessionViewModel> ActiveSessions { get; set; } = new();
  }

  public class ActiveSessionViewModel
  {
    public string Id { get; set; } = string.Empty;
    public string DeviceInfo { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public DateTime LastActivity { get; set; }
    public bool IsCurrent { get; set; }
  }

  // ═══════════════════════════════════════════════════════
  // NOTIFICATIONS
  // ═══════════════════════════════════════════════════════

  public class NotificationSettingsViewModel
  {
    [Display(Name = "Notifications par email")]
    public bool EmailNotifications { get; set; }

    [Display(Name = "Nouveautés et mises à jour")]
    public bool NewsletterEnabled { get; set; }

    [Display(Name = "Alertes de sécurité")]
    public bool SecurityAlerts { get; set; } = true; // Toujours activé par défaut
  }
}
