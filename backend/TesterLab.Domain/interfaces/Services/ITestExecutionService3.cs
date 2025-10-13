using TesterLab.Domain.Models;
using Environment = TesterLab.Domain.Models.Environment;

namespace TesterLab.Domain.interfaces.Services
{
    public interface ITestExecutionService3
    {
        // Gestion des TestRuns
        Task<TestRun> CreateTestRunAsync(TestRun testRun);
        Task<TestRun> StartTestRunAsync(int testRunId);
        Task<TestRun> GetTestRunByIdAsync(int id);
        Task<TestRun> CompleteTestRunAsync(int testRunId, string status, string results);

        // Sélection d'environnement et données
        Task<IEnumerable<Environment>> GetEnvironmentsByApplicationAsync(int applicationId);
        Task<IEnumerable<TestData>> GetTestDataByApplicationAsync(int applicationId);
        Task<IEnumerable<TestData>> GetTestDataByEnvironmentAsync(int environmentId);
        Task<TestData> GetBestTestDataForEnvironmentAsync(int applicationId, int environmentId);

        // Récupération des résultats pour dashboards
        Task<IEnumerable<TestCaseExecution>> GetTestCaseExecutionsByRunIdAsync(int testRunId);
        Task<IEnumerable<TestCaseExecution>> GetTestCaseExecutionsByApplicationAsync(int applicatonId);
        Task<IEnumerable<ExecutionLog>> GetExecutionLogsByRunIdAsync(int testRunId);
        Task<IEnumerable<PerformanceMetric>> GetPerformanceMetricsByRunIdAsync(int testRunId);
        Task<IEnumerable<Screenshot>> GetScreenshotsByRunIdAsync(int testRunId);
    }
}
