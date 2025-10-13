using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TesterLab.Domain.interfaces.Repositories;
using TesterLab.Domain.interfaces.Services;
using TesterLab.Domain.Models;

namespace TesterLab.Applications.Services
{
    public class TestExecutionService2 : ITestExecutionService2
    {
        private readonly ITestRunRepository _testRunRepository;
        private readonly ITestCaseRepository _testCaseRepository;
        private readonly IFeatureRepository _featureRepository;
        private readonly IEnvironmentRepository _environmentRepository;
        private readonly ITestDataRepository _testDataRepository;
        private readonly ITestExecutor _testExecutor;
        private readonly ILogger<TestExecutionService2> _logger;

        public TestExecutionService2(
            ITestRunRepository testRunRepository,
            ITestCaseRepository testCaseRepository,
            IFeatureRepository featureRepository,
            IEnvironmentRepository environmentRepository,
            ITestDataRepository testDataRepository,
            ITestExecutor testExecutor,
            ILogger<TestExecutionService2> logger)
        {
            _testRunRepository = testRunRepository;
            _testCaseRepository = testCaseRepository;
            _featureRepository = featureRepository;
            _environmentRepository = environmentRepository;
            _testDataRepository = testDataRepository;
            _testExecutor = testExecutor;
            _logger = logger;
        }

        public async Task<TestRun> StartTestRunAsync(int testRunId)
        {
            var testRun = await _testRunRepository.GetByIdAsync(testRunId);
            if (testRun == null)
                throw new ArgumentException("Test run not found");

            // Charger les relations nécessaires
            await LoadTestRunRelationsAsync(testRun);

            testRun.Status = "Running";
            testRun.StartedAt = DateTime.UtcNow;
            testRun.ProgressPercentage = 0;

            await _testRunRepository.UpdateAsync(testRun);

            // Lancer l'exécution en arrière-plan
            /*_ = Task.Run(async () => */await ExecuteTestRunAsync(testRun)/*)*/;

            return testRun;
        }

        private async Task LoadTestRunRelationsAsync(TestRun testRun)
        {
            // Charger l'environnement
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

            // Charger les données de test si spécifiées
            if (testRun.TestDataId.HasValue && testRun.TestDataId.Value > 0)
            {
                testRun.TestData = await _testDataRepository.GetByIdAsync(testRun.TestDataId.Value);
            }
        }

        private async Task ExecuteTestRunAsync(TestRun testRun)
        {
            try
            {
                _logger.LogInformation($"Starting test run: {testRun.Name}");

                // Vérifier que l'environnement est chargé
                if (testRun.Environment == null)
                {
                    await LoadTestRunRelationsAsync(testRun);
                }

                // Récupérer les test cases à exécuter
                var testCases = await GetTestCasesToExecute(testRun);

                var totalTests = testCases.Count();

                if (totalTests == 0)
                {
                    throw new InvalidOperationException("No test cases found to execute");
                }

                var completedTests = 0;
                var passedCount = 0;
                var failedCount = 0;
                var skippedCount = 0;

                var logs = new System.Text.StringBuilder();
                logs.AppendLine($"Starting test run at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
                logs.AppendLine($"Environment: {testRun.Environment.Name} ({testRun.Environment.BaseUrl})");
                logs.AppendLine($"Browser: {testRun.Browser} (Headless: {testRun.Headless})");
                logs.AppendLine($"Total tests to execute: {totalTests}");
                logs.AppendLine("---");

                foreach (var testCase in testCases)
                {
                    try
                    {
                        logs.AppendLine($"\n[{DateTime.UtcNow:HH:mm:ss}] Executing: {testCase.Name}");
                        _logger.LogInformation($"Executing test case: {testCase.Name} (ID: {testCase.Id})");

                        // Vérifier que les steps sont chargés
                        if (testCase.TestSteps == null || !testCase.TestSteps.Any())
                        {
                            logs.AppendLine($"⚠️ SKIPPED - No test steps found for {testCase.Name}");
                            skippedCount++;
                            completedTests++;
                            continue;
                        }

                        // Exécuter le test
                        var result = await _testExecutor.ExecuteTestCaseAsync(testCase, testRun);

                        completedTests++;

                        if (result.Success)
                        {
                            passedCount++;
                            logs.AppendLine($"✅ PASSED - {testCase.Name} ({result.Duration.TotalSeconds:F2}s)");
                            logs.AppendLine($"   Steps executed: {result.StepResults.Count}");
                        }
                        else
                        {
                            failedCount++;
                            logs.AppendLine($"❌ FAILED - {testCase.Name}");
                            logs.AppendLine($"   Error: {result.ErrorDetails}");

                            // Ajouter les détails des steps échoués
                            var failedSteps = result.StepResults.Where(sr => !sr.Success).ToList();
                            if (failedSteps.Any())
                            {
                                logs.AppendLine($"   Failed steps:");
                                foreach (var failedStep in failedSteps)
                                {
                                    logs.AppendLine($"     - Step {failedStep.Order}: {failedStep.ErrorMessage}");
                                }
                            }
                        }

                        // Mettre à jour la progression
                        var progress = (int)((double)completedTests / totalTests * 100);
                        await UpdateTestRunProgressAsync(testRun.Id, progress, logs.ToString());

                        // Petit délai entre les tests
                        await Task.Delay(500);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error executing test case: {testCase.Name}");
                        failedCount++;
                        completedTests++;
                        logs.AppendLine($"❌ ERROR - {testCase.Name}: {ex.Message}");
                        logs.AppendLine($"   Stack trace: {ex.StackTrace}");
                    }
                }

                // Finaliser l'exécution
                logs.AppendLine("\n---");
                logs.AppendLine($"Test run completed at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
                logs.AppendLine($"Results: {passedCount} passed, {failedCount} failed, {skippedCount} skipped");

                var duration = testRun.StartedAt.HasValue
                    ? DateTime.UtcNow - testRun.StartedAt.Value
                    : TimeSpan.Zero;
                logs.AppendLine($"Total duration: {duration.TotalMinutes:F2} minutes");

                var finalStatus = failedCount == 0 ? "Completed" : "Failed";
                var results = JsonSerializer.Serialize(new
                {
                    passed = passedCount,
                    failed = failedCount,
                    skipped = skippedCount,
                    total = totalTests,
                    duration = duration.TotalSeconds,
                    environment = testRun.Environment.Name,
                    browser = testRun.Browser
                });

                testRun.Status = finalStatus;
                testRun.CompletedAt = DateTime.UtcNow;
                testRun.ProgressPercentage = 100;
                testRun.PassedCount = passedCount;
                testRun.FailedCount = failedCount;
                testRun.SkippedCount = skippedCount;
                testRun.ExecutionLogs = logs.ToString();
                testRun.DetailedResults = results;

                await _testRunRepository.UpdateAsync(testRun);

                _logger.LogInformation($"Test run completed: {testRun.Name}. Status: {finalStatus}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Fatal error during test run: {testRun.Name}");

                testRun.Status = "Failed";
                testRun.CompletedAt = DateTime.UtcNow;
                testRun.ProgressPercentage = 100;
                testRun.ExecutionLogs += $"\n\nFATAL ERROR: {ex.Message}\n{ex.StackTrace}";

                await _testRunRepository.UpdateAsync(testRun);
            }
        }

        private async Task<IEnumerable<TestCase>> GetTestCasesToExecute(TestRun testRun)
        {
            var testCases = new List<TestCase>();

            try
            {
                // Parser les IDs cibles
                //var targetIds = JsonSerializer.Deserialize<int[]>(testRun.TargetIds) ?? Array.Empty<int>();
                var targetIds = testRun.TargetIds?.Split(',').Select(int.Parse).ToArray() ?? Array.Empty<int>();

                if (!targetIds.Any())
                {
                    throw new InvalidOperationException("No target IDs specified for test run");
                }

                _logger.LogInformation($"Loading test cases for execution type: {testRun.ExecutionType}");
                _logger.LogInformation($"Target IDs: {string.Join(", ", targetIds)}");

                switch (testRun.ExecutionType.ToLower())
                {
                    case "testcase":
                        // Exécuter un ou plusieurs test cases spécifiques
                        //foreach (var id in targetIds)
                        //{
                        //    var testCase = await _testCaseRepository.GetByIdWithStepsAsync(id);
                        //    if (testCase != null && testCase.Active)
                        //    {
                        //        _logger.LogInformation($"Added test case: {testCase.Name} with {testCase.TestSteps?.Count ?? 0} steps");
                        //        testCases.Add(testCase);
                        //    }
                        //    else
                        //    {
                        //        _logger.LogWarning($"Test case {id} not found or inactive");
                        //    }
                        //}

                        testCases = (await _testCaseRepository.GetByTagsAsync(testRun.TargetIds?.Split(','))).ToList();
                        break;

                    case "feature":
                        // Exécuter tous les tests d'une ou plusieurs features
                        foreach (var featureId in targetIds)
                        {
                            var featureTests = await _testCaseRepository.GetByFeatureIdAsync(featureId);
                            var activeTests = featureTests.Where(tc => tc.Active).ToList();

                            // Charger les steps pour chaque test case
                            foreach (var testCase in activeTests)
                            {
                                var testCaseWithSteps = await _testCaseRepository.GetByIdWithStepsAsync(testCase.Id);
                                if (testCaseWithSteps != null)
                                {
                                    _logger.LogInformation($"Added test case from feature: {testCaseWithSteps.Name}");
                                    testCases.Add(testCaseWithSteps);
                                }
                            }
                        }
                        break;

                    case "multiple":
                        // Exécuter une sélection personnalisée de tests
                        foreach (var id in targetIds)
                        {
                            var testCase = await _testCaseRepository.GetByIdWithStepsAsync(id);
                            if (testCase != null && testCase.Active)
                            {
                                _logger.LogInformation($"Added test case: {testCase.Name}");
                                testCases.Add(testCase);
                            }
                        }
                        break;

                    default:
                        throw new NotSupportedException($"Execution type '{testRun.ExecutionType}' is not supported");
                }

                _logger.LogInformation($"Total test cases loaded: {testCases.Count}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading test cases to execute");
                throw;
            }

            return testCases.Distinct().ToList();
        }

        private async Task UpdateTestRunProgressAsync(int testRunId, int progress, string logs)
        {
            try
            {
                var testRun = await _testRunRepository.GetByIdAsync(testRunId);
                if (testRun != null)
                {
                    testRun.ProgressPercentage = Math.Min(100, Math.Max(0, progress));
                    testRun.ExecutionLogs = logs;
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
            // Valider que l'environnement existe
            var environment = await _environmentRepository.GetByIdAsync(testRun.EnvironmentId);
            if (environment == null)
            {
                throw new ArgumentException("Environment not found");
            }

            // Valider que les test data existent si spécifiés
            if (testRun.TestDataId.HasValue)
            {
                var testData = await _testDataRepository.GetByIdAsync(testRun.TestDataId.Value);
                if (testData == null)
                {
                    throw new ArgumentException("Test data not found");
                }
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
    }

    public interface ITestExecutionService2
    {
        Task<TestRun> StartTestRunAsync(int testRunId);
        Task<TestRun> GetTestRunByIdAsync(int id);
        Task<TestRun> CreateTestRunAsync(TestRun testRun);
        Task<TestRun> CompleteTestRunAsync(int testRunId, string status, string results);
    }

}
