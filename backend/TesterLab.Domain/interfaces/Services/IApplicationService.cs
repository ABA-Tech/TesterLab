using TesterLab.Domain.Models;
using Environment = TesterLab.Domain.Models.Environment;

namespace TesterLab.Domain.interfaces.Services
{
    public interface IApplicationService
    {
        Task<IEnumerable<Application>> GetAllApplicationsAsync();
        Task<Application?> GetApplicationByIdAsync(int id);
        Task<Application?> GetSelectedAsync();
        Task<Application?> SetSelectedAsync(int appId);
        Task<Application> CreateApplicationAsync(Application application);
        Task<Application> UpdateApplicationAsync(Application application);
        Task<bool> DeleteApplicationAsync(int id);
        Task<bool> ValidateApplicationNameAsync(string name, int? excludeId = null);
    }

    public interface IFeatureService
    {
        Task<IEnumerable<Feature>> GetFeaturesByApplicationAsync(int applicationId);
        Task<Feature?> GetFeatureByIdAsync(int id);
        Task<Feature> CreateFeatureAsync(Feature feature);
        Task<Feature> UpdateFeatureAsync(Feature feature);
        Task<bool> DeleteFeatureAsync(int id);
        Task<bool> CanDeleteFeatureAsync(int id);
    }

    public interface ITestCaseService
    {
        Task<IEnumerable<TestCase>> GetTestCasesByFeatureAsync(int featureId);
        Task<IEnumerable<TestCase>> GetTestCasesByApplicationAsync(int applicationId);
        Task<TestCase?> GetTestCaseWithStepsAsync(int id);
        Task<TestCase> CreateTestCaseAsync(TestCase testCase);
        Task<TestCase> UpdateTestCaseAsync(TestCase testCase);
        Task<bool> DeleteTestCaseAsync(int id);
        Task<TestCase> DuplicateTestCaseAsync(int id, string newName);
        Task<IEnumerable<TestCase>> SearchTestCasesAsync(int applicationId, string query);
    }

    public interface ITestStepService: IGenericService<TestStep>
    {
        Task<IEnumerable<TestStep>> GetStepsByTestCaseAsync(int testCaseId);
        Task<TestStep> CreateStepAsync(TestStep testStep);
        Task<TestStep> UpdateStepAsync(TestStep testStep);
        Task<bool> DeleteStepAsync(int id);
        Task<bool> ReorderStepsAsync(int testCaseId, List<int> stepIds);
        Task<TestStep> InsertStepAsync(TestStep testStep, int afterStepId);
    }

    public interface ITestDataService
    {
        Task<IEnumerable<TestData>> GetTestDataByApplicationAsync(int applicationId);
        Task<TestData?> GetTestDataByIdAsync(int id);
        Task<TestData> CreateTestDataAsync(TestData testData);
        Task<TestData> UpdateTestDataAsync(TestData testData);
        Task<bool> DeleteTestDataAsync(int id);
        Task<bool> ValidateJsonDataAsync(string jsonData);
        Task<TestData> CreateFromTemplateAsync(int templateId, string name, Dictionary<string, string> values);
    }

    public interface IEnvironmentService
    {
        Task<IEnumerable<Environment>> GetEnvironmentsByApplicationAsync(int applicationId);
        Task<Environment?> GetEnvironmentByIdAsync(int id);
        Task<Environment> CreateEnvironmentAsync(Environment environment);
        Task<Environment> UpdateEnvironmentAsync(Environment environment);
        Task<bool> DeleteEnvironmentAsync(int id);
        Task<bool> TestEnvironmentConnectivityAsync(int id);
        //Task CloneEnvironmentAsync(int id, string newName, string newUrl);
    }

    public interface ITestExecutionService
    {
        Task<TestRun> CreateTestRunAsync(TestRun testRun);
        Task<TestRun> StartTestRunAsync(int testRunId);
        Task<TestRun> UpdateTestRunProgressAsync(int testRunId, int progressPercentage, string? logs = null);
        Task<TestRun> CompleteTestRunAsync(int testRunId, string status, string results);
        Task<IEnumerable<TestRun>> GetRecentRunsAsync(int applicationId);
        Task<Dictionary<string, int>> GetRunStatisticsAsync(int applicationId);
        Task<string> GenerateReportAsync(int testRunId);
        Task<TestRun?> GetTestRunByIdAsync(int id);
        Task<IEnumerable<TestRun>> GetRunningTestsAsync();
    }

    public interface IActionTemplateService
    {
        Task<IEnumerable<ActionTemplate>> GetAllTemplatesAsync();
        Task<IEnumerable<ActionTemplate>> GetTemplatesByCategoryAsync(string category);
        Task<ActionTemplate> CreateCustomTemplateAsync(ActionTemplate template);
    }
}
