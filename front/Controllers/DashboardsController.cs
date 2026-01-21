using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TesterLab.Domain.DTOs;
using TesterLab.Domain.interfaces.Repositories;
using TesterLab.Domain.interfaces.Services;
using TesterLab.Domain.Models;

namespace front.Controllers
{
  public class DashboardsController : Controller
  {
          private readonly IApplicationRepository _applicationRepo;
        private readonly ITestRunRepository2 _testRunRepo;
        private readonly IJobRepository2 _jobRepo;

        public DashboardsController(
            IApplicationRepository applicationRepo,
            ITestRunRepository2 testRunRepo,
          IJobRepository2  jobRepo)
        {
            _applicationRepo = applicationRepo;
            _testRunRepo = testRunRepo;
            _jobRepo = jobRepo;
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
