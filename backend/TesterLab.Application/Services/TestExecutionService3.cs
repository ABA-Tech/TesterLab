using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TesterLab.Domain.interfaces.Repositories;
using TesterLab.Domain.interfaces.Services;
using TesterLab.Domain.Models;
using Environment = TesterLab.Domain.Models.Environment;

namespace TesterLab.Applications.Services
{
    public class TestExecutionService3 : ITestExecutionService3
    {
        private ITestRunRepository _testRunRepository;
        private ITestCaseRepository _testCaseRepository;
        private IFeatureRepository _featureRepository;
        private IEnvironmentRepository _environmentRepository;
        private ITestDataRepository _testDataRepository;
        private ITestCaseExecutionRepository _testCaseExecutionRepository;
        private ITestStepExecutionRepository _testStepExecutionRepository;
        private IExecutionLogRepository _executionLogRepository;
        private IPerformanceMetricRepository _performanceMetricRepository;
        private IScreenshotRepository _screenshotRepository;
        private ITestExecutor _testExecutor; 
        private IServiceScopeFactory _serviceScopeFactory;
        private ILogger<TestExecutionService> _logger;

        public TestExecutionService3(
            ITestRunRepository testRunRepository,
            ITestCaseRepository testCaseRepository,
            IFeatureRepository featureRepository,
            IEnvironmentRepository environmentRepository,
            ITestDataRepository testDataRepository,
            ITestCaseExecutionRepository testCaseExecutionRepository,
            ITestStepExecutionRepository testStepExecutionRepository,
            IExecutionLogRepository executionLogRepository,
            IPerformanceMetricRepository performanceMetricRepository,
            IScreenshotRepository screenshotRepository,
            ITestExecutor testExecutor,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<TestExecutionService> logger)
        {
            _testRunRepository = testRunRepository;
            _testCaseRepository = testCaseRepository;
            _featureRepository = featureRepository;
            _environmentRepository = environmentRepository;
            _testDataRepository = testDataRepository;
            _testCaseExecutionRepository = testCaseExecutionRepository;
            _testStepExecutionRepository = testStepExecutionRepository;
            _executionLogRepository = executionLogRepository;
            _performanceMetricRepository = performanceMetricRepository;
            _screenshotRepository = screenshotRepository;
            _testExecutor = testExecutor;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public async Task<TestRun> StartTestRunAsync(int testRunId)
        {
            var testRun = await _testRunRepository.GetByIdAsync(testRunId);
            if (testRun == null)
                throw new ArgumentException("Test run not found");

            await LoadTestRunRelationsAsync(testRun);

            testRun.Status = "Running";
            testRun.StartedAt = DateTime.UtcNow;
            testRun.ProgressPercentage = 0;

            await _testRunRepository.UpdateAsync(testRun);

            /*_ = Task.Run(async () => */await ExecuteTestRunAsync(testRun)/*)*/;

            return testRun;
        }

        private async Task ExecuteTestRunAsync(TestRun testRun)
        {
            try
            {

                await LogExecutionAsync(testRun.Id, "Info", $"Starting test run: {testRun.Name}");

                if (testRun.Environment == null)
                {
                    await LoadTestRunRelationsAsync(testRun);
                }

                var testCases = await GetTestCasesToExecute(testRun);
                var totalTests = testCases.Count();

                if (totalTests == 0)
                {
                    await LogExecutionAsync(testRun.Id, "Error", "No test cases found to execute");
                    throw new InvalidOperationException("No test cases found to execute");
                }
                var environment = testRun.Environment != null ? testRun.Environment : throw new InvalidOperationException("Test run must have an environment configured");
                try
                {
                await LogExecutionAsync(testRun.Id, "Info", $"Total tests to execute: {totalTests.ToString()}");
                await LogExecutionAsync(testRun.Id, "Info", $"Environment: "+ environment.Name +" "+environment.BaseUrl);
                await LogExecutionAsync(testRun.Id, "Info", $"Browser: "+testRun.Browser+" (Headless: "+testRun.Headless+")");
                    
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }


                var completedTests = 0;
                var passedCount = 0;
                var failedCount = 0;
                var skippedCount = 0;
                var totalDurationMs = 0L;

                foreach (var testCase in testCases)
                {
                    TestCaseExecution testCaseExecution = null;

                    try
                    {
                        await LogExecutionAsync(testRun.Id, "Info", $"Executing: {testCase.Name}");

                        if (testCase.TestSteps == null || !testCase.TestSteps.Any())
                        {
                            await LogExecutionAsync(testRun.Id, "Warning", $"No test steps found for {testCase.Name}");
                            skippedCount++;
                            completedTests++;
                            continue;
                        }

                        // Créer l'enregistrement d'exécution du test case
                        testCaseExecution = new TestCaseExecution
                        {
                            TestRunId = testRun.Id,
                            TestCaseId = testCase.Id,
                            TestCaseName = testCase.Name,
                            Status = "Running",
                            StartedAt = DateTime.UtcNow,
                            TotalSteps = testCase.TestSteps.Count,
                            Id = 0,
                            CompletedAt = DateTime.UtcNow,
                            ErrorMessage = "",
                            ErrorStackTrace = ""
                            
                        };
                        testCaseExecution = await _testCaseExecutionRepository.CreateAsync(testCaseExecution);

                        // Exécuter le test
                        var result = await _testExecutor.ExecuteTestCaseAsync(testCase, testRun);

                        // Mettre à jour le test case execution
                        testCaseExecution.CompletedAt = DateTime.UtcNow;
                        testCaseExecution.DurationMs = (int)result.Duration.TotalMilliseconds;
                        testCaseExecution.Status = result.Success ? "Passed" : "Failed";
                        testCaseExecution.ErrorMessage = result.ErrorDetails ?? "";
                        testCaseExecution.PassedSteps = result.StepResults.Count(sr => sr.Success);
                        testCaseExecution.FailedSteps = result.StepResults.Count(sr => !sr.Success);

                        await _testCaseExecutionRepository.UpdateAsync(testCaseExecution);

                        // Persister chaque step execution
                        foreach (var stepResult in result.StepResults)
                        {
                            var stepExecution = new TestStepExecution
                            {
                                TestCaseExecutionId = testCaseExecution.Id,
                                TestStepId = stepResult.StepId,
                                StepOrder = stepResult.Order,
                                Action = stepResult.Action,
                                Description = stepResult.Message ?? "",
                                Status = stepResult.Success ? "Passed" : "Failed",
                                StartedAt = DateTime.UtcNow.AddMilliseconds(-stepResult.Duration.TotalMilliseconds),
                                CompletedAt = DateTime.UtcNow,
                                DurationMs = (int)stepResult.Duration.TotalMilliseconds,
                                ErrorMessage = stepResult.ErrorMessage ?? "",
                                ScreenshotPath = stepResult.Screenshot ?? "",
                                Selector = "",
                                Value = ""
                            };

                            await _testStepExecutionRepository.CreateAsync(stepExecution);

                            // Sauvegarder les screenshots
                            if (!string.IsNullOrEmpty(stepResult.Screenshot))
                            {
                                var screenshot = new Screenshot
                                {
                                    TestRunId = testRun.Id,
                                    TestCaseExecutionId = testCaseExecution.Id,
                                    TestStepExecutionId = stepExecution.Id,
                                    FilePath = stepResult.Screenshot ?? "",
                                    Type = stepResult.Success ? "Success" : "Failure",
                                    CapturedAt = DateTime.UtcNow,
                                    Description = $"Step {stepResult.Order}: {stepResult.Action}"
                                };

                                await _screenshotRepository.CreateAsync(screenshot);
                            }
                        }

                        // Sauvegarder les métriques de performance
                        await _performanceMetricRepository.CreateAsync(new PerformanceMetric
                        {
                            TestRunId = testRun.Id,
                            TestCaseExecutionId = testCaseExecution.Id,
                            MetricName = "TestCaseDuration",
                            Value = result.Duration.TotalMilliseconds,
                            Unit = "ms",
                            RecordedAt = DateTime.UtcNow,
                            Context = ""
                        });

                        completedTests++;
                        totalDurationMs += testCaseExecution.DurationMs;

                        if (result.Success)
                        {
                            passedCount++;
                            await LogExecutionAsync(testRun.Id, "Info",
                                $"✅ PASSED - {testCase.Name} ({result.Duration.TotalSeconds:F2}s)",
                                testCaseExecution.Id);
                        }
                        else
                        {
                            failedCount++;
                            await LogExecutionAsync(testRun.Id, "Error",
                                $"❌ FAILED - {testCase.Name}: {result.ErrorDetails}",
                                testCaseExecution.Id);
                        }

                        var progress = (int)((double)completedTests / totalTests * 100);
                        await UpdateTestRunProgressAsync(testRun.Id, progress);

                        await Task.Delay(500);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error executing test case: {testCase.Name}");
                        failedCount++;
                        completedTests++;

                        if (testCaseExecution != null)
                        {
                            testCaseExecution.Status = "Error";
                            testCaseExecution.CompletedAt = DateTime.UtcNow;
                            testCaseExecution.ErrorMessage = ex.Message;
                            testCaseExecution.ErrorStackTrace = ex.StackTrace;
                            await _testCaseExecutionRepository.UpdateAsync(testCaseExecution);
                        }

                        await LogExecutionAsync(testRun.Id, "Error",
                            $"❌ ERROR - {testCase.Name}: {ex.Message}",
                            testCaseExecution?.Id,
                            ex.StackTrace);
                    }
                }

                // Finaliser l'exécution
                await LogExecutionAsync(testRun.Id, "Info", $"Test run completed");
                await LogExecutionAsync(testRun.Id, "Info", $"Results: {passedCount} passed, {failedCount} failed, {skippedCount} skipped");

                var duration = testRun.StartedAt.HasValue
                    ? DateTime.UtcNow - testRun.StartedAt.Value
                    : TimeSpan.Zero;

                await LogExecutionAsync(testRun.Id, "Info", $"Total duration: {duration.TotalMinutes:F2} minutes");

                // Calculer les métriques globales
                var successRate = totalTests > 0 ? (double)passedCount / totalTests * 100 : 0;
                var avgDuration = totalTests > 0 ? (double)totalDurationMs / totalTests : 0;

                var finalStatus = failedCount == 0 ? "Completed" : "Failed";
                var results = JsonSerializer.Serialize(new
                {
                    passed = passedCount,
                    failed = failedCount,
                    skipped = skippedCount,
                    total = totalTests,
                    duration = duration.TotalSeconds,
                    successRate = successRate,
                    averageDurationMs = avgDuration,
                    environment = testRun.Environment.Name,
                    browser = testRun.Browser
                });

                testRun.Status = finalStatus;
                testRun.CompletedAt = DateTime.UtcNow;
                testRun.ProgressPercentage = 100;
                testRun.PassedCount = passedCount;
                testRun.FailedCount = failedCount;
                testRun.SkippedCount = skippedCount;
                testRun.DetailedResults = results;
                testRun.SuccessRate = successRate;
                testRun.AverageDurationMs = avgDuration;

                await _testRunRepository.UpdateAsync(testRun);

                _logger.LogInformation($"Test run completed: {testRun.Name}. Status: {finalStatus}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Fatal error during test run: {testRun.Name}");

                await LogExecutionAsync(testRun.Id, "Error",
                    $"FATAL ERROR: {ex.Message}",
                    null,
                    ex.StackTrace);

                testRun.Status = "Failed";
                testRun.CompletedAt = DateTime.UtcNow;
                testRun.ProgressPercentage = 100;

                await _testRunRepository.UpdateAsync(testRun);
            }
        }

        private async Task LogExecutionAsync(int testRunId, string level, string message, int? testCaseExecutionId = null, string details = null)
        {
            try
            {
                var log = new ExecutionLog
                {
                    TestRunId = testRunId,
                    TestCaseExecutionId = testCaseExecutionId,
                    Timestamp = DateTime.UtcNow,
                    Level = level,
                    Message = message ?? "",
                    Details = details ?? ""
                };

                await _executionLogRepository.CreateAsync(log);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to persist execution log");
            }
        }

        private async Task LoadTestRunRelationsAsync(TestRun testRun)
        {
            if (testRun.EnvironmentId > 0)
            {
                testRun.Environment = await _environmentRepository.GetByIdAsync(testRun.EnvironmentId);
                if (testRun.Environment == null)
                {
                    throw new InvalidOperationException($"Environment with ID {testRun.EnvironmentId} not found");
                }
            }
            else
            {
                throw new InvalidOperationException("Test run must have an environment configured");
            }

            testRun.TestData = await SelectBestTestDataForEnvironmentAsync(testRun);
        }

        private async Task<TestData> SelectBestTestDataForEnvironmentAsync(TestRun testRun)
        {
            if (testRun.TestDataId.HasValue && testRun.TestDataId.Value > 0)
            {
                var specificData = await _testDataRepository.GetByIdAsync(testRun.TestDataId.Value);
                if (specificData != null)
                {
                    _logger.LogInformation($"Using user-selected test data: {specificData.Name}");
                    return specificData;
                }
            }

            var environmentTestData = await _testDataRepository.GetByEnvironmentAsync(testRun.EnvironmentId);
            var bestMatch = environmentTestData.FirstOrDefault();

            if (bestMatch != null)
            {
                _logger.LogInformation($"Auto-selected test data for environment: {bestMatch.Name}");
                return bestMatch;
            }

            var genericData = await _testDataRepository.GetByApplicationIdAsync(testRun.ApplicationId);
            var fallback = genericData.FirstOrDefault(td => td.SpecificEnvironmentId == null);

            if (fallback != null)
            {
                _logger.LogWarning($"Using generic test data (no environment-specific data found): {fallback.Name}");
                return fallback;
            }

            _logger.LogWarning("No test data found - tests will run without data variables");
            return null;
        }

        private async Task<IEnumerable<TestCase>> GetTestCasesToExecute(TestRun testRun)
        {
            var testCases = new List<TestCase>();

            try
            {
                var targetIds = JsonSerializer.Deserialize<int[]>(testRun.TargetIds) ?? Array.Empty<int>();

                if (!targetIds.Any())
                {
                    throw new InvalidOperationException("No target IDs specified for test run");
                }

                _logger.LogInformation($"Loading test cases for execution type: {testRun.ExecutionType}");

                switch (testRun.ExecutionType.ToLower())
                {
                    case "testcase":
                        foreach (var id in targetIds)
                        {
                            var testCase = await _testCaseRepository.GetByIdWithStepsAsync(id);
                            if (testCase != null && testCase.Active)
                            {
                                testCases.Add(testCase);
                            }
                        }
                        break;

                    case "feature":
                        foreach (var featureId in targetIds)
                        {
                            var featureTests = await _testCaseRepository.GetByFeatureIdAsync(featureId);
                            var activeTests = featureTests.Where(tc => tc.Active).ToList();

                            foreach (var testCase in activeTests)
                            {
                                var testCaseWithSteps = await _testCaseRepository.GetByIdWithStepsAsync(testCase.Id);
                                if (testCaseWithSteps != null)
                                {
                                    testCases.Add(testCaseWithSteps);
                                }
                            }
                        }
                        break;

                    case "multiple":
                        foreach (var id in targetIds)
                        {
                            var testCase = await _testCaseRepository.GetByIdWithStepsAsync(id);
                            if (testCase != null && testCase.Active)
                            {
                                testCases.Add(testCase);
                            }
                        }
                        break;

                    default:
                        throw new NotSupportedException($"Execution type '{testRun.ExecutionType}' is not supported");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading test cases to execute");
                throw;
            }

            return testCases.Distinct().ToList();
        }

        private async Task UpdateTestRunProgressAsync(int testRunId, int progress)
        {
            try
            {
                var testRun = await _testRunRepository.GetByIdAsync(testRunId);
                if (testRun != null)
                {
                    testRun.ProgressPercentage = Math.Min(100, Math.Max(0, progress));
                    await _testRunRepository.UpdateAsync(testRun);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to update test run progress");
            }
        }

        public async Task<TestRun> GetTestRunByIdAsync(int id)
        {
            var testRun = await _testRunRepository.GetByIdAsync(id);
            if (testRun != null)
            {
                await LoadTestRunRelationsAsync(testRun);
            }
            return testRun;
        }

        public async Task<TestRun> CreateTestRunAsync(TestRun testRun)
        {
            if (testRun.ApplicationId <= 0)
            {
                throw new ArgumentException("Application is required");
            }

            var environment = await _environmentRepository.GetByIdAsync(testRun.EnvironmentId);
            if (environment == null)
            {
                throw new ArgumentException($"Environment with ID {testRun.EnvironmentId} not found");
            }

            if (environment.ApplicationId != testRun.ApplicationId)
            {
                throw new ArgumentException($"Environment {environment.Name} does not belong to the selected application");
            }

            if (!environment.Active)
            {
                throw new ArgumentException($"Environment {environment.Name} is not active");
            }

            if (testRun.TestDataId.HasValue && testRun.TestDataId.Value > 0)
            {
                var testData = await _testDataRepository.GetByIdAsync(testRun.TestDataId.Value);
                if (testData == null)
                {
                    throw new ArgumentException($"Test data with ID {testRun.TestDataId.Value} not found");
                }

                if (testData.ApplicationId != testRun.ApplicationId)
                {
                    throw new ArgumentException($"Test data {testData.Name} does not belong to the selected application");
                }
            }

            if (string.IsNullOrEmpty(testRun.TargetIds))
            {
                throw new ArgumentException("Target IDs are required");
            }

            testRun.Status = "Created";
            testRun.ProgressPercentage = 0;
            testRun.CreatedAt = DateTime.UtcNow;

            return await _testRunRepository.CreateAsync(testRun);
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

            return await _testRunRepository.UpdateAsync(testRun);
        }

        // Nouvelle méthode qui crée son propre scope
        private async Task ExecuteTestRunInBackgroundAsync(int testRunId)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            // Récupérer les services depuis le nouveau scope
            var testRunRepository = scope.ServiceProvider.GetRequiredService<ITestRunRepository>();
            var testCaseRepository = scope.ServiceProvider.GetRequiredService<ITestCaseRepository>();
            var environmentRepository = scope.ServiceProvider.GetRequiredService<IEnvironmentRepository>();
            var testDataRepository = scope.ServiceProvider.GetRequiredService<ITestDataRepository>();
            var testCaseExecutionRepository = scope.ServiceProvider.GetRequiredService<ITestCaseExecutionRepository>();
            var testStepExecutionRepository = scope.ServiceProvider.GetRequiredService<ITestStepExecutionRepository>();
            var executionLogRepository = scope.ServiceProvider.GetRequiredService<IExecutionLogRepository>();
            var performanceMetricRepository = scope.ServiceProvider.GetRequiredService<IPerformanceMetricRepository>();
            var screenshotRepository = scope.ServiceProvider.GetRequiredService<IScreenshotRepository>();
            var testExecutor = scope.ServiceProvider.GetRequiredService<ITestExecutor>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<TestExecutionService>>();

            try
            {
                var testRun = await testRunRepository.GetByIdAsync(testRunId);
                if (testRun == null)
                {
                    logger.LogError($"TestRun {testRunId} not found for background execution");
                    return;
                }

               ExecuteTestRunWithServicesAsync(
                    testRun,
                    testRunRepository,
                    testCaseRepository,
                    environmentRepository,
                    testDataRepository,
                    testCaseExecutionRepository,
                    testStepExecutionRepository,
                    executionLogRepository,
                    performanceMetricRepository,
                    screenshotRepository,
                    testExecutor,
                    logger
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Fatal error in background execution for TestRun {testRunId}");
            }
        }

        private void ExecuteTestRunWithServicesAsync(TestRun testRun, ITestRunRepository testRunRepository, ITestCaseRepository testCaseRepository, IEnvironmentRepository environmentRepository, ITestDataRepository testDataRepository, ITestCaseExecutionRepository testCaseExecutionRepository, ITestStepExecutionRepository testStepExecutionRepository, IExecutionLogRepository executionLogRepository, IPerformanceMetricRepository performanceMetricRepository, IScreenshotRepository screenshotRepository, ITestExecutor testExecutor, ILogger<TestExecutionService> logger)
        {
            _testRunRepository = testRunRepository;
            _testCaseRepository = testCaseRepository;
            _environmentRepository = environmentRepository;
            _testDataRepository = testDataRepository;
            _testCaseExecutionRepository = testCaseExecutionRepository;
            _testStepExecutionRepository = testStepExecutionRepository;
            _executionLogRepository = executionLogRepository;
            _performanceMetricRepository = performanceMetricRepository;
            _screenshotRepository = screenshotRepository;
            _testExecutor = testExecutor;
            _logger = logger;
        }

        public async Task<IEnumerable<Environment>> GetEnvironmentsByApplicationAsync(int applicationId)
        {
            return await _environmentRepository.GetByApplicationIdAsync(applicationId);
        }

        public async Task<IEnumerable<TestData>> GetTestDataByApplicationAsync(int applicationId)
        {
            return await _testDataRepository.GetByApplicationIdAsync(applicationId);
        }

        public async Task<IEnumerable<TestData>> GetTestDataByEnvironmentAsync(int environmentId)
        {
            return await _testDataRepository.GetByEnvironmentAsync(environmentId);
        }

        public async Task<TestData> GetBestTestDataForEnvironmentAsync(int applicationId, int environmentId)
        {
            var environmentData = await _testDataRepository.GetByEnvironmentAsync(environmentId);
            var specificData = environmentData.FirstOrDefault();

            if (specificData != null)
            {
                return specificData;
            }

            var allAppData = await _testDataRepository.GetByApplicationIdAsync(applicationId);
            return allAppData.FirstOrDefault(td => td.SpecificEnvironmentId == null);
        }

        // Nouvelles méthodes pour les dashboards
        public async Task<IEnumerable<TestCaseExecution>> GetTestCaseExecutionsByRunIdAsync(int testRunId)
        {
            return await _testCaseExecutionRepository.GetByTestRunIdAsync(testRunId);
        }

        // Nouvelles méthodes pour les dashboards
        public async Task<IEnumerable<TestCaseExecution>> GetTestCaseExecutionsByApplicationAsync(int applicatonId)
        {
            return await _testCaseExecutionRepository.GetByTestApplicationIdAsync(applicatonId);
        }

        public async Task<IEnumerable<ExecutionLog>> GetExecutionLogsByRunIdAsync(int testRunId)
        {
            return await _executionLogRepository.GetByTestRunIdAsync(testRunId);
        }

        public async Task<IEnumerable<PerformanceMetric>> GetPerformanceMetricsByRunIdAsync(int testRunId)
        {
            return await _performanceMetricRepository.GetByTestRunIdAsync(testRunId);
        }

        public async Task<IEnumerable<Screenshot>> GetScreenshotsByRunIdAsync(int testRunId)
        {
            return await _screenshotRepository.GetByTestRunIdAsync(testRunId);
        }

        Task<IEnumerable<Domain.Models.Environment>> ITestExecutionService3.GetEnvironmentsByApplicationAsync(int applicationId)
        {
            throw new NotImplementedException();
        }
    }
}
