using System.ComponentModel.DataAnnotations;
using Execution.Domain.Common;
using Execution.Domain.Enums;

namespace Execution.Domain.Models;

public class Application : TenantAuditableEntity
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Url, MaxLength(500)]
        public string? MainUrl { get; set; }

        public AppType AppType { get; set; } = AppType.Web;

        public bool Active { get; set; } = true;

        // Relations
        public List<Feature> Features { get; set; } = new();
        public List<TestEnvironment> Environments { get; set; } = new();
        public List<TestDataSet> TestDataSets { get; set; } = new();
    }

    public class Feature : TenantAuditableEntity
    {
        [Required]
        public int ApplicationId { get; set; }
        public Application? Application { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(10)]
        public string? Icon { get; set; } = "🔧";

        [Range(1, 5)]
        public int BusinessPriority { get; set; } = 3;

        public Complexity Complexity { get; set; } = Complexity.Medium;

        public bool Active { get; set; } = true;

        public List<TestCase> TestCases { get; set; } = new();
    }

    public class TestCase : TenantAuditableEntity
    {
        [Required]
        public int FeatureId { get; set; }
        public Feature? Feature { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        public TestCaseType Type { get; set; } = TestCaseType.E2E;

        [Range(1, 5)]
        public int CriticalityLevel { get; set; } = 3;

        public ExecutionFrequency ExecutionFrequency { get; set; } = ExecutionFrequency.Manual;

        // Normalisé plutôt qu'un CSV — voir TestCaseTag ci-dessous
        public List<TestCaseTag> Tags { get; set; } = new();

        public int EstimatedMinutes { get; set; } = 2;

        [MaxLength(100)]
        public string? UserPersona { get; set; }

        public bool Active { get; set; } = true;

        public List<TestStep> TestSteps { get; set; } = new();
    }

    // Tags normalisés : permet de filtrer/indexer proprement
    // ("montre-moi tous les TestCase taggés 'smoke'"), impossible
    // à faire efficacement sur une colonne CSV.
    public class TestCaseTag
    {
        public int Id { get; set; }
        public int TestCaseId { get; set; }
        public TestCase? TestCase { get; set; }

        [Required, MaxLength(50)]
        public string Value { get; set; } = string.Empty; // "smoke", "regression", "critical"
    }

    public class TestStep : AuditableEntity
    {
        public int Id { get; set; }

        [Required]
        public int TestCaseId { get; set; }
        public TestCase? TestCase { get; set; }

        [Required, MaxLength(100)]
        public string Action { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Target { get; set; }

        [MaxLength(500)]
        public string? Selector { get; set; }

        [MaxLength(50)]
        public string? TagName { get; set; }

        [MaxLength(500)]
        public string? Text { get; set; }

        [MaxLength(2000)]
        public string? Value { get; set; }

        [Required]
        public int Order { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public bool IsOptional { get; set; } = false;

        [Range(1, 300)]
        public int TimeoutSeconds { get; set; } = 10;
    }

    public class TestEnvironment : TenantAuditableEntity
    {
        [Required]
        public int ApplicationId { get; set; }
        public Application? Application { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, Url, MaxLength(500)]
        public string BaseUrl { get; set; } = string.Empty;

        public EnvironmentType Type { get; set; } = EnvironmentType.Staging;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public bool RequiresAuth { get; set; } = false;

        // Ne jamais stocker en clair — voir note sécurité plus bas
        [MaxLength(2000)]
        public string? EncryptedAccessInfo { get; set; }

        public bool Active { get; set; } = true;
    }

    public class TestDataSet : TenantAuditableEntity
    {
        [Required]
        public int ApplicationId { get; set; }
        public Application? Application { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string DataTypeCategory { get; set; } = "UserAccount"; // UserAccount, Product, Payment...

        [Required]
        public string DataJson { get; set; } = "{}";

        public bool IsTemplate { get; set; } = false;

        public int? SpecificEnvironmentId { get; set; }
        public TestEnvironment? SpecificEnvironment { get; set; }
    }