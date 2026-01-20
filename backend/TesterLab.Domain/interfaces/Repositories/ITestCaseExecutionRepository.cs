using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesterLab.Domain.DTOs;
using TesterLab.Domain.Models;

namespace TesterLab.Domain.interfaces.Repositories
{
    public interface ITestCaseExecutionRepository
    {
        Task<TestCaseExecution> CreateAsync(TestCaseExecution execution);
        Task<TestCaseExecution> UpdateAsync(TestCaseExecution execution);
        Task<TestCaseExecution> GetByIdAsync(int id);
        Task<IEnumerable<TestCaseExecution>> GetByTestRunIdAsync(int testRunId);
        Task<IEnumerable<TestCaseExecution>> GetByTestCaseIdAsync(int testCaseId, int limit = 10);
        Task<IEnumerable<TestCaseExecution>> GetByTestApplicationIdAsync(int applicatonId);
    }

    public interface ITestStepExecutionRepository
    {
        Task<TestStepExecution> CreateAsync(TestStepExecution execution);
        Task<TestStepExecution> UpdateAsync(TestStepExecution execution);
        Task<IEnumerable<TestStepExecution>> GetByTestCaseExecutionIdAsync(int testCaseExecutionId);
    }

    public interface IExecutionLogRepository
    {
        Task<ExecutionLog> CreateAsync(ExecutionLog log);
        Task<IEnumerable<ExecutionLog>> GetByTestRunIdAsync(int testRunId);
        Task<IEnumerable<ExecutionLog>> GetByLevelAsync(int testRunId, string level);
    }

    public interface IPerformanceMetricRepository
    {
        Task<PerformanceMetric> CreateAsync(PerformanceMetric metric);
        Task<IEnumerable<PerformanceMetric>> GetByTestRunIdAsync(int testRunId);
        Task<IEnumerable<PerformanceMetric>> GetByMetricNameAsync(string metricName, DateTime from, DateTime to);
    }

    public interface IScreenshotRepository
    {
        Task<Screenshot> CreateAsync(Screenshot screenshot);
        Task<IEnumerable<Screenshot>> GetByTestRunIdAsync(int testRunId);
        Task<IEnumerable<Screenshot>> GetByTestCaseExecutionIdAsync(int testCaseExecutionId);
    }


    public interface ITestRunRepository2
    {
        Task<List<TestRun>> GetRecentAsync(int days);
        Task<List<TestRun>> GetLatestAsync(int count);
        Task<int> CountRecentAsync(int days);
        Task<int> CountRecentByStatusAsync(int days, string status);
        Task<double> GetSuccessRateAsync(int days);
        Task<List<DailyTrendData>> GetDailyTrendsAsync(int days);
        Task<List<ChartDataPoint>> GetSuccessRateTrendAsync(int days);
        Task<List<ChartDataPoint>> GetExecutionVolumeTrendAsync(int days);
    }

    public interface IJobRepository2
    {
        Task<int> CountActiveAsync();
        Task<List<Job>> GetUpcomingAsync(int count);
    }
}
