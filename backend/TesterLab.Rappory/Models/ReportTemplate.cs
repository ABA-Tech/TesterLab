using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesterLab.Rappory.Models
{
    // ═══════════════════════════════════════════════════════
    // TEMPLATE DE RAPPORT
    // ═══════════════════════════════════════════════════════

    public class ReportTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = "Standard"; // Standard, Executive, Detailed, Minimal
        public bool IncludeSummary { get; set; } = true;
        public bool IncludeCharts { get; set; } = true;
        public bool IncludeScreenshots { get; set; } = true;
        public bool IncludeFailedTestsOnly { get; set; } = false;
        public bool IncludeStepDetails { get; set; } = true;
        public bool IncludePerformanceMetrics { get; set; } = false;
        public bool IncludeHistoricalComparison { get; set; } = false;
        public string OutputFormat { get; set; } = "PDF"; // PDF, HTML, Excel
        public string BrandingLogo { get; set; } = string.Empty;
        public string PrimaryColor { get; set; } = "#007bff";
    }

    // ═══════════════════════════════════════════════════════
    // DONNÉES DU RAPPORT
    // ═══════════════════════════════════════════════════════

    public class TestRunReportData
    {
        // Informations de base
        public int TestRunId { get; set; }
        public string TestRunName { get; set; } = string.Empty;
        public string ApplicationName { get; set; } = string.Empty;
        public string EnvironmentName { get; set; } = string.Empty;
        public DateTime ExecutionDate { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public TimeSpan? Duration { get; set; }

        // Résumé statistique
        public TestRunSummaryData Summary { get; set; } = new();

        // Résultats détaillés
        public List<TestCaseResultData> TestCases { get; set; } = new();

        // Métriques de performance
        public PerformanceMetricsData? PerformanceMetrics { get; set; }

        // Historique (pour comparaison)
        public List<HistoricalRunData>? HistoricalData { get; set; }

        // Configuration
        public string Browser { get; set; } = string.Empty;
        public bool Headless { get; set; }
        public string Trigger { get; set; } = string.Empty;
    }

    public class TestRunSummaryData
    {
        public int TotalTests { get; set; }
        public int PassedCount { get; set; }
        public int FailedCount { get; set; }
        public int SkippedCount { get; set; }
        public double SuccessRate { get; set; }
        public string Status { get; set; } = string.Empty;

        // Statistiques additionnelles
        public int TotalSteps { get; set; }
        public int CriticalTestsFailed { get; set; }
        public double AverageDurationSeconds { get; set; }
    }

    public class TestCaseResultData
    {
        public int TestCaseId { get; set; }
        public string TestCaseName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // Passed, Failed, Skipped
        public int CriticalityLevel { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public double DurationSeconds { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ErrorStackTrace { get; set; }

        // Détails des steps
        public List<TestStepResultData> Steps { get; set; } = new();

        // Screenshots
        public List<string> ScreenshotPaths { get; set; } = new();
    }

    public class TestStepResultData
    {
        public int StepOrder { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public double DurationMs { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ScreenshotPath { get; set; }
    }

    public class PerformanceMetricsData
    {
        public double AveragePageLoadTime { get; set; }
        public double MaxPageLoadTime { get; set; }
        public double MinPageLoadTime { get; set; }
        public double TotalExecutionTime { get; set; }
        public Dictionary<string, double> CustomMetrics { get; set; } = new();
    }

    public class HistoricalRunData
    {
        public DateTime Date { get; set; }
        public double SuccessRate { get; set; }
        public int TotalTests { get; set; }
        public double AverageDuration { get; set; }
    }
}
