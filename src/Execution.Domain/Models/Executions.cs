using System.ComponentModel.DataAnnotations;
using Execution.Domain.Common;
using Execution.Domain.Enums;

namespace Execution.Domain.Models;

public class TestRun : TenantAuditableEntity
    {
        [Required]
        public int ApplicationId { get; set; }
        public Application? Application { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public TriggerType Trigger { get; set; } = TriggerType.Manual;

        public ExecutionType ExecutionType { get; set; }

        [Required]
        public int EnvironmentId { get; set; }
        public TestEnvironment? Environment { get; set; }

        public int? TestDataId { get; set; }
        public TestDataSet? TestData { get; set; }

        public BrowserType Browser { get; set; } = BrowserType.Chrome;
        public bool Headless { get; set; } = true;

        public RunStatus Status { get; set; } = RunStatus.Created;
        public int ProgressPercentage { get; set; } = 0;

        public int PassedCount { get; set; } = 0;
        public int FailedCount { get; set; } = 0;
        public int SkippedCount { get; set; } = 0;

        public DateTime? StartedAtUtc { get; set; }
        public DateTime? CompletedAtUtc { get; set; }

        public double AverageDurationMs { get; set; }
        public double SuccessRate { get; set; }

        [MaxLength(1000)]
        public string? ReportPath { get; set; }

        // ── Traçabilité de relance ──
        // Pointe vers le run d'origine. JAMAIS de réutilisation
        // de données : c'est un nouveau TestRun.Id à chaque fois.
        public int? SourceRunId { get; set; }
        public TestRun? SourceRun { get; set; }

        // ── Relations (le remplacement des 3 champs blob) ──
        public List<TestRunTarget> Targets { get; set; } = new();
        public List<TestCaseExecution> TestCaseExecutions { get; set; } = new();
        public List<ExecutionLog> Logs { get; set; } = new();
        public List<PerformanceMetric> PerformanceMetrics { get; set; } = new();
        public List<Screenshot> Screenshots { get; set; } = new();

        // ❌ SUPPRIMÉS : DetailedResults, ExecutionLogs, Screenshots (string)
        // → cause probable du bug de cumul, remplacés par les relations ci-dessus
    }

    // Remplace TargetIds (string CSV) — un run "Multiple"/"FullRegression"
    // vise plusieurs TestCase, chacun traçable individuellement
    public class TestRunTarget
    {
        public int Id { get; set; }

        public int TestRunId { get; set; }
        public TestRun? TestRun { get; set; }

        public int TestCaseId { get; set; }
        public TestCase? TestCase { get; set; }
    }

    public class TestCaseExecution : AuditableEntity
    {
        public int Id { get; set; }

        [Required]
        public int TestRunId { get; set; }
        public TestRun? TestRun { get; set; }

        [Required]
        public int TestCaseId { get; set; }
        public TestCase? TestCase { get; set; }

        // Dénormalisation volontaire : garde le nom même si le
        // TestCase est renommé/supprimé plus tard (historique fiable)
        [Required, MaxLength(200)]
        public string TestCaseNameSnapshot { get; set; } = string.Empty;

        public ExecutionStatus Status { get; set; } = ExecutionStatus.Pending;

        public DateTime? StartedAtUtc { get; set; }
        public DateTime? CompletedAtUtc { get; set; }
        public int DurationMs { get; set; } = 0;

        [MaxLength(2000)]
        public string? ErrorMessage { get; set; }
        public string? ErrorStackTrace { get; set; }

        public int TotalSteps { get; set; } = 0;
        public int PassedSteps { get; set; } = 0;
        public int FailedSteps { get; set; } = 0;

        // Numéro de tentative — SEUL cas légitime d'accumulation
        // (retry automatique d'un test flaky dans le MÊME run)
        public int AttemptNumber { get; set; } = 1;

        public List<TestStepExecution> StepExecutions { get; set; } = new();
    }

    public class TestStepExecution
    {
        public int Id { get; set; }

        [Required]
        public int TestCaseExecutionId { get; set; }
        public TestCaseExecution? TestCaseExecution { get; set; }

        [Required]
        public int TestStepId { get; set; }
        public TestStep? TestStep { get; set; }

        [Required]
        public int StepOrder { get; set; }

        [Required, MaxLength(100)]
        public string Action { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? Selector { get; set; }

        [MaxLength(2000)]
        public string? Value { get; set; }

        public ExecutionStatus Status { get; set; } = ExecutionStatus.Pending;

        public DateTime? StartedAtUtc { get; set; }
        public DateTime? CompletedAtUtc { get; set; }
        public int DurationMs { get; set; }

        [MaxLength(2000)]
        public string? ErrorMessage { get; set; }

        public bool IsOptional { get; set; }

        public int AttemptNumber { get; set; } = 1; // retry au niveau step

        public List<Screenshot> Screenshots { get; set; } = new();
    }

    public class ExecutionLog
    {
        public long Id { get; set; } // long : volume potentiellement élevé

        [Required]
        public int TestRunId { get; set; }
        public TestRun? TestRun { get; set; }

        public int? TestCaseExecutionId { get; set; }
        public TestCaseExecution? TestCaseExecution { get; set; }

        [Required]
        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

        public LogLevel Level { get; set; } = LogLevel.Info;

        [Required, MaxLength(2000)]
        public string Message { get; set; } = string.Empty;

        public string? Details { get; set; }
        public string? StackTrace { get; set; }
    }

    public class PerformanceMetric
    {
        public long Id { get; set; }

        [Required]
        public int TestRunId { get; set; }
        public TestRun? TestRun { get; set; }

        public int? TestCaseExecutionId { get; set; }
        public TestCaseExecution? TestCaseExecution { get; set; }

        [Required, MaxLength(100)]
        public string MetricName { get; set; } = string.Empty;

        public double Value { get; set; }

        [MaxLength(20)]
        public string Unit { get; set; } = "ms";

        public DateTime RecordedAtUtc { get; set; } = DateTime.UtcNow;

        public string? ContextJson { get; set; }
    }

    public class Screenshot
    {
        public int Id { get; set; }

        [Required]
        public int TestRunId { get; set; }
        public TestRun? TestRun { get; set; }

        public int? TestCaseExecutionId { get; set; }
        public TestCaseExecution? TestCaseExecution { get; set; }

        public int? TestStepExecutionId { get; set; }
        public TestStepExecution? TestStepExecution { get; set; }

        [Required, MaxLength(500)]
        public string FilePath { get; set; } = string.Empty;

        public ScreenshotType Type { get; set; } = ScreenshotType.Failure;

        public DateTime CapturedAtUtc { get; set; } = DateTime.UtcNow;

        public long FileSizeBytes { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
    }