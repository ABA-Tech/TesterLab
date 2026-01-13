using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TesterLab.Domain.interfaces.Repositories;
using TesterLab.Domain.interfaces.Services;
using TesterLab.Domain.Models;

namespace front.Controllers
{
  public class DashboardController : Controller
  {
    private readonly ILogger<DashboardController> _logger;
    private readonly IApplicationService _applicationService;
    private readonly ITestCaseService _testCaseService;
    private readonly ITestExecutionService _executionService;
    private readonly ITestExecutionService3 _executionService3;
    private readonly ITestRunRepository _testRunRepository;
    public DashboardController(ILogger<DashboardController> logger,
      IApplicationService applicationService,
      ITestCaseService testCaseService,
      ITestExecutionService executionService,
      ITestExecutionService3 executionService3,
      ITestRunRepository testRunRepository)
    {
      _logger = logger;
      _applicationService = applicationService;
      _testCaseService = testCaseService;
      _executionService = executionService;
      _executionService3 = executionService3;
      _testRunRepository = testRunRepository;
    }

    public async Task<IActionResult> Index()
    {
      var allApplications = await _applicationService.GetAllApplicationsAsync();
      var allTestCases = await _testCaseService.GetAllTestCasesAsync();

      var recentsRuns = await _testRunRepository.GetAllAsync();
      var model = new DashboardViewModel
      {
          // Statistiques globales
          TotalApplications = allApplications.Count(),
          ActiveApplications = allApplications.Count(),
          TotalTestCases = allTestCases.Count(),
          //ActiveJobs = await _jobRepo.CountActiveAsync(),

          // Exécutions récentes (30 derniers jours)
          RecentTestRuns = recentsRuns.ToList(),
          TotalExecutions = recentsRuns.Count(),
          // SuccessfulExecutions = await _testRunRepo.CountRecentByStatusAsync(30, "Passed"),
          // FailedExecutions = await _testRunRepo.CountRecentByStatusAsync(30, "Failed"),

          SuccessfulExecutions = recentsRuns.Count(x=>x.Status=="Passed"),
          FailedExecutions = recentsRuns.Count(x=>x.Status=="Failed"),

          // Taux de réussite
          SuccessRate = recentsRuns.Count(x=>x.Status=="Passed"),

          // Applications avec statistiques
          Applications = allApplications.ToList(),

          // // Jobs planifiés
          // UpcomingJobs = await _jobRepo.GetUpcomingAsync(10),

          // // Dernières exécutions
          // LatestTestRuns = await _testRunRepo.GetLatestAsync(5),

          // // Graphiques de tendance
          // TestTrends = await _testRunRepo.GetDailyTrendsAsync(14)
      };

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View("Error!");
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

    public class DailyTrendData
    {
        public DateTime Date { get; set; }
        public int TotalRuns { get; set; }
        public int PassedRuns { get; set; }
        public int FailedRuns { get; set; }
        public double SuccessRate { get; set; }
    }
}
