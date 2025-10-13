using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesterLab.Domain.Models;
using Environment = TesterLab.Domain.Models.Environment;

namespace TesterLab.Domain.interfaces.Repositories
{
    public interface IApplicationRepository
    {
        Task<IEnumerable<Application>> GetAllAsync();
        Task<Application?> GetByIdAsync(int id);
        Task<Application?> GetSelectedAsync();
        Task<Application?> SetSelectedAsync(int appId);
        Task<Application?> GetByNameAsync(string name);
        Task<Application> CreateAsync(Application application);
        Task<Application> UpdateAsync(Application application);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<Application>> GetByUserAsync(string userId);
    }

    public interface IFeatureRepository
    {
        Task<IEnumerable<Feature>> GetAllAsync();
        Task<IEnumerable<Feature>> GetByApplicationIdAsync(int applicationId);
        Task<Feature?> GetByIdAsync(int id);
        Task<Feature> CreateAsync(Feature feature);
        Task<Feature> UpdateAsync(Feature feature);
        Task<bool> DeleteAsync(int id);
        Task<int> GetTestCaseCountAsync(int featureId);
    }

    public interface ITestCaseRepository
    {
        Task<IEnumerable<TestCase>> GetAllAsync();
        Task<IEnumerable<TestCase>> GetByFeatureIdAsync(int featureId);
        Task<IEnumerable<TestCase>> GetByApplicationIdAsync(int applicationId);
        Task<IEnumerable<TestCase>> GetByTagsAsync(string[] tags);
        Task<TestCase?> GetByIdAsync(int id);
        Task<TestCase?> GetByIdWithStepsAsync(int id);
        Task<TestCase> CreateAsync(TestCase testCase);
        Task<TestCase> UpdateAsync(TestCase testCase);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<TestCase>> GetByCriticalityAsync(int minLevel);
    }

    public interface ITestStepRepository
    {
        Task<IEnumerable<TestStep>> GetByTestCaseIdAsync(int testCaseId);
        Task<TestStep?> GetByIdAsync(int id);
        Task<TestStep> CreateAsync(TestStep testStep);
        Task<TestStep> UpdateAsync(TestStep testStep);
        Task<bool> DeleteAsync(int id);
        Task<bool> ReorderStepsAsync(int testCaseId, Dictionary<int, int> newOrders);
    }

    public interface ITestDataRepository
    {
        Task<IEnumerable<TestData>> GetAllAsync();
        Task<IEnumerable<TestData>> GetByApplicationIdAsync(int applicationId);
        Task<IEnumerable<TestData>> GetTemplatesAsync();
        Task<TestData?> GetByIdAsync(int id);
        Task<TestData> CreateAsync(TestData testData);
        Task<TestData> UpdateAsync(TestData testData);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<TestData>> GetByEnvironmentAsync(int environmentId);
    }

    public interface IEnvironmentRepository
    {
        Task<IEnumerable<Environment>> GetAllAsync();
        Task<IEnumerable<Environment>> GetByApplicationIdAsync(int applicationId);
        Task<Environment?> GetByIdAsync(int id);
        Task<Environment> CreateAsync(Environment environment);
        Task<Environment> UpdateAsync(Environment environment);
        Task<bool> DeleteAsync(int id);
    }

    public interface ITestRunRepository
    {
        Task<IEnumerable<TestRun>> GetAllAsync();
        Task<IEnumerable<TestRun>> GetByApplicationIdAsync(int applicationId);
        Task<IEnumerable<TestRun>> GetRecentAsync(int applicationId, int count = 10);
        Task<TestRun?> GetByIdAsync(int id);
        Task<TestRun> CreateAsync(TestRun testRun);
        Task<TestRun> UpdateAsync(TestRun testRun);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<TestRun>> GetRunningAsync();
        Task<Dictionary<string, int>> GetStatusCountsAsync(int applicationId);
    }

    public interface IActionTemplateRepository : IGenericRepository<ActionTemplate>
    {
        Task<IEnumerable<ActionTemplate>> GetAllAsync();
        Task<IEnumerable<ActionTemplate>> GetByCategoryAsync(string category);
        Task<ActionTemplate?> GetByIdAsync(int id);
    }
}
