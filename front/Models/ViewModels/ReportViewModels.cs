using TesterLab.Rappory.Models;

namespace TesterLab.Models.ViewModels
{
  public class GenerateReportViewModel
  {
    public int TestRunId { get; set; }
    public string TestRunName { get; set; } = string.Empty;

    public ReportTemplate SelectedTemplate { get; set; } = new();
    public List<ReportTemplate> AvailableTemplates { get; set; } = new();
  }

  public class ReportPreviewViewModel
  {
    public int TestRunId { get; set; }
    public string TestRunName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int TotalTests { get; set; }
    public int PassedCount { get; set; }
    public int FailedCount { get; set; }
    public int SkippedCount { get; set; }
    public double SuccessRate { get; set; }
    public TimeSpan? Duration { get; set; }
  }
}
