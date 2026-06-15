using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text.Json;
using TesterLab.Domain.interfaces.Repositories;
using TesterLab.Domain.interfaces.Services;
using TesterLab.Domain.Models;
using Environment = TesterLab.Domain.Models.Environment;

namespace TesterLab.Applications.Services
{
    /// <summary>
    /// Service d'exécution de tests optimisé pour les exécutions parallèles et simultanées
    /// Thread-safe avec gestion appropriée des scopes et transactions
    /// </summary>
    public class TestExecutionService3 : ITestExecutionService3
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<TestExecutionService3> _logger;

        // Dictionnaire thread-safe pour suivre les exécutions en cours
        private static readonly ConcurrentDictionary<int, CancellationTokenSource> _runningExecutions = new();

        public TestExecutionService3(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<TestExecutionService3> logger)
        {
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region Public API Methods

        public async Task<IEnumerable<TestCaseExecution>> GetTestRunByTestCaseIdAsync(int idTestCase)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var testRunRepository = scope.ServiceProvider.GetRequiredService<ITestRunRepository>();

            return await testRunRepository.GetByTestCaseIdAsync(idTestCase);
        }

        /// <summary>
        /// Démarre l'exécution d'un test run de manière asynchrone en arrière-plan
        /// </summary>
        public async Task<TestRun> StartTestRunAsync(int testRunId)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var testRunRepository = scope.ServiceProvider.GetRequiredService<ITestRunRepository>();
            var environmentRepository = scope.ServiceProvider.GetRequiredService<IEnvironmentRepository>();
            var testDataRepository = scope.ServiceProvider.GetRequiredService<ITestDataRepository>();

            var testRun = await testRunRepository.GetByIdAsync(testRunId);
            if (testRun == null)
                throw new ArgumentException($"Test run with ID {testRunId} not found");

            // Vérifier si une exécution est déjà en cours
            if (_runningExecutions.ContainsKey(testRunId))
            {
                throw new InvalidOperationException($"Test run {testRunId} is already running");
            }

            // Charger les relations nécessaires
            await LoadTestRunRelationsAsync(testRun, environmentRepository, testDataRepository, testRun.ApplicationId);

            // Mettre à jour le statut
            testRun.Status = "Running";
            testRun.StartedAt = DateTime.UtcNow;
            testRun.ProgressPercentage = 0;
            testRun.CompletedAt = null;
            testRun.PassedCount = 0;
            testRun.FailedCount = 0;
            testRun.SkippedCount = 0;

            await testRunRepository.UpdateAsync(testRun);

            // Créer un token d'annulation pour cette exécution
            var cts = new CancellationTokenSource();
            _runningExecutions.TryAdd(testRunId, cts);

            // Lancer l'exécution en arrière-plan
            _ = Task.Run(() => ExecuteTestRunInBackgroundAsync(testRunId, cts.Token), cts.Token);

            _logger.LogInformation($"Test run {testRunId} ({testRun.Name}) started successfully");

            return testRun;
        }

        /// <summary>
        /// Annule une exécution en cours
        /// </summary>
        public async Task<bool> CancelTestRunAsync(int testRunId)
        {
            if (_runningExecutions.TryRemove(testRunId, out var cts))
            {
                cts.Cancel();
                _logger.LogWarning($"Test run {testRunId} cancellation requested");

                // Mettre à jour le statut dans la base de données
                using var scope = _serviceScopeFactory.CreateScope();
                var testRunRepository = scope.ServiceProvider.GetRequiredService<ITestRunRepository>();
                var testRun = await testRunRepository.GetByIdAsync(testRunId);

                if (testRun != null)
                {
                    testRun.Status = "Cancelled";
                    testRun.CompletedAt = DateTime.UtcNow;
                    testRun.ProgressPercentage = testRun.ProgressPercentage; // Garder la progression actuelle
                    await testRunRepository.UpdateAsync(testRun);
                }

                return true;
            }

            return false;
        }

        public async Task<TestRun> GetTestRunByIdAsync(int id)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var testRunRepository = scope.ServiceProvider.GetRequiredService<ITestRunRepository>();
            var environmentRepository = scope.ServiceProvider.GetRequiredService<IEnvironmentRepository>();
            var testDataRepository = scope.ServiceProvider.GetRequiredService<ITestDataRepository>();

            var testRun = await testRunRepository.GetByIdAsync(id);
            if (testRun != null)
            {
                await LoadTestRunRelationsAsync(testRun, environmentRepository, testDataRepository, testRun.ApplicationId);
            }
            return testRun;
        }

        public async Task<TestRun> CreateTestRunAsync(TestRun testRun)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var testRunRepository = scope.ServiceProvider.GetRequiredService<ITestRunRepository>();
            var environmentRepository = scope.ServiceProvider.GetRequiredService<IEnvironmentRepository>();
            var testDataRepository = scope.ServiceProvider.GetRequiredService<ITestDataRepository>();

            // Validations
            if (testRun.ApplicationId <= 0)
                throw new ArgumentException("Application is required");

            if (string.IsNullOrEmpty(testRun.TargetIds))
                throw new ArgumentException("Target IDs are required");

            var environment = await environmentRepository.GetByIdAsync(testRun.EnvironmentId);
            if (environment == null)
                throw new ArgumentException($"Environment with ID {testRun.EnvironmentId} not found");

            if (environment.ApplicationId != testRun.ApplicationId)
                throw new ArgumentException($"Environment {environment.Name} does not belong to the selected application");

            if (!environment.Active)
                throw new ArgumentException($"Environment {environment.Name} is not active");

            if (testRun.TestDataId.HasValue && testRun.TestDataId.Value > 0)
            {
                var testData = await testDataRepository.GetByIdAsync(testRun.TestDataId.Value);
                if (testData == null)
                    throw new ArgumentException($"Test data with ID {testRun.TestDataId.Value} not found");

                if (testData.ApplicationId != testRun.ApplicationId)
                    throw new ArgumentException($"Test data {testData.Name} does not belong to the selected application");
            }

            // Initialiser les valeurs par défaut
            testRun.Status = "Created";
            testRun.ProgressPercentage = 0;
            testRun.CreatedAt = DateTime.UtcNow;
            testRun.PassedCount = 0;
            testRun.FailedCount = 0;
            testRun.SkippedCount = 0;

            var createdTestRun = await testRunRepository.CreateAsync(testRun);
            _logger.LogInformation($"Test run {createdTestRun.Id} ({createdTestRun.Name}) created successfully");

            return createdTestRun;
        }

        public async Task<TestRun> CompleteTestRunAsync(int testRunId, string status, string results)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var testRunRepository = scope.ServiceProvider.GetRequiredService<ITestRunRepository>();

            var testRun = await testRunRepository.GetByIdAsync(testRunId);
            if (testRun == null)
                throw new ArgumentException($"Test run with ID {testRunId} not found");

            testRun.Status = status;
            testRun.CompletedAt = DateTime.UtcNow;
            testRun.ProgressPercentage = 100;
            testRun.DetailedResults = results;

            return await testRunRepository.UpdateAsync(testRun);
        }

        public async Task<IEnumerable<Environment>> GetEnvironmentsByApplicationAsync(int applicationId)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var environmentRepository = scope.ServiceProvider.GetRequiredService<IEnvironmentRepository>();
            return await environmentRepository.GetByApplicationIdAsync(applicationId);
        }

        public async Task<IEnumerable<TestData>> GetTestDataByApplicationAsync(int applicationId)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var testDataRepository = scope.ServiceProvider.GetRequiredService<ITestDataRepository>();
            return await testDataRepository.GetByApplicationIdAsync(applicationId);
        }

        public async Task<IEnumerable<TestData>> GetTestDataByEnvironmentAsync(int environmentId)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var testDataRepository = scope.ServiceProvider.GetRequiredService<ITestDataRepository>();
            return await testDataRepository.GetByEnvironmentAsync(environmentId);
        }

        public async Task<TestData> GetBestTestDataForEnvironmentAsync(int applicationId, int environmentId)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var testDataRepository = scope.ServiceProvider.GetRequiredService<ITestDataRepository>();

            var environmentData = await testDataRepository.GetByEnvironmentAsync(environmentId);
            var specificData = environmentData.FirstOrDefault();

            if (specificData != null)
                return specificData;

            var allAppData = await testDataRepository.GetByApplicationIdAsync(applicationId);
            return allAppData.FirstOrDefault(td => td.SpecificEnvironmentId == null);
        }

        public async Task<IEnumerable<TestCaseExecution>> GetTestCaseExecutionsByRunIdAsync(int testRunId)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var testCaseExecutionRepository = scope.ServiceProvider.GetRequiredService<ITestCaseExecutionRepository>();
            return await testCaseExecutionRepository.GetByTestRunIdAsync(testRunId);
        }

        public async Task<IEnumerable<TestCaseExecution>> GetTestCaseExecutionsByApplicationAsync(int applicationId)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var testCaseExecutionRepository = scope.ServiceProvider.GetRequiredService<ITestCaseExecutionRepository>();
            return await testCaseExecutionRepository.GetByTestApplicationIdAsync(applicationId);
        }

        public async Task<IEnumerable<ExecutionLog>> GetExecutionLogsByRunIdAsync(int testRunId)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var executionLogRepository = scope.ServiceProvider.GetRequiredService<IExecutionLogRepository>();
            return await executionLogRepository.GetByTestRunIdAsync(testRunId);
        }

        public async Task<IEnumerable<PerformanceMetric>> GetPerformanceMetricsByRunIdAsync(int testRunId)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var performanceMetricRepository = scope.ServiceProvider.GetRequiredService<IPerformanceMetricRepository>();
            return await performanceMetricRepository.GetByTestRunIdAsync(testRunId);
        }

        public async Task<IEnumerable<Screenshot>> GetScreenshotsByRunIdAsync(int testRunId)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var screenshotRepository = scope.ServiceProvider.GetRequiredService<IScreenshotRepository>();
            return await screenshotRepository.GetByTestRunIdAsync(testRunId);
        }

        #endregion

        #region Private Execution Methods

        /// <summary>
        /// Exécute le test run en arrière-plan avec son propre scope de services
        /// Cette méthode est thread-safe et peut être exécutée en parallèle
        /// </summary>
        private async Task ExecuteTestRunInBackgroundAsync(int testRunId, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            // Récupérer TOUS les services depuis le scope local (thread-safe)
            var testRunRepository = scope.ServiceProvider.GetRequiredService<ITestRunRepository>();
            var testCaseRepository = scope.ServiceProvider.GetRequiredService<ITestCaseRepository>();
            var featureRepository = scope.ServiceProvider.GetRequiredService<IFeatureRepository>();
            var environmentRepository = scope.ServiceProvider.GetRequiredService<IEnvironmentRepository>();
            var testDataRepository = scope.ServiceProvider.GetRequiredService<ITestDataRepository>();
            var testCaseExecutionRepository = scope.ServiceProvider.GetRequiredService<ITestCaseExecutionRepository>();
            var testStepExecutionRepository = scope.ServiceProvider.GetRequiredService<ITestStepExecutionRepository>();
            var executionLogRepository = scope.ServiceProvider.GetRequiredService<IExecutionLogRepository>();
            var performanceMetricRepository = scope.ServiceProvider.GetRequiredService<IPerformanceMetricRepository>();
            var screenshotRepository = scope.ServiceProvider.GetRequiredService<IScreenshotRepository>();
            var testExecutor = scope.ServiceProvider.GetRequiredService<ITestExecutor>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<TestExecutionService3>>();

            try
            {
                var testRun = await testRunRepository.GetByIdAsync(testRunId);
                if (testRun == null)
                {
                    logger.LogError($"TestRun {testRunId} not found for background execution");
                    return;
                }

                await ExecuteTestRunAsync(
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
                    logger,
                    cancellationToken);
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning($"Test run {testRunId} was cancelled");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Fatal error in background execution for TestRun {testRunId}");
            }
            finally
            {
                // Nettoyer le suivi de l'exécution
                _runningExecutions.TryRemove(testRunId, out _);
            }
        }

        /// <summary>
        /// Exécute le test run avec les services fournis
        /// Méthode pure sans état partagé - thread-safe
        /// </summary>
        private async Task ExecuteTestRunAsync(
            TestRun testRun,
            ITestRunRepository testRunRepository,
            ITestCaseRepository testCaseRepository,
            IEnvironmentRepository environmentRepository,
            ITestDataRepository testDataRepository,
            ITestCaseExecutionRepository testCaseExecutionRepository,
            ITestStepExecutionRepository testStepExecutionRepository,
            IExecutionLogRepository executionLogRepository,
            IPerformanceMetricRepository performanceMetricRepository,
            IScreenshotRepository screenshotRepository,
            ITestExecutor testExecutor,
            ILogger<TestExecutionService3> logger,
            CancellationToken cancellationToken)
        {
            try
            {
                await LogExecutionAsync(executionLogRepository, logger, testRun.Id, "Info",
                    $"Starting test run: {testRun.Name}");

                // Charger les relations si nécessaire
                if (testRun.Environment == null)
                {
                    await LoadTestRunRelationsAsync(testRun, environmentRepository, testDataRepository, testRun.ApplicationId);
                }

                // Récupérer les test cases à exécuter
                var testCases = await GetTestCasesToExecuteAsync(
                    testRun,
                    testCaseRepository,
                    logger);

                var totalTests = testCases.Count();

                if (totalTests == 0)
                {
                    await LogExecutionAsync(executionLogRepository, logger, testRun.Id, "Error",
                        "No test cases found to execute");
                    throw new InvalidOperationException("No test cases found to execute");
                }

                var environment = testRun.Environment ??
                    throw new InvalidOperationException("Test run must have an environment configured");

                // Logs d'information
                await LogExecutionAsync(executionLogRepository, logger, testRun.Id, "Info",
                    $"Total tests to execute: {totalTests}");
                await LogExecutionAsync(executionLogRepository, logger, testRun.Id, "Info",
                    $"Environment: {environment.Name} {environment.BaseUrl}");
                await LogExecutionAsync(executionLogRepository, logger, testRun.Id, "Info",
                    $"Browser: {testRun.Browser} (Headless: {testRun.Headless})");

                // Métriques d'exécution
                var completedTests = 0;
                var passedCount = 0;
                var failedCount = 0;
                var skippedCount = 0;
                var totalDurationMs = 0L;

                // Exécution séquentielle des test cases
                foreach (var testCase in testCases)
                {
                    // Vérifier l'annulation
                    cancellationToken.ThrowIfCancellationRequested();

                    TestCaseExecution testCaseExecution = null;

                    try
                    {
                        await LogExecutionAsync(executionLogRepository, logger, testRun.Id, "Info",
                            $"Executing: {testCase.Name}");

                        if (testCase.TestSteps == null || !testCase.TestSteps.Any())
                        {
                            await LogExecutionAsync(executionLogRepository, logger, testRun.Id, "Warning",
                                $"No test steps found for {testCase.Name}");
                            skippedCount++;
                            completedTests++;
                            continue;
                        }

                        // Créer l'enregistrement d'exécution
                        testCaseExecution = new TestCaseExecution
                        {
                            TestRunId = testRun.Id,
                            TestCaseId = testCase.Id,
                            TestCaseName = testCase.Name,
                            Status = "Running",
                            StartedAt = DateTime.UtcNow,
                            TotalSteps = testCase.TestSteps.Count,
                            PassedSteps = 0,
                            FailedSteps = 0,
                            DurationMs = 0,
                            ErrorMessage = string.Empty,
                            ErrorStackTrace = string.Empty
                        };

                        testCaseExecution = await testCaseExecutionRepository.CreateAsync(testCaseExecution);

                        // Exécuter le test case
                        var result = await testExecutor.ExecuteTestCaseAsync(testCase, testRun);

                        // Mettre à jour l'exécution du test case
                        testCaseExecution.CompletedAt = DateTime.UtcNow;
                        testCaseExecution.DurationMs = (int)result.Duration.TotalMilliseconds;
                        testCaseExecution.Status = result.Success ? "Passed" : "Failed";
                        testCaseExecution.ErrorMessage = result.ErrorDetails ?? string.Empty;
                        testCaseExecution.PassedSteps = result.StepResults?.Count(sr => sr.Success) ?? 0;
                        testCaseExecution.FailedSteps = result.StepResults?.Count(sr => !sr.Success) ?? 0;

                        await testCaseExecutionRepository.UpdateAsync(testCaseExecution);

                        // Persister les résultats des steps
                        if (result.StepResults != null)
                        {
                            await SaveStepExecutionsAsync(
                                result.StepResults,
                                testCaseExecution.Id,
                                testRun.Id,
                                testStepExecutionRepository,
                                screenshotRepository);
                        }

                        // Sauvegarder les métriques de performance
                        await performanceMetricRepository.CreateAsync(new PerformanceMetric
                        {
                            TestRunId = testRun.Id,
                            TestCaseExecutionId = testCaseExecution.Id,
                            MetricName = "TestCaseDuration",
                            Value = result.Duration.TotalMilliseconds,
                            Unit = "ms",
                            RecordedAt = DateTime.UtcNow,
                            Context = string.Empty
                        });

                        completedTests++;
                        totalDurationMs += testCaseExecution.DurationMs;

                        if (result.Success)
                        {
                            passedCount++;
                            await LogExecutionAsync(executionLogRepository, logger, testRun.Id, "Info",
                                $"✅ PASSED - {testCase.Name} ({result.Duration.TotalSeconds:F2}s)",
                                testCaseExecution.Id);
                        }
                        else
                        {
                            failedCount++;
                            await LogExecutionAsync(executionLogRepository, logger, testRun.Id, "Error",
                                $"❌ FAILED - {testCase.Name}: {result.ErrorDetails}",
                                testCaseExecution.Id);
                        }

                        // Mettre à jour la progression
                        var progress = (int)((double)completedTests / totalTests * 100);
                        await UpdateTestRunProgressAsync(testRunRepository, logger, testRun.Id, progress);

                        // Petit délai pour éviter la surcharge
                        await Task.Delay(100, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        throw; // Propager l'annulation
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"Error executing test case: {testCase.Name}");
                        failedCount++;
                        completedTests++;

                        if (testCaseExecution != null)
                        {
                            testCaseExecution.Status = "Error";
                            testCaseExecution.CompletedAt = DateTime.UtcNow;
                            testCaseExecution.ErrorMessage = ex.Message;
                            testCaseExecution.ErrorStackTrace = ex.StackTrace ?? string.Empty;
                            await testCaseExecutionRepository.UpdateAsync(testCaseExecution);
                        }

                        await LogExecutionAsync(executionLogRepository, logger, testRun.Id, "Error",
                            $"❌ ERROR - {testCase.Name}: {ex.Message}",
                            testCaseExecution?.Id,
                            ex.StackTrace);
                    }
                }

                // Finaliser l'exécution
                await FinalizeTestRunAsync(
                    testRun,
                    testRunRepository,
                    executionLogRepository,
                    logger,
                    totalTests,
                    passedCount,
                    failedCount,
                    skippedCount,
                    totalDurationMs);
            }
            catch (OperationCanceledException)
            {
                await HandleCancelledTestRunAsync(testRun, testRunRepository, executionLogRepository, logger);
            }
            catch (Exception ex)
            {
                await HandleFailedTestRunAsync(testRun, testRunRepository, executionLogRepository, logger, ex);
            }
        }

        /// <summary>
        /// Sauvegarde les exécutions de steps de manière optimisée
        /// </summary>
        private async Task SaveStepExecutionsAsync(
            IEnumerable<dynamic> stepResults,
            int testCaseExecutionId,
            int testRunId,
            ITestStepExecutionRepository testStepExecutionRepository,
            IScreenshotRepository screenshotRepository)
        {
            foreach (var stepResult in stepResults)
            {
                var stepExecution = new TestStepExecution
                {
                    TestCaseExecutionId = testCaseExecutionId,
                    TestStepId = stepResult.StepId,
                    StepOrder = stepResult.Order,
                    Action = stepResult.Action ?? string.Empty,
                    Description = stepResult.Message ?? string.Empty,
                    Status = stepResult.Success ? "Passed" : "Failed",
                    StartedAt = DateTime.UtcNow.AddMilliseconds(-stepResult.Duration.TotalMilliseconds),
                    CompletedAt = DateTime.UtcNow,
                    DurationMs = (int)stepResult.Duration.TotalMilliseconds,
                    ErrorMessage = stepResult.ErrorMessage ?? string.Empty,
                    ScreenshotPath = stepResult.Screenshot ?? string.Empty,
                    Selector = string.Empty,
                    Value = string.Empty
                };

                var savedStepExecution = await testStepExecutionRepository.CreateAsync(stepExecution);

                // Sauvegarder les screenshots si présents
                if (!string.IsNullOrEmpty(stepResult.Screenshot))
                {
                    var screenshot = new Screenshot
                    {
                        TestRunId = testRunId,
                        TestCaseExecutionId = testCaseExecutionId,
                        TestStepExecutionId = savedStepExecution.Id,
                        FilePath = stepResult.Screenshot,
                        Type = stepResult.Success ? "Success" : "Failure",
                        CapturedAt = DateTime.UtcNow,
                        Description = $"Step {stepResult.Order}: {stepResult.Action}"
                    };

                    await screenshotRepository.CreateAsync(screenshot);
                }
            }
        }

        /// <summary>
        /// Finalise l'exécution du test run avec toutes les métriques
        /// </summary>
        private async Task FinalizeTestRunAsync(
            TestRun testRun,
            ITestRunRepository testRunRepository,
            IExecutionLogRepository executionLogRepository,
            ILogger<TestExecutionService3> logger,
            int totalTests,
            int passedCount,
            int failedCount,
            int skippedCount,
            long totalDurationMs)
        {
            await LogExecutionAsync(executionLogRepository, logger, testRun.Id, "Info",
                "Test run completed");
            await LogExecutionAsync(executionLogRepository, logger, testRun.Id, "Info",
                $"Results: {passedCount} passed, {failedCount} failed, {skippedCount} skipped");

            var duration = testRun.StartedAt.HasValue
                ? DateTime.UtcNow - testRun.StartedAt.Value
                : TimeSpan.Zero;

            await LogExecutionAsync(executionLogRepository, logger, testRun.Id, "Info",
                $"Total duration: {duration.TotalMinutes:F2} minutes");

            // Calculer les métriques
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
                environment = testRun.Environment?.Name ?? "Unknown",
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

            await testRunRepository.UpdateAsync(testRun);

            logger.LogInformation($"Test run {testRun.Id} ({testRun.Name}) completed. Status: {finalStatus}");
        }

        /// <summary>
        /// Gère l'annulation d'un test run
        /// </summary>
        private async Task HandleCancelledTestRunAsync(
            TestRun testRun,
            ITestRunRepository testRunRepository,
            IExecutionLogRepository executionLogRepository,
            ILogger<TestExecutionService3> logger)
        {
            logger.LogWarning($"Test run {testRun.Id} ({testRun.Name}) was cancelled");

            await LogExecutionAsync(executionLogRepository, logger, testRun.Id, "Warning",
                "Test run was cancelled by user");

            testRun.Status = "Cancelled";
            testRun.CompletedAt = DateTime.UtcNow;

            await testRunRepository.UpdateAsync(testRun);
        }

        /// <summary>
        /// Gère les erreurs fatales d'un test run
        /// </summary>
        private async Task HandleFailedTestRunAsync(
            TestRun testRun,
            ITestRunRepository testRunRepository,
            IExecutionLogRepository executionLogRepository,
            ILogger<TestExecutionService3> logger,
            Exception ex)
        {
            logger.LogError(ex, $"Fatal error during test run {testRun.Id} ({testRun.Name})");

            await LogExecutionAsync(executionLogRepository, logger, testRun.Id, "Error",
                $"FATAL ERROR: {ex.Message}",
                null,
                ex.StackTrace);

            testRun.Status = "Failed";
            testRun.CompletedAt = DateTime.UtcNow;
            testRun.ProgressPercentage = 100;

            await testRunRepository.UpdateAsync(testRun);
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Charge les relations nécessaires pour le test run
        /// </summary>
        private async Task LoadTestRunRelationsAsync(
            TestRun testRun,
            IEnvironmentRepository environmentRepository,
            ITestDataRepository testDataRepository,
            int applicationId)
        {
            if (testRun.EnvironmentId > 0)
            {
                testRun.Environment = await environmentRepository.GetByIdAsync(testRun.EnvironmentId);
                if (testRun.Environment == null)
                {
                    throw new InvalidOperationException(
                        $"Environment with ID {testRun.EnvironmentId} not found");
                }
            }
            else
            {
                throw new InvalidOperationException("Test run must have an environment configured");
            }

            testRun.TestData = await SelectBestTestDataForEnvironmentAsync(
                testRun,
                testDataRepository,
                applicationId);
        }

        /// <summary>
        /// Sélectionne automatiquement les meilleures données de test
        /// </summary>
        private async Task<TestData> SelectBestTestDataForEnvironmentAsync(
            TestRun testRun,
            ITestDataRepository testDataRepository,
            int applicationId)
        {
            // Si des données spécifiques sont sélectionnées
            if (testRun.TestDataId.HasValue && testRun.TestDataId.Value > 0)
            {
                var specificData = await testDataRepository.GetByIdAsync(testRun.TestDataId.Value);
                if (specificData != null)
                {
                    _logger.LogInformation($"Using user-selected test data: {specificData.Name}");
                    return specificData;
                }
            }

            // Chercher des données spécifiques à l'environnement
            var environmentTestData = await testDataRepository.GetByEnvironmentAsync(testRun.EnvironmentId);
            var bestMatch = environmentTestData.FirstOrDefault();

            if (bestMatch != null)
            {
                _logger.LogInformation($"Auto-selected test data for environment: {bestMatch.Name}");
                return bestMatch;
            }

            // Fallback sur des données génériques
            var genericData = await testDataRepository.GetByApplicationIdAsync(applicationId);
            var fallback = genericData.FirstOrDefault(td => td.SpecificEnvironmentId == null);

            if (fallback != null)
            {
                _logger.LogWarning(
                    $"Using generic test data (no environment-specific data found): {fallback.Name}");
                return fallback;
            }

            _logger.LogWarning("No test data found - tests will run without data variables");
            return null;
        }

        /// <summary>
        /// Récupère les test cases à exécuter selon le type d'exécution
        /// </summary>
        private async Task<IEnumerable<TestCase>> GetTestCasesToExecuteAsync(
            TestRun testRun,
            ITestCaseRepository testCaseRepository,
            ILogger<TestExecutionService3> logger)
        {
            var testCases = new List<TestCase>();

            try
            {
                var targetIds = JsonSerializer.Deserialize<int[]>(testRun.TargetIds) ?? Array.Empty<int>();

                if (!targetIds.Any())
                {
                    throw new InvalidOperationException("No target IDs specified for test run");
                }

                logger.LogInformation(
                    $"Loading test cases for execution type: {testRun.ExecutionType}");

                switch (testRun.ExecutionType.ToLower())
                {
                    case "testcase":
                    case "multiple":
                        foreach (var id in targetIds)
                        {
                            var testCase = await testCaseRepository.GetByIdWithStepsAsync(id);
                            if (testCase != null && testCase.Active)
                            {
                                testCases.Add(testCase);
                            }
                            else
                            {
                                logger.LogWarning($"TestCase {id} not found or inactive");
                            }
                        }
                        break;

                    case "feature":
                        foreach (var featureId in targetIds)
                        {
                            var featureTests = await testCaseRepository.GetByFeatureIdAsync(featureId);
                            var activeTests = featureTests.Where(tc => tc.Active).ToList();

                            foreach (var testCase in activeTests)
                            {
                                var testCaseWithSteps = await testCaseRepository.GetByIdWithStepsAsync(testCase.Id);
                                if (testCaseWithSteps != null)
                                {
                                    testCases.Add(testCaseWithSteps);
                                }
                            }
                        }
                        break;

                    default:
                        throw new NotSupportedException(
                            $"Execution type '{testRun.ExecutionType}' is not supported");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading test cases to execute");
                throw;
            }

            // Retourner une liste distincte
            return testCases.GroupBy(tc => tc.Id).Select(g => g.First()).ToList();
        }

        /// <summary>
        /// Met à jour la progression du test run
        /// </summary>
        private async Task UpdateTestRunProgressAsync(
            ITestRunRepository testRunRepository,
            ILogger<TestExecutionService3> logger,
            int testRunId,
            int progress)
        {
            try
            {
                var testRun = await testRunRepository.GetByIdAsync(testRunId);
                if (testRun != null)
                {
                    testRun.ProgressPercentage = Math.Min(100, Math.Max(0, progress));
                    await testRunRepository.UpdateAsync(testRun);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, $"Failed to update progress for test run {testRunId}");
            }
        }

        /// <summary>
        /// Enregistre un log d'exécution
        /// </summary>
        private async Task LogExecutionAsync(
            IExecutionLogRepository executionLogRepository,
            ILogger<TestExecutionService3> logger,
            int testRunId,
            string level,
            string message,
            int? testCaseExecutionId = null,
            string details = null)
        {
            try
            {
                var log = new ExecutionLog
                {
                    TestRunId = testRunId,
                    TestCaseExecutionId = testCaseExecutionId,
                    Timestamp = DateTime.UtcNow,
                    Level = level,
                    Message = message ?? string.Empty,
                    Details = details ?? string.Empty
                };

                await executionLogRepository.CreateAsync(log);

                // Logger aussi dans les logs applicatifs
                switch (level.ToLower())
                {
                    case "error":
                        logger.LogError($"[TestRun {testRunId}] {message}");
                        break;
                    case "warning":
                        logger.LogWarning($"[TestRun {testRunId}] {message}");
                        break;
                    default:
                        logger.LogInformation($"[TestRun {testRunId}] {message}");
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, $"Failed to persist execution log for test run {testRunId}");
            }
        }

        #endregion
    }
}