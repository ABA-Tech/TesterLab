using System.ComponentModel.DataAnnotations;

namespace TesterLab.Domain.Models
{

    public class Application
    {
        public int Id { get; set; }

        [Required, Display(Name = "Nom de l'application")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "URL principale")]
        [Url]
        public string? MainUrl { get; set; }

        [Display(Name = "Type d'application")]
        public string AppType { get; set; } = "Web"; // Web, Mobile, API, Desktop

        //[Display(Name = "Secteur d'activité")]
        //public string? Industry { get; set; } // E-commerce, Banking, Education, etc.

        [Display(Name = "Actif")]
        public bool Active { get; set; } = true;

        //[Display(Name = "Couleur du thème")]
        //public string? ThemeColor { get; set; } = "#007bff";

        public bool Selected { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relations
        public List<Feature> Features { get; set; } = new();
        public List<Environment> Environments { get; set; } = new();
        public List<TestData> TestDataSets { get; set; } = new();
        public DateTime UpdatedAt { get; set; }
    }
    public class Feature
    {
        public int Id { get; set; }

        [Required, Display(Name = "Application")]
        public int ApplicationId { get; set; }

        [Required, Display(Name = "Nom de la fonctionnalité")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Icône")]
        public string? Icon { get; set; } = "🔧"; // Emoji par défaut

        [Display(Name = "Priorité métier")]
        [Range(1, 5)]
        public int BusinessPriority { get; set; } = 3;

        [Display(Name = "Complexité")]
        public string Complexity { get; set; } = "Medium"; // Low, Medium, High

        [Display(Name = "Actif")]
        public bool Active { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relations
        public Application? Application { get; set; }
        public List<TestCase> TestCases { get; set; } = new();
        public DateTime UpdatedAt { get; set; }
    }

    public class TestCase
    {
        public int Id { get; set; }

        [Required, Display(Name = "Nom du scénario")]
        public string Name { get; set; }

        [Display(Name = "Description (en langage naturel)")]
        public string? Description { get; set; }

        [Required, Display(Name = "Fonctionnalité")]
        public int FeatureId { get; set; }

        [Display(Name = "Niveau de criticité")]
        [Range(1, 5)]
        public int CriticalityLevel { get; set; } = 3;

        [Display(Name = "Fréquence d'exécution")]
        public string ExecutionFrequency { get; set; } = "Manual"; // Manual, Daily, Weekly, OnDemand

        [Display(Name = "Tags")]
        public string? Tags { get; set; } // "smoke,regression,critical" séparés par virgules

        [Display(Name = "Durée estimée (minutes)")]
        public int EstimatedMinutes { get; set; } = 2;

        [Display(Name = "Persona utilisateur")]
        public string? UserPersona { get; set; } // "Admin", "Client", "Guest"

        [Display(Name = "Actif")]
        public bool Active { get; set; } = true;

        public bool Selected { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relations
        public Feature? Feature { get; set; }
        public List<TestStep> TestSteps { get; set; } = new();
    }

    public class TestStep
    {
        public int Id { get; set; }

        public int TestCaseId { get; set; }

        [Required, Display(Name = "Action")]
        public string Action { get; set; }

        [Display(Name = "Élément ciblé")]
        public string? Target { get; set; }

        [Display(Name = "Sélecteur technique")]
        public string? Selector { get; set; }

        // [Display(Name = "XPath (fallback)")]
        // public string? Xpath { get; set; }
        //
        // [Display(Name = "ID HTML")]
        // public string? ElementId { get; set; }
        //
        // [Display(Name = "Tag HTML")]
        // public string? TagName { get; set; }
        //
        // [Display(Name = "Texte de l'élément")]
        // public string? Text { get; set; }

        [Display(Name = "Valeur à saisir")]
        public string? Value { get; set; }

        // [Display(Name = "Valeur attendue")]
        // public string? ExpectedValue { get; set; }

        [Display(Name = "Ordre")]
        public int Order { get; set; }

        // [Display(Name = "URL de la page")]
        // public string? PageUrl { get; set; }
        //
        // [Display(Name = "Titre de la page")]
        // public string? PageTitle { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Optionnel")]
        public bool IsOptional { get; set; } = false;

        [Display(Name = "Délai d'attente (secondes)")]
        public int TimeoutSeconds { get; set; } = 10;

        // Métadonnées (debug / traçabilité)
        // public long? Timestamp { get; set; }
        // public int? SequenceNumber { get; set; }

        // Relations
        public TestCase? TestCase { get; set; }
    }

    
    public class TestData
    {
        public int Id { get; set; }

        [Required, Display(Name = "Application")]
        public int ApplicationId { get; set; }

        [Required, Display(Name = "Nom du jeu de données")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Type de données")]
        public string DataType { get; set; } = "UserAccount"; // UserAccount, Product, Payment, etc.

        [Display(Name = "Données (format simple)")]
        public string DataJson { get; set; } = "{}";

        [Display(Name = "Modèle/Template")]
        public bool IsTemplate { get; set; } = false;

        [Display(Name = "Environnement spécifique")]
        public int? SpecificEnvironmentId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relations
        public Application? Application { get; set; }
        public Environment? SpecificEnvironment { get; set; }
    }
    public class Environment
    {
        public int Id { get; set; }

        [Required, Display(Name = "Application")]
        public int ApplicationId { get; set; }

        [Required, Display(Name = "Nom de l'environnement")]
        public string Name { get; set; }

        [Required, Display(Name = "URL de base")]
        [Url]
        public string BaseUrl { get; set; }

        [Display(Name = "Type")]
        public string Type { get; set; } = "Staging"; // Development, Staging, Production, Testing

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Authentification requise")]
        public bool RequiresAuth { get; set; } = false;

        [Display(Name = "Informations d'accès")]
        public string? AccessInfo { get; set; }

        [Display(Name = "Actif")]
        public bool Active { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relations
        public Application? Application { get; set; }
    }
    public class TestRun
    {
        public int Id { get; set; }

        [Required, Display(Name = "Application")]
        public int ApplicationId { get; set; }

        [Required, Display(Name = "Nom de l'exécution")]
        public string Name { get; set; }

        [Display(Name = "Déclencheur")]
        public string Trigger { get; set; } = "Manual"; // Manual, Scheduled, CI/CD, API

        [Required, Display(Name = "Type d'exécution")]
        public string ExecutionType { get; set; } // "Feature", "TestCase", "Multiple", "FullRegression"

        [Required, Display(Name = "Cibles")]
        public string TargetIds { get; set; }

        [Required, Display(Name = "Environnement")]
        public int EnvironmentId { get; set; }

        [Display(Name = "Données de test")]
        public int? TestDataId { get; set; }

        [Display(Name = "Navigateur")]
        public string Browser { get; set; } = "Chrome"; // Chrome, Firefox, Safari, Edge

        [Display(Name = "Mode headless")]
        public bool Headless { get; set; } = true;

        [Display(Name = "Statut")]
        public string Status { get; set; } = "Created";

        [Display(Name = "Progression")]
        public int ProgressPercentage { get; set; } = 0;

        [Display(Name = "Tests réussis")]
        public int PassedCount { get; set; } = 0;

        [Display(Name = "Tests échoués")]
        public int FailedCount { get; set; } = 0;

        [Display(Name = "Tests ignorés")]
        public int SkippedCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        [Display(Name = "Résultats détaillés")]
        public string? DetailedResults { get; set; }

        [Display(Name = "Logs d'exécution")]
        public string? ExecutionLogs { get; set; }

        [Display(Name = "Captures d'écran")]
        public string? Screenshots { get; set; } // JSON array des URLs

        [Display(Name = "Rapport PDF")]
        public string? ReportPath { get; set; }
        // Métriques calculées (peuvent être calculées ou stockées)
        public double AverageDurationMs { get; set; }
        public double SuccessRate { get; set; }

        // Relations
        public Application? Application { get; set; }
        public Environment? Environment { get; set; }
        public TestData? TestData { get; set; }
        public string? CreatedByUserId { get; set; }
    }
    public class ActionTemplate
    {
        public int Id { get; set; }

        [Required, Display(Name = "Nom de l'action")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Catégorie")]
        public string Category { get; set; } // Navigation, Form, Validation, Wait

        [Display(Name = "Icône")]
        public string Icon { get; set; }

        [Display(Name = "Exemple d'usage")]
        public string Example { get; set; }

        // Exemples prédéfinis
        public static List<ActionTemplate> GetDefaultTemplates()
        {
            return new List<ActionTemplate>
            {
                new() { Name = "Aller à la page", Category = "Navigation", Icon = "🏠", Description = "Naviguer vers une page", Example = "Aller à /login" },
                new() { Name = "Cliquer sur", Category = "Interaction", Icon = "👆", Description = "Cliquer sur un élément", Example = "Cliquer sur le bouton Se connecter" },
                new() { Name = "Saisir du texte", Category = "Form", Icon = "⌨️", Description = "Remplir un champ", Example = "Saisir l'email dans le champ Email" },
                new() { Name = "Vérifier que", Category = "Validation", Icon = "✅", Description = "Valider un résultat", Example = "Vérifier que le message Bienvenue s'affiche" },
                new() { Name = "Attendre", Category = "Wait", Icon = "⏱️", Description = "Attendre un délai", Example = "Attendre 3 secondes" }
            };
        }
    }
}
