using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AspnetCoreMvcFull.Models;
using TesterLab.Domain.interfaces.Services;
using TesterLab.Models;
using Microsoft.EntityFrameworkCore;
using TesterLab.Domain.Models;
using System.Text.Json;

namespace AspnetCoreMvcFull.Controllers;

public class DashboardsController : Controller
{
  private readonly ILogger<HomeController> _logger;
  private readonly IApplicationService _applicationService;
  private readonly ITestCaseService _testCaseService;
  private readonly ITestExecutionService _executionService;
  private readonly ITestExecutionService3 _executionService3;


  public DashboardsController(ILogger<HomeController> logger,
    IApplicationService applicationService,
    ITestCaseService testCaseService,
    ITestExecutionService executionService,
    ITestExecutionService3 executionService3)
  {
    _logger = logger;
    _applicationService = applicationService;
    _testCaseService = testCaseService;
    _executionService = executionService;
    _executionService3 = executionService3;
  }

  public async Task<IActionResult> Index(int? applicationId = null, int days = 7)
  {
    try
    {
      var startDate = DateTime.UtcNow.AddDays(-days);
      var previousStartDate = startDate.AddDays(-days);
      var currentApp = STATIC_CURRENT_APP.CurrentApp;

      var allTestRuns = await _executionService.GetRecentRunsAsync(currentApp.Id);

      var applications = (await _applicationService.GetAllApplicationsAsync()).Where(x => x.Selected).ToList();








      // Calculer les métriques globales
      var totalTests = allTestRuns.Sum(tr => tr.PassedCount + tr.FailedCount + tr.SkippedCount);
      var passedTests = allTestRuns.Sum(tr => tr.PassedCount);
      var failedTests = allTestRuns.Sum(tr => tr.FailedCount);
      var skippedTests = allTestRuns.Sum(tr => tr.SkippedCount);

      var successRate = totalTests > 0 ? (double)passedTests / totalTests * 100 : 0;
      var avgDuration = allTestRuns
          .Where(tr => tr.CompletedAt.HasValue && tr.StartedAt.HasValue)
          .Select(tr => (tr.CompletedAt!.Value - tr.StartedAt!.Value).TotalSeconds)
          .DefaultIfEmpty(0)
          .Average();

      // Statistiques par application
      var appStats = allTestRuns
          //.Where(tr => tr.CreatedAt >= startDate)
          .GroupBy(tr => new { tr.ApplicationId, tr.Application!.Name })
          .Select(g => new ApplicationStats
          {
            ApplicationId = g.Key.ApplicationId,
            ApplicationName = g.Key.Name,
            TotalTests = g.Sum(tr => tr.PassedCount + tr.FailedCount + tr.SkippedCount),
            PassedTests = g.Sum(tr => tr.PassedCount),
            SuccessRate = g.Sum(tr => tr.PassedCount + tr.FailedCount + tr.SkippedCount) > 0
                  ? (double)g.Sum(tr => tr.PassedCount) / g.Sum(tr => tr.PassedCount + tr.FailedCount + tr.SkippedCount) * 100
                  : 0
          })
          .OrderByDescending(s => s.TotalTests)
          .Take(5)
          .ToList();

      // Tests critiques échoués
      var allTestcases = await _testCaseService.GetTestCasesByApplicationAsync(currentApp.Id);
      var testsExecutions = await _executionService3.GetTestCaseExecutionsByApplicationAsync(currentApp.Id);

      var criticalFailures = testsExecutions
          .Where(tce => tce.Status == "Failed"
              && tce.TestCase!.CriticalityLevel >= 4
              && tce.StartedAt >= startDate)
          .OrderByDescending(tce => tce.StartedAt)
          .Take(5)
          .Select(tce => new CriticalFailure
          {
            TestCaseName = tce.TestCaseName,
            ApplicationName = tce.TestRun!.Application!.Name,
            CriticalityLevel = tce.TestCase!.CriticalityLevel,
            ErrorMessage = tce.ErrorMessage ?? "Erreur inconnue",
            FailedAt = tce.StartedAt,
            TestRunId = tce.TestRunId
          })
          .ToList();

      // Données pour le graphique d'évolution (7 derniers jours)
      var evolutionData = new List<DailyStats>();
      for (int i = 6; i >= 0; i--)
      {
        var date = DateTime.UtcNow.Date.AddDays(-i);
        var dayEnd = date.AddDays(1);

        var dayTestRuns = allTestRuns.Where(tr => tr.CreatedAt >= date && tr.CreatedAt < dayEnd).ToList();

        evolutionData.Add(new DailyStats
        {
          Date = date,
          DayName = GetDayName(date),
          TotalTests = dayTestRuns.Sum(tr => tr.PassedCount + tr.FailedCount + tr.SkippedCount),
          PassedTests = dayTestRuns.Sum(tr => tr.PassedCount),
          FailedTests = dayTestRuns.Sum(tr => tr.FailedCount)
        });
      }

      // Assembler le ViewModel
      var viewModel = new DashboardViewModel
      {
        // Métriques globales
        SuccessRate = Math.Round(successRate, 1),
        TotalTestsExecuted = totalTests,
        FailedTests = failedTests,
        AverageDurationSeconds = Math.Round(avgDuration, 0),

        // Comparaison avec période précédente (simplifié)
        SuccessRateTrend = 2.3,
        FailedTestsTrend = -5,
        DurationTrend = -12,

        // Données détaillées
        RecentTestRuns = allTestRuns.ToList(),
        ApplicationStats = appStats,
        CriticalFailures = criticalFailures,
        EvolutionData = evolutionData,

        // Filtres
        Applications = applications,
        SelectedApplicationId = applicationId,
        SelectedDays = days
      };

      return View(viewModel);

    }
    catch (Exception)
    {
      throw;
    }
    return View();
  }

  private string GetDayName(DateTime date)
  {
    return date.ToString("ddd", new System.Globalization.CultureInfo("fr-FR"));
  }

  private async Task<PeriodStats> CalculatePeriodStats(IQueryable<TestRun> query)
  {
    var testRuns = await query.ToListAsync();

    var totalTests = testRuns.Sum(tr => tr.PassedCount + tr.FailedCount + tr.SkippedCount);
    var passedTests = testRuns.Sum(tr => tr.PassedCount);
    var failedTests = testRuns.Sum(tr => tr.FailedCount);

    var successRate = totalTests > 0 ? (double)passedTests / totalTests * 100 : 0;
    var avgDuration = testRuns
        .Where(tr => tr.CompletedAt.HasValue && tr.StartedAt.HasValue)
        .Select(tr => (tr.CompletedAt!.Value - tr.StartedAt!.Value).TotalSeconds)
        .DefaultIfEmpty(0)
        .Average();

    return new PeriodStats
    {
      TotalTests = totalTests,
      PassedTests = passedTests,
      FailedTests = failedTests,
      SuccessRate = successRate,
      AvgDuration = avgDuration
    };
  }
  //public async Task<IActionResult> Index()
  //{

  //  //var applications = await _applicationService.GetAllApplicationsAsync();
  //  //ViewBag.Applications = applications;
  //  return View();
  //}








  public async Task<IActionResult> TestRunDetails(int id)
  {
    var testRun = await _executionService3.GetTestRunByIdAsync(id);

    if (testRun == null)
      return NotFound();

    var viewModel = new TestRunDetailsViewModel
    {
      TestRun = testRun
    };

    // Parser les résultats détaillés (JSON)
    if (!string.IsNullOrEmpty(testRun.DetailedResults))
    {
      try
      {
        var testCaseExecution = await _executionService3.GetTestCaseExecutionsByRunIdAsync(id);
        var executionDetail = testCaseExecution.Select(x =>
          new TestExecutionDetail
          {
            ErrorMessage = x.ErrorMessage,
            StartedAt = x.StartedAt,
            Status = x.Status,
            CompletedAt = x.CompletedAt,
            TestCaseId = x.TestCaseId,
            TestCaseName = x.TestCaseName,
            Steps = x.StepExecutions.Select(s => new StepExecutionDetail
            {
              Status = s.Status,
              Action = s.Action,
              DurationMs = s.DurationMs,
              ErrorMessage = s.ErrorMessage,
              Order = s.StepOrder,
              StepId = s.TestStepId,
              ExecutedAt = s.StartedAt,
              LogMessage = s.Description,
              ScreenshotPath = s.ScreenshotPath,
            }).ToList()
          }).ToList();
        viewModel.ExecutionDetails = executionDetail;
      }
      catch (Exception ex)
      {
        // Log error
        viewModel.ExecutionDetails = new List<TestExecutionDetail>();
      }
    }

    // Parser les captures d'écran (JSON array)

      try
      {
        viewModel.ScreenshotUrls = (await _executionService3.GetScreenshotsByRunIdAsync(id)).Select(s=>s.FilePath).ToList();
      }
      catch
      {
        viewModel.ScreenshotUrls = new List<string>();
      }

    var executionLogs = await _executionService3.GetExecutionLogsByRunIdAsync(id);
    viewModel.TestExecutionLog = executionLogs.ToList();
    // Logs d'exécution
    viewModel.ExecutionLogs = testRun.ExecutionLogs ?? "";

    return View(viewModel);
  }

  // Action pour télécharger le rapport PDF
  public async Task<IActionResult> DownloadReport(int id)
  {
    var testRun = await _executionService3.GetTestRunByIdAsync(id);

    if (testRun == null || string.IsNullOrEmpty(testRun.ReportPath))
      return NotFound();

    var filePath = Path.Combine(Directory.GetCurrentDirectory(), testRun.ReportPath);

    if (!System.IO.File.Exists(filePath))
      return NotFound();

    var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
    return File(fileBytes, "application/pdf", $"TestReport_{testRun.Id}_{DateTime.Now:yyyyMMdd}.pdf");
  }



  // Action pour voir une capture d'écran
  public async Task<IActionResult> ViewScreenshot(int runId, string path)
  {
    var testRun = await _executionService3.GetTestRunByIdAsync(runId);

    if (testRun == null)
      return NotFound();

    var filePath = "wwwroot"+Path.Combine(Directory.GetCurrentDirectory(), path);

    if (!System.IO.File.Exists(filePath))
      return NotFound();

    var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
    return File(fileBytes, "image/png");
  }

}

// Classe helper pour les statistiques de période
internal class PeriodStats
{
  public int TotalTests { get; set; }
  public int PassedTests { get; set; }
  public int FailedTests { get; set; }
  public double SuccessRate { get; set; }
  public double AvgDuration { get; set; }
}


public class DashboardViewModel
{
  // Métriques principales
  public double SuccessRate { get; set; }
  public int TotalTestsExecuted { get; set; }
  public int FailedTests { get; set; }
  public double AverageDurationSeconds { get; set; }

  // Tendances (comparaison avec période précédente)
  public double SuccessRateTrend { get; set; }
  public int FailedTestsTrend { get; set; }
  public double DurationTrend { get; set; }

  // Données détaillées
  public List<TestRun> RecentTestRuns { get; set; } = new();
  public List<ApplicationStats> ApplicationStats { get; set; } = new();
  public List<CriticalFailure> CriticalFailures { get; set; } = new();
  public List<DailyStats> EvolutionData { get; set; } = new();

  // Filtres
  public List<Application> Applications { get; set; } = new();
  public int? SelectedApplicationId { get; set; }
  public int SelectedDays { get; set; }

  // Helpers pour la vue
  public string GetStatusBadgeClass(string status)
  {
    return status switch
    {
      "Running" => "bg-primary",
      "Completed" => "bg-success",
      "Failed" => "bg-danger",
      "Pending" => "bg-warning text-dark",
      _ => "bg-secondary"
    };
  }

  public string GetStatusIcon(string status)
  {
    return status switch
    {
      "Running" => "bi-play-circle",
      "Completed" => "bi-check-circle",
      "Failed" => "bi-x-circle",
      "Pending" => "bi-clock",
      _ => "bi-question-circle"
    };
  }

  public string FormatDuration(double seconds)
  {
    var ts = TimeSpan.FromSeconds(seconds);
    if (ts.TotalMinutes < 1)
      return $"{ts.Seconds}s";
    return $"{(int)ts.TotalMinutes}m {ts.Seconds}s";
  }

  public string GetRelativeTime(DateTime dateTime)
  {
    var diff = DateTime.UtcNow - dateTime;

    if (diff.TotalMinutes < 1)
      return "À l'instant";
    if (diff.TotalMinutes < 60)
      return $"Il y a {(int)diff.TotalMinutes} min";
    if (diff.TotalHours < 24)
      return $"Il y a {(int)diff.TotalHours}h";
    if (diff.TotalDays < 7)
      return dateTime.ToString("ddd HH:mm");
    return dateTime.ToString("dd/MM HH:mm");
  }
}

public class ApplicationStats
{
  public int ApplicationId { get; set; }
  public string ApplicationName { get; set; } = "";
  public int TotalTests { get; set; }
  public int PassedTests { get; set; }
  public double SuccessRate { get; set; }
}

public class CriticalFailure
{
  public string TestCaseName { get; set; } = "";
  public string ApplicationName { get; set; } = "";
  public int CriticalityLevel { get; set; }
  public string ErrorMessage { get; set; } = "";
  public DateTime FailedAt { get; set; }
  public int TestRunId { get; set; }
}

public class DailyStats
{
  public DateTime Date { get; set; }
  public string DayName { get; set; } = "";
  public int TotalTests { get; set; }
  public int PassedTests { get; set; }
  public int FailedTests { get; set; }

  public double SuccessRate => TotalTests > 0 ? (double)PassedTests / TotalTests * 100 : 0;
  public int BarHeight => TotalTests > 0 ? Math.Min((TotalTests * 10), 100) : 0;

  public string BarColor
  {
    get
    {
      if (FailedTests > PassedTests) return "linear-gradient(to top, #dc3545, #fd7e14)";
      if (FailedTests > 0) return "linear-gradient(to top, #ffc107, #fd7e14)";
      return "linear-gradient(to top, #198754, #20c997)";
    }
  }
}

