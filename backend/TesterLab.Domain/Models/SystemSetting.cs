using System.ComponentModel.DataAnnotations;

namespace TesterLab.Domain.Models
{
    public class SystemSetting
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Key { get; set; } = string.Empty;

        [Required]
        public string Value { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string Category { get; set; } = "General"; // General, Email, Testing, Security, Branding

        [MaxLength(50)]
        public string DataType { get; set; } = "String"; // String, Integer, Boolean, Json

        public bool IsEncrypted { get; set; } = false;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string? UpdatedBy { get; set; }
    }

    // ═══════════════════════════════════════════════════════
    // MODÈLE FORTEMENT TYPÉ POUR L'APPLICATION
    // ═══════════════════════════════════════════════════════

    public class SystemSettingsViewModel
    {
        public GeneralSettings General { get; set; } = new();
        public EmailSettings Email { get; set; } = new();
        public TestingSettings Testing { get; set; } = new();
        public SecuritySettings Security { get; set; } = new();
        public BrandingSettings Branding { get; set; } = new();
        public NotificationSettings Notifications { get; set; } = new();
        public StorageSettings Storage { get; set; } = new();
    }

    // ═══════════════════════════════════════════════════════
    // PARAMÈTRES GÉNÉRAUX
    // ═══════════════════════════════════════════════════════

    public class GeneralSettings
    {
        [Display(Name = "Nom de l'application")]
        public string ApplicationName { get; set; } = "TesterLab";

        [Display(Name = "URL de base")]
        [Url]
        public string BaseUrl { get; set; } = "http://localhost";

        [Display(Name = "Fuseau horaire")]
        public string TimeZone { get; set; } = "UTC";

        [Display(Name = "Langue par défaut")]
        public string DefaultLanguage { get; set; } = "fr-FR";

        [Display(Name = "Maintenance activée")]
        public bool MaintenanceMode { get; set; } = false;

        [Display(Name = "Message de maintenance")]
        public string? MaintenanceMessage { get; set; }

        [Display(Name = "Autoriser les inscriptions")]
        public bool AllowRegistration { get; set; } = true;

        [Display(Name = "Confirmation email obligatoire")]
        public bool RequireEmailConfirmation { get; set; } = true;
    }

    // ═══════════════════════════════════════════════════════
    // PARAMÈTRES EMAIL (SMTP)
    // ═══════════════════════════════════════════════════════

    public class EmailSettings
    {
        [Display(Name = "Serveur SMTP")]
        public string SmtpHost { get; set; } = "smtp.gmail.com";

        [Display(Name = "Port SMTP")]
        public int SmtpPort { get; set; } = 587;

        [Display(Name = "Utiliser SSL")]
        public bool EnableSsl { get; set; } = true;

        [Display(Name = "Nom d'utilisateur SMTP")]
        public string SmtpUsername { get; set; } = string.Empty;

        [Display(Name = "Mot de passe SMTP")]
        [DataType(DataType.Password)]
        public string SmtpPassword { get; set; } = string.Empty;

        [Display(Name = "Email expéditeur")]
        [EmailAddress]
        public string FromEmail { get; set; } = "noreply@testerlab.com";

        [Display(Name = "Nom de l'expéditeur")]
        public string FromName { get; set; } = "TesterLab";

        [Display(Name = "Activer les emails")]
        public bool EmailEnabled { get; set; } = true;
    }

    // ═══════════════════════════════════════════════════════
    // PARAMÈTRES DE TEST
    // ═══════════════════════════════════════════════════════

    public class TestingSettings
    {
        [Display(Name = "Timeout par défaut (secondes)")]
        [Range(1, 300)]
        public int DefaultTimeoutSeconds { get; set; } = 30;

        [Display(Name = "Nombre max de tentatives")]
        [Range(1, 10)]
        public int MaxRetries { get; set; } = 3;

        [Display(Name = "Navigateur par défaut")]
        public string DefaultBrowser { get; set; } = "Chrome";

        [Display(Name = "Mode headless par défaut")]
        public bool DefaultHeadlessMode { get; set; } = true;

        [Display(Name = "Capturer screenshots sur échec")]
        public bool CaptureScreenshotOnFailure { get; set; } = true;

        [Display(Name = "Capturer screenshots sur succès")]
        public bool CaptureScreenshotOnSuccess { get; set; } = false;

        [Display(Name = "Conserver les logs (jours)")]
        [Range(1, 365)]
        public int RetentionDays { get; set; } = 30;

        [Display(Name = "Exécutions parallèles max")]
        [Range(1, 20)]
        public int MaxParallelExecutions { get; set; } = 5;

        [Display(Name = "Auto-retry sur échec")]
        public bool AutoRetryOnFailure { get; set; } = true;
    }

    // ═══════════════════════════════════════════════════════
    // PARAMÈTRES DE SÉCURITÉ
    // ═══════════════════════════════════════════════════════

    public class SecuritySettings
    {
        [Display(Name = "Longueur minimum du mot de passe")]
        [Range(8, 128)]
        public int MinPasswordLength { get; set; } = 12;

        [Display(Name = "Exiger majuscules")]
        public bool RequireUppercase { get; set; } = true;

        [Display(Name = "Exiger minuscules")]
        public bool RequireLowercase { get; set; } = true;

        [Display(Name = "Exiger chiffres")]
        public bool RequireDigit { get; set; } = true;

        [Display(Name = "Exiger caractères spéciaux")]
        public bool RequireNonAlphanumeric { get; set; } = true;

        [Display(Name = "Tentatives max avant verrouillage")]
        [Range(3, 10)]
        public int MaxFailedLoginAttempts { get; set; } = 5;

        [Display(Name = "Durée de verrouillage (minutes)")]
        [Range(1, 1440)]
        public int LockoutDurationMinutes { get; set; } = 15;

        [Display(Name = "Expiration des sessions (minutes)")]
        [Range(5, 1440)]
        public int SessionTimeoutMinutes { get; set; } = 720; // 12 heures

        [Display(Name = "Forcer 2FA pour les admins")]
        public bool Force2FAForAdmins { get; set; } = false;

        [Display(Name = "Limiter les connexions par IP")]
        public bool EnableIpRateLimit { get; set; } = true;

        [Display(Name = "Requêtes max par heure (IP)")]
        [Range(10, 1000)]
        public int IpRateLimitPerHour { get; set; } = 100;
    }

    // ═══════════════════════════════════════════════════════
    // PARAMÈTRES DE BRANDING
    // ═══════════════════════════════════════════════════════

    public class BrandingSettings
    {
        [Display(Name = "Logo (URL)")]
        public string? LogoUrl { get; set; }

        [Display(Name = "Favicon (URL)")]
        public string? FaviconUrl { get; set; }

        [Display(Name = "Couleur primaire")]
        public string PrimaryColor { get; set; } = "#007bff";

        [Display(Name = "Couleur secondaire")]
        public string SecondaryColor { get; set; } = "#6c757d";

        [Display(Name = "Pied de page personnalisé")]
        public string? CustomFooter { get; set; }

        [Display(Name = "Texte de bienvenue")]
        public string? WelcomeMessage { get; set; }

        [Display(Name = "Afficher le nom de l'entreprise")]
        public bool ShowCompanyName { get; set; } = true;

        [Display(Name = "Nom de l'entreprise")]
        public string? CompanyName { get; set; }
    }

    // ═══════════════════════════════════════════════════════
    // PARAMÈTRES DE NOTIFICATIONS
    // ═══════════════════════════════════════════════════════

    public class NotificationSettings
    {
        [Display(Name = "Webhook Slack")]
        [Url]
        public string? SlackWebhookUrl { get; set; }

        [Display(Name = "Webhook Teams")]
        [Url]
        public string? TeamsWebhookUrl { get; set; }

        [Display(Name = "Webhook Discord")]
        [Url]
        public string? DiscordWebhookUrl { get; set; }

        [Display(Name = "Notifier les échecs de tests")]
        public bool NotifyOnTestFailure { get; set; } = true;

        [Display(Name = "Notifier les succès de tests")]
        public bool NotifyOnTestSuccess { get; set; } = false;

        [Display(Name = "Notifier les exécutions planifiées")]
        public bool NotifyOnScheduledRuns { get; set; } = true;

        [Display(Name = "Taux de réussite minimum pour notification")]
        [Range(0, 100)]
        public int MinSuccessRateForNotification { get; set; } = 90;
    }

    // ═══════════════════════════════════════════════════════
    // PARAMÈTRES DE STOCKAGE
    // ═══════════════════════════════════════════════════════

    public class StorageSettings
    {
        [Display(Name = "Chemin des screenshots")]
        public string ScreenshotPath { get; set; } = "wwwroot/screenshots";

        [Display(Name = "Chemin des rapports")]
        public string ReportsPath { get; set; } = "wwwroot/reports";

        [Display(Name = "Taille max des fichiers (MB)")]
        [Range(1, 100)]
        public int MaxFileSizeMB { get; set; } = 10;

        [Display(Name = "Nettoyer automatiquement les anciens fichiers")]
        public bool AutoCleanupEnabled { get; set; } = true;

        [Display(Name = "Conserver les fichiers (jours)")]
        [Range(1, 365)]
        public int FileRetentionDays { get; set; } = 30;

        [Display(Name = "Utiliser le stockage cloud")]
        public bool UseCloudStorage { get; set; } = false;

        [Display(Name = "Type de stockage cloud")]
        public string CloudStorageProvider { get; set; } = "None"; // None, Azure, AWS, GCP

        [Display(Name = "Conteneur/Bucket")]
        public string? CloudStorageContainer { get; set; }
    }
}
