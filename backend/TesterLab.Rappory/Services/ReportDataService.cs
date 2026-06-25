using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesterLab.Infrastructure.Data;
using TesterLab.Rappory.Models;

namespace TesterLab.Rappory.Services
{
    public interface IReportDataService
    {
        Task<TestRunReportData> CollectTestRunDataAsync(int testRunId, bool includeHistorical = false);
        Task<List<HistoricalRunData>> GetHistoricalDataAsync(int applicationId, int days = 30);
    }

    public class ReportDataService : IReportDataService
    {
        private readonly TesterLabDbContext _context;
        private readonly ILogger<ReportDataService> _logger;

        public ReportDataService(
            TesterLabDbContext context,
            ILogger<ReportDataService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<TestRunReportData> CollectTestRunDataAsync(int testRunId, bool includeHistorical = false)
        {
            TestRunReportData reportData = null;
            try
            {
                // Charger le TestRun avec toutes ses relations
                var testRun = await _context.TestRuns
                    .Include(tr => tr.Application)
                    .Include(tr => tr.Environment)
                    .Include(tr => tr.TestData)
                    .FirstOrDefaultAsync(tr => tr.Id == testRunId);

                if (testRun == null)
                {
                    throw new Exception($"TestRun {testRunId} introuvable");
                }

                // Charger les exécutions de test cases
                var testCaseExecutions = await _context.TestCaseExecutions
                    .Include(tce => tce.TestCase)
                    .Include(tce => tce.StepExecutions)
                    .Where(tce => tce.TestRunId == testRunId)
                    .OrderBy(tce => tce.StartedAt)
                    .ToListAsync();

                // Charger les screenshots
                var screenshots = await _context.Screenshots
                    .Where(s => s.TestRunId == testRunId)
                    .ToListAsync();

                // Construire le rapport
                reportData = new TestRunReportData
                {
                    TestRunId = testRun.Id,
                    TestRunName = testRun.Name,
                    ApplicationName = testRun.Application?.Name ?? "Unknown",
                    EnvironmentName = testRun.Environment?.Name ?? "Unknown",
                    ExecutionDate = testRun.CreatedAt,
                    StartedAt = testRun.StartedAt,
                    CompletedAt = testRun.CompletedAt,
                    Duration = testRun.CompletedAt.HasValue && testRun.StartedAt.HasValue
                        ? testRun.CompletedAt.Value - testRun.StartedAt.Value
                        : null,
                    Browser = testRun.Browser,
                    Headless = testRun.Headless,
                    Trigger = testRun.Trigger,
                    Summary = new TestRunSummaryData
                    {
                        TotalTests = testCaseExecutions.Count,
                        PassedCount = testRun.PassedCount,
                        FailedCount = testRun.FailedCount,
                        SkippedCount = testRun.SkippedCount,
                        SuccessRate = testRun.SuccessRate,
                        Status = testRun.Status,
                        TotalSteps = testCaseExecutions.Sum(tce => tce.TotalSteps),
                        CriticalTestsFailed = testCaseExecutions.Count(tce =>
                            tce.Status == "Failed" && tce.TestCase != null && tce.TestCase.CriticalityLevel >= 4),
                        AverageDurationSeconds = testCaseExecutions.Any()
                            ? testCaseExecutions.Average(tce => tce.DurationMs) / 1000.0
                            : 0
                    }
                };

                // Mapper les test cases
                foreach (var execution in testCaseExecutions)
                {
                    var testCaseData = new TestCaseResultData
                    {
                        TestCaseId = execution.TestCaseId,
                        TestCaseName = execution.TestCaseName,
                        Description = execution.TestCase?.Description ?? "",
                        Status = execution.Status,
                        CriticalityLevel = execution.TestCase?.CriticalityLevel ?? 3,
                        StartedAt = execution.StartedAt,
                        CompletedAt = execution.CompletedAt,
                        DurationSeconds = execution.DurationMs / 1000.0,
                        ErrorMessage = execution.ErrorMessage,
                        ErrorStackTrace = execution.ErrorStackTrace
                    };

                    // Mapper les steps
                    if (execution.StepExecutions != null)
                    {
                        testCaseData.Steps = execution.StepExecutions
                            .OrderBy(se => se.StepOrder)
                            .Select(se => new TestStepResultData
                            {
                                StepOrder = se.StepOrder,
                                Action = se.Action,
                                Description = se.Description ?? "",
                                Status = se.Status,
                                DurationMs = se.DurationMs,
                                ErrorMessage = se.ErrorMessage,
                                ScreenshotPath = se.ScreenshotPath
                            })
                            .ToList();
                    }

                    // Ajouter les screenshots liés à ce test case
                    testCaseData.ScreenshotPaths = screenshots
                        .Where(s => s.TestCaseExecutionId == execution.Id)
                        .Select(s => s.FilePath)
                        .ToList();

                    reportData.TestCases.Add(testCaseData);
                }

                // Ajouter les données historiques si demandé
                if (includeHistorical && testRun.ApplicationId > 0)
                {
                    try
                    {
                        reportData.HistoricalData = await GetHistoricalDataAsync(testRun.ApplicationId, 30);
                    }
                    catch (Exception)
                    {
                    }                
                }

                // Calculer les métriques de performance
                var performanceMetrics = await _context.PerformanceMetrics
                    .Where(pm => pm.TestRunId == testRunId)
                    .ToListAsync();

                if (performanceMetrics.Any())
                {
                    try
                    {
                        reportData.PerformanceMetrics = new PerformanceMetricsData
                        {
                            //AveragePageLoadTime = performanceMetrics
                            //           .Where(pm => pm.MetricName == "PageLoadTime").ToList()
                            //           .Average(pm => pm.Value),
                            //MaxPageLoadTime = performanceMetrics
                            //           .Where(pm => pm.MetricName == "PageLoadTime").ToList()
                            //           .Max(pm => pm.Value),
                            //MinPageLoadTime = performanceMetrics
                            //           .Where(pm => pm.MetricName == "PageLoadTime").ToList()
                            //           .Min(pm => pm.Value),
                            TotalExecutionTime = testRun.CompletedAt.HasValue && testRun.StartedAt.HasValue
                                       ? (testRun.CompletedAt.Value - testRun.StartedAt.Value).TotalSeconds
                                       : 0
                        };
                    }
                    catch (Exception)
                    {
                    }
                }
                _logger.LogInformation("Données de rapport collectées pour TestRun {TestRunId}", testRunId);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la collecte des données pour TestRun {TestRunId}", testRunId);
                //throw;
            }
            return reportData;
        }

        public async Task<List<HistoricalRunData>> GetHistoricalDataAsync(int applicationId, int days = 30)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-days);

            var historicalRuns = await _context.TestRuns
                .Where(tr => tr.ApplicationId == applicationId
                          && tr.CreatedAt >= cutoffDate
                          && tr.Status == "Completed")
                .OrderBy(tr => tr.CreatedAt)
                .Select(tr => new HistoricalRunData
                {
                    Date = tr.CreatedAt,
                    SuccessRate = tr.SuccessRate,
                    TotalTests = tr.PassedCount + tr.FailedCount + tr.SkippedCount,
                    AverageDuration = tr.CompletedAt.HasValue && tr.StartedAt.HasValue
                        ? (tr.CompletedAt.Value - tr.StartedAt.Value).TotalSeconds
                        : 0
                })
                .ToListAsync();

            return historicalRuns;
        }
    }
}
