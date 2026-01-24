using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TesterLab.Domain.DTOs;
using TesterLab.Domain.interfaces.Repositories;
using TesterLab.Domain.interfaces.Services;
using TesterLab.Domain.Models;
using TesterLab.Models;

namespace front.Controllers
{
  [Authorize(Roles = "Admin")]
  public class DashboardsController : Controller
  {
          private readonly IApplicationRepository _applicationRepo;
        private readonly ITestRunRepository2 _testRunRepo;
        private readonly IJobRepository2 _jobRepo;
    private readonly ITestExecutionService3 _executionService3;

    public DashboardsController(
            IApplicationRepository applicationRepo,
            ITestRunRepository2 testRunRepo,
          IJobRepository2  jobRepo,
          ITestExecutionService3 testExecutionService3)
        {
            _applicationRepo = applicationRepo;
            _testRunRepo = testRunRepo;
            _jobRepo = jobRepo;
            _executionService3 = testExecutionService3;
        }

        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel
            {
                // Statistiques globales
                TotalApplications = await _applicationRepo.CountAsync(),
                ActiveApplications = await _applicationRepo.CountActiveAsync(),
                TotalTestCases = await _applicationRepo.CountAllTestCasesAsync(),
                ActiveJobs = await _jobRepo.CountActiveAsync(),

                // Exécutions récentes (30 derniers jours)
                RecentTestRuns = await _testRunRepo.GetRecentAsync(30),
                TotalExecutions = await _testRunRepo.CountRecentAsync(30),
                SuccessfulExecutions = await _testRunRepo.CountRecentByStatusAsync(30, "Passed"),
                FailedExecutions = await _testRunRepo.CountRecentByStatusAsync(30, "Failed"),

                // Taux de réussite
                SuccessRate = await _testRunRepo.GetSuccessRateAsync(30),

                // Applications avec statistiques
                Applications = await _applicationRepo.GetAllWithStatsAsync(),

                // Jobs planifiés
                UpcomingJobs = await _jobRepo.GetUpcomingAsync(10),

                // Dernières exécutions
                LatestTestRuns = await _testRunRepo.GetLatestAsync(5),

                // Graphiques de tendance - Par défaut 14 jours pour un bon compromis
                TestTrends = await _testRunRepo.GetDailyTrendsAsync(14)
            };

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> GetChartData(string type, int days = 7)
        {
            try
            {
                switch (type)
                {
                    case "success-rate":
                        var successData = await _testRunRepo.GetSuccessRateTrendAsync(days);
                        return Json(successData);

                    case "execution-volume":
                        // Retourner les données complètes des tendances quotidiennes
                        var volumeData = await _testRunRepo.GetDailyTrendsAsync(days);
                        return Json(volumeData);

                    default:
                        return BadRequest(new { error = "Type de données invalide" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Erreur serveur", message = ex.Message });
            }
        }


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
        viewModel.ScreenshotUrls = (await _executionService3.GetScreenshotsByRunIdAsync(id)).Select(s => s.FilePath).ToList();
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

    // Action pour voir une capture d'écran
    public async Task<IActionResult> ViewScreenshot(int runId, string path)
    {
      var testRun = await _executionService3.GetTestRunByIdAsync(runId);

      if (testRun == null)
        return NotFound();

      var filePath = "wwwroot" + Path.Combine(Directory.GetCurrentDirectory(), path);

      if (!System.IO.File.Exists(filePath))
        return NotFound();

      var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
      return File(fileBytes, "image/png");
    }

  }

  // ViewModel pour le dashboard
  public class DashboardViewModel
    {
        public int TotalApplications { get; set; }
        public int ActiveApplications { get; set; }
        public int TotalTestCases { get; set; }
        public int ActiveJobs { get; set; }

        public int TotalExecutions { get; set; }
        public int SuccessfulExecutions { get; set; }
        public int FailedExecutions { get; set; }
        public double SuccessRate { get; set; }

        public List<Application> Applications { get; set; } = new();
        public List<TestRun> RecentTestRuns { get; set; } = new();
        public List<Job> UpcomingJobs { get; set; } = new();
        public List<TestRun> LatestTestRuns { get; set; } = new();
        public List<DailyTrendData> TestTrends { get; set; } = new();
    }
}
