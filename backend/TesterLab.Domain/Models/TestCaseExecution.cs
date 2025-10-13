using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesterLab.Domain.Models
{
    // Résultat d'exécution d'un TestCase
    public class TestCaseExecution
    {
        public int Id { get; set; }

        [Required]
        public int TestRunId { get; set; }

        [Required]
        public int TestCaseId { get; set; }

        [Required, MaxLength(200)]
        public string TestCaseName { get; set; }

        [Required, MaxLength(50)]
        public string Status { get; set; } // Passed, Failed, Skipped, Error

        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public int DurationMs { get; set; } = 0; // Durée en millisecondes

        public string ErrorMessage { get; set; }
        public string ErrorStackTrace { get; set; }

        public int TotalSteps { get; set; } = 0;
        public int PassedSteps { get; set; } = 0;
        public int FailedSteps { get; set; } = 0;

        // Relations
        public TestRun? TestRun { get; set; }
        public TestCase? TestCase { get; set; }
        public List<TestStepExecution>? StepExecutions { get; set; } = new();
    }

    // Résultat d'exécution d'un TestStep
    public class TestStepExecution
    {
        public int Id { get; set; }

        [Required]
        public int TestCaseExecutionId { get; set; }

        [Required]
        public int TestStepId { get; set; }

        [Required]
        public int StepOrder { get; set; }

        [Required, MaxLength(100)]
        public string Action { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public string Selector { get; set; }
        public string Value { get; set; }

        [Required, MaxLength(50)]
        public string Status { get; set; } // Passed, Failed, Skipped

        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public int DurationMs { get; set; }

        public string ErrorMessage { get; set; }

        public string ScreenshotPath { get; set; }

        public bool IsOptional { get; set; }

        // Relations
        public TestCaseExecution? TestCaseExecution { get; set; }
        public TestStep? TestStep { get; set; }
    }

    // Log d'exécution en temps réel
    public class ExecutionLog
    {
        public int Id { get; set; }

        [Required]
        public int TestRunId { get; set; }

        public int? TestCaseExecutionId { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [Required, MaxLength(50)]
        public string Level { get; set; } // Info, Warning, Error, Debug

        [Required]
        public string Message { get; set; } = "";

        public string Details { get; set; } = "";

        public string StackTrace { get; set; } = "";

        // Relations
        public TestRun? TestRun { get; set; }
        public TestCaseExecution? TestCaseExecution { get; set; }
    }

    // Métrique de performance
    public class PerformanceMetric
    {
        public int Id { get; set; }

        [Required]
        public int TestRunId { get; set; }

        public int? TestCaseExecutionId { get; set; }

        [Required, MaxLength(100)]
        public string MetricName { get; set; } // PageLoadTime, ElementWaitTime, etc.

        public double Value { get; set; }

        [MaxLength(50)]
        public string Unit { get; set; } // ms, seconds, bytes, etc.

        public DateTime RecordedAt { get; set; }

        public string Context { get; set; } // JSON avec infos contextuelles

        // Relations
        public TestRun? TestRun { get; set; }
        public TestCaseExecution? TestCaseExecution { get; set; }
    }

    // Capture d'écran avec métadonnées
    public class Screenshot
    {
        public int Id { get; set; }

        [Required]
        public int TestRunId { get; set; }

        public int? TestCaseExecutionId { get; set; }

        public int? TestStepExecutionId { get; set; }

        [Required, MaxLength(500)]
        public string FilePath { get; set; }

        [Required, MaxLength(50)]
        public string Type { get; set; } // Success, Failure, Error, Assertion

        public DateTime CapturedAt { get; set; }

        public long FileSizeBytes { get; set; }

        public string Description { get; set; }

        // Relations
        public TestRun? TestRun { get; set; }
        public TestCaseExecution? TestCaseExecution { get; set; }
        public TestStepExecution? TestStepExecution { get; set; }
    }
}
