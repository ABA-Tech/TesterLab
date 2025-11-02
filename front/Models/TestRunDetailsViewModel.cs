using TesterLab.Domain.Models;

namespace TesterLab.Models
{
  public class TestRunDetailsViewModel
  {
    public TestRun TestRun { get; set; }
    public List<TestExecutionDetail> ExecutionDetails { get; set; } = new();
    public List<ExecutionLog> TestExecutionLog { get; set; } = new();
    public List<string> ScreenshotUrls { get; set; } = new();
    public string ExecutionLogs { get; set; }

    // Statistiques calculées
    public int TotalSteps => ExecutionDetails.Sum(d => d.Steps?.Count ?? 0);
    public int CompletedSteps => ExecutionDetails.Sum(d => d.Steps?.Count(s => s.Status == "Passed" || s.Status == "Failed") ?? 0);
    public TimeSpan? TotalDuration => TestRun.CompletedAt.HasValue && TestRun.StartedAt.HasValue
        ? TestRun.CompletedAt.Value - TestRun.StartedAt.Value
        : null;

    public string GetStatusColor()
    {
      return TestRun.Status switch
      {
        "Completed" => TestRun.FailedCount > 0 ? "danger" : "success",
        "Running" => "primary",
        "Failed" => "danger",
        "Cancelled" => "warning",
        _ => "secondary"
      };
    }

    public string GetStatusIcon()
    {
      return TestRun.Status switch
      {
        "Completed" => TestRun.FailedCount > 0 ? "bi-x-circle-fill" : "bi-check-circle-fill",
        "Running" => "bi-hourglass-split",
        "Failed" => "bi-exclamation-triangle-fill",
        "Cancelled" => "bi-dash-circle-fill",
        _ => "bi-question-circle"
      };
    }
  }

  public class TestExecutionDetail
  {
    public int TestCaseId { get; set; }
    public string TestCaseName { get; set; }
    public string Status { get; set; } // Passed, Failed, Skipped
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public TimeSpan? Duration => CompletedAt.HasValue && StartedAt.HasValue
        ? CompletedAt.Value - StartedAt.Value
        : null;
    public string ErrorMessage { get; set; }
    public List<StepExecutionDetail> Steps { get; set; } = new();
  }

  public class StepExecutionDetail
  {
    public int StepId { get; set; }
    public int Order { get; set; }
    public string Action { get; set; }
    public string Target { get; set; }
    public string Status { get; set; } // Passed, Failed, Skipped, Running
    public DateTime? ExecutedAt { get; set; }
    public int DurationMs { get; set; }
    public string ErrorMessage { get; set; }
    public string ScreenshotPath { get; set; }
    public string LogMessage { get; set; }
  }
}
