using TesterLab.Domain.Models;
using TesterLab.Domain.interfaces.Repositories;
using TesterLab.Domain.interfaces.Services;
using System.Text.Json;

namespace TesterLab.Applications.Services
{
    public class TestExecutionService : ITestExecutionService
    {
        private readonly ITestRunRepository _testRunRepository;
        private readonly ITestCaseRepository _testCaseRepository;
        private readonly IFeatureRepository _featureRepository;

        public TestExecutionService(
            ITestRunRepository testRunRepository,
            ITestCaseRepository testCaseRepository,
            IFeatureRepository featureRepository)
        {
            _testRunRepository = testRunRepository;
            _testCaseRepository = testCaseRepository;
            _featureRepository = featureRepository;
        }

        public async Task<TestRun> CreateTestRunAsync(TestRun testRun)
        {
            var fid = int.TryParse(testRun.TargetIds, out int featureId);
            var testcases = fid ? (await _testCaseRepository.GetByFeatureIdAsync(featureId)) : new List<TestCase>();

            testRun.Status = "Created";
            testRun.TargetIds = JsonSerializer.Serialize(testcases.Select(x=>x.Id).ToArray());
            testRun.ProgressPercentage = 0;
            return await _testRunRepository.CreateAsync(testRun);
        }

        public async Task<TestRun> StartTestRunAsync(int testRunId)
        {
            var testRun = await _testRunRepository.GetByIdAsync(testRunId);
            if (testRun == null)
                throw new ArgumentException("Test run not found");

            testRun.Status = "Running";
            testRun.StartedAt = DateTime.UtcNow;
            testRun.ProgressPercentage = 0;

            return await _testRunRepository.UpdateAsync(testRun);
        }

        public async Task<TestRun> UpdateTestRunProgressAsync(int testRunId, int progressPercentage, string? logs = null)
        {
            var testRun = await _testRunRepository.GetByIdAsync(testRunId);
            if (testRun == null)
                throw new ArgumentException("Test run not found");

            testRun.ProgressPercentage = Math.Min(100, Math.Max(0, progressPercentage));

            if (!string.IsNullOrEmpty(logs))
            {
                testRun.ExecutionLogs = (testRun.ExecutionLogs ?? "") + logs + "\n";
            }

            return await _testRunRepository.UpdateAsync(testRun);
        }

        public async Task<TestRun> CompleteTestRunAsync(int testRunId, string status, string results)
        {
            var testRun = await _testRunRepository.GetByIdAsync(testRunId);
            if (testRun == null)
                throw new ArgumentException("Test run not found");

            testRun.Status = status;
            testRun.CompletedAt = DateTime.UtcNow;
            testRun.ProgressPercentage = 100;
            testRun.DetailedResults = results;

            // Parse results to update counters
            try
            {
                var resultData = JsonSerializer.Deserialize<Dictionary<string, object>>(results);
                if (resultData != null)
                {
                    if (resultData.ContainsKey("passed"))
                        testRun.PassedCount = Convert.ToInt32(resultData["passed"]);
                    if (resultData.ContainsKey("failed"))
                        testRun.FailedCount = Convert.ToInt32(resultData["failed"]);
                    if (resultData.ContainsKey("skipped"))
                        testRun.SkippedCount = Convert.ToInt32(resultData["skipped"]);
                }
            }
            catch
            {
                // Ignore JSON parsing errors
            }

            return await _testRunRepository.UpdateAsync(testRun);
        }

        public async Task<IEnumerable<TestRun>> GetRecentRunsAsync(int applicationId)
        {
            return await _testRunRepository.GetRecentAsync(applicationId, 20);
        }

        public async Task<Dictionary<string, int>> GetRunStatisticsAsync(int applicationId)
        {
            return await _testRunRepository.GetStatusCountsAsync(applicationId);
        }

        public async Task<string> GenerateReportAsync(int testRunId)
        {
            var testRun = await _testRunRepository.GetByIdAsync(testRunId);
            if (testRun == null)
                throw new ArgumentException("Test run not found");

            // Ici vous pouvez générer un rapport PDF/HTML
            // Pour l'exemple, on retourne juste un chemin fictif
            var reportPath = $"/reports/test-run-{testRunId}-{DateTime.Now:yyyyMMdd-HHmmss}.pdf";

            testRun.ReportPath = reportPath;
            await _testRunRepository.UpdateAsync(testRun);

            return reportPath;
        }

        public async Task<TestRun?> GetTestRunByIdAsync(int id)
        {
            return await _testRunRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<TestRun>> GetRunningTestsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
