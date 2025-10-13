//using Microsoft.Extensions.DependencyInjection;
//using OpenQA.Selenium;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TesterLab.Domain.interfaces.Repositories;
//using TesterLab.Domain.Models;
//using TesterLab.Engine.Models;

//namespace TesterLab.Engine.Services.Core
//{

//    public class TestExecutor : ITestExecutor
//    {
//        private readonly IBrowserManager _browserManager;
//        private readonly IServiceProvider _serviceProvider;
//        private readonly IScreenshotManager _screenshotManager;
//        private readonly ITestLogger _logger;
//        private CancellationTokenSource? _cancellationTokenSource;
//        private readonly ITestCaseRepository _testCaseRepository;

//        public TestExecutor(
//            IBrowserManager browserManager,
//            IServiceProvider serviceProvider,
//            IScreenshotManager screenshotManager,
//            ITestLogger logger,
//            ITestCaseRepository testCaseRepository)
//        {
//            _browserManager = browserManager ?? throw new ArgumentNullException(nameof(browserManager));
//            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
//            _screenshotManager = screenshotManager ?? throw new ArgumentNullException(nameof(screenshotManager));
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//            _testCaseRepository = testCaseRepository ?? throw new ArgumentNullException(nameof(testCaseRepository));
//        }

//        public async Task<TestExecutionResult> ExecuteTestCaseAsync(TestCase testCase, Domain.Models.Environment environment, DataSet? dataSet = null)
//        {
//            var result = new TestExecutionResult
//            {
//                TestCaseId = testCase.Id,
//                TestCaseName = testCase.Name,
//                StartTime = DateTime.UtcNow,
//                Status = TestStatus.Running
//            };

//            _cancellationTokenSource = new CancellationTokenSource();

//            try
//            {
//                _logger.LogInfo($"Starting test execution: {testCase.Name}");

//                // Initialiser le navigateur si nécessaire
//                if (!_browserManager.IsInitialized)
//                {
//                    var config = CreateBrowserConfiguration();
//                    await _browserManager.InitializeAsync(config);
//                }

//                // Naviguer vers l'URL de base
//                await _browserManager.NavigateToAsync(environment.BaseUrl);

//                // Préparer les données de test
//                var testData = PrepareTestData(dataSet);

//                // Exécuter les étapes
//                var orderedSteps = testCase.TestSteps?.OrderBy(s => s.Order).ToList() ?? new List<TestStep>();

//                foreach (var step in orderedSteps)
//                {
//                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();

//                    var stepResult = await ExecuteStepAsync(step, testData);
//                    result.StepResults.Add(stepResult);

//                    // Si l'étape échoue et n'est pas configurée pour être ignorée
//                    if (stepResult.Status == ActionStatus.Failed /*&& !step.SkipOnFailure*/)
//                    {
//                        result.Status = TestStatus.Failed;
//                        result.ErrorMessage = stepResult.ErrorMessage;
//                        break;
//                    }
//                }

//                // Déterminer le statut final
//                if (result.Status == TestStatus.Running)
//                {
//                    result.Status = result.StepResults.Any(s => s.Status == ActionStatus.Failed)
//                        ? TestStatus.Failed
//                        : TestStatus.Passed;
//                }

//                result.EndTime = DateTime.UtcNow;
//                _logger.LogInfo($"Test execution completed: {testCase.Name} - Status: {result.Status}");

//                return result;
//            }
//            catch (OperationCanceledException)
//            {
//                result.Status = TestStatus.Cancelled;
//                result.EndTime = DateTime.UtcNow;
//                _logger.LogInfo($"Test execution cancelled: {testCase.Name}");
//                return result;
//            }
//            catch (Exception ex)
//            {
//                result.Status = TestStatus.Failed;
//                result.EndTime = DateTime.UtcNow;
//                result.ErrorMessage = ex.Message;
//                result.Exception = ex;
//                _logger.LogError($"Test execution failed: {testCase.Name}", ex);
//                return result;
//            }
//        }

//        //public async Task<List<TestExecutionResult>> ExecuteTestSuiteAsync(TestSuite testSuite, Domain.Models.Environment environment, DataSet? dataSet = null)
//        //{
//        //    var results = new List<TestExecutionResult>();
//        //    _logger.LogInfo($"Starting test suite execution: {testSuite.Name}");

//        //    try
//        //    {
//        //        var testCases = testSuite.TestCases?.Where(tc => tc.Active).OrderBy(tc => tc.Priority).ToList() ?? new List<TestCase>();

//        //        foreach (var testCase in testCases)
//        //        {
//        //            var result = await ExecuteTestCaseAsync(testCase, environment, dataSet);
//        //            results.Add(result);

//        //            // Optionnel: arrêter l'exécution de la suite si un test critique échoue
//        //            if (result.Status == TestStatus.Failed && testCase.Priority == 1)
//        //            {
//        //                _logger.LogWarning($"Critical test failed, stopping suite execution: {testCase.Name}");
//        //                break;
//        //            }
//        //        }

//        //        _logger.LogInfo($"Test suite execution completed: {testSuite.Name} - {results.Count(r => r.Status == TestStatus.Passed)}/{results.Count} passed");
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        _logger.LogError($"Test suite execution failed: {testSuite.Name}", ex);
//        //    }

//        //    return results;
//        //}

//        public async Task StopExecutionAsync()
//        {
//            _cancellationTokenSource?.Cancel();
//            await Task.CompletedTask;
//        }

//        private async Task<ActionResult> ExecuteStepAsync(TestStep step, Dictionary<string, string> testData)
//        {
//            var stopwatch = Stopwatch.StartNew();
//            var result = new ActionResult
//            {
//                StepId = step.Id,
//                Action = step.Action,
//                Element = step.Selector ?? "N/A",
//                Value = step.Value,
//                Timestamp = DateTime.UtcNow
//            };

//            try
//            {
//                _logger.LogInfo($"Executing step: {step.Action} on {result.Element}");

//                // Remplacer les variables dans la valeur
//                var processedValue = ProcessVariables(step.Value, testData);
//                result.Value = processedValue;

//                // Obtenir l'exécuteur d'action approprié
//                var actionExecutors = _serviceProvider.GetServices<IActionExecutor>();
//                var executor = actionExecutors.FirstOrDefault(e => e.CanExecute(step.Action));

//                if (executor == null)
//                {
//                    throw new NotSupportedException($"No executor found for action: {step.Action}");
//                }

//                // Exécuter l'action
//                var actionResult = await executor.ExecuteActionAsync(step, _browserManager, testData);

//                result.Status = actionResult.Status;
//                result.ErrorMessage = actionResult.ErrorMessage;
//                result.Exception = actionResult.Exception;

//                stopwatch.Stop();
//                result.Duration = stopwatch.Elapsed;

//                return result;
//            }
//            catch (Exception ex)
//            {
//                stopwatch.Stop();
//                result.Status = ActionStatus.Failed;
//                result.Duration = stopwatch.Elapsed;
//                result.ErrorMessage = ex.Message;
//                result.Exception = ex;

//                _logger.LogError($"Step execution failed: {step.Action} on {result.Element}", ex);
//                return result;
//            }
//        }

//        private Dictionary<string, string> PrepareTestData(DataSet? dataSet)
//        {
//            var testData = new Dictionary<string, string>();

//            if (dataSet?.TestData != null)
//            {
//                foreach (var data in dataSet.TestData)
//                {
//                    testData[data.Key] = data.Value;
//                }
//            }

//            // Ajouter des variables système
//            testData["{{TODAY}}"] = DateTime.Now.ToString("yyyy-MM-dd");
//            testData["{{NOW}}"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
//            testData["{{TIMESTAMP}}"] = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
//            testData["{{GUID}}"] = Guid.NewGuid().ToString();

//            return testData;
//        }

//        private string? ProcessVariables(string? input, Dictionary<string, string> variables)
//        {
//            if (string.IsNullOrEmpty(input)) return input;

//            var result = input;
//            foreach (var variable in variables)
//            {
//                result = result.Replace(variable.Key, variable.Value);
//            }

//            return result;
//        }

//        private BrowserConfiguration CreateBrowserConfiguration()
//        {
//            return new BrowserConfiguration
//            {
//                BrowserType = BrowserType.Chrome,
//                Headless = false,
//                ImplicitWait = TimeSpan.FromSeconds(10),
//                PageLoadTimeout = TimeSpan.FromSeconds(30),
//                Arguments = new List<string>
//                {
//                    "--disable-blink-features=AutomationControlled",
//                    "--disable-extensions",
//                    "--no-sandbox",
//                    "--disable-dev-shm-usage"
//                }
//            };
//        }

//        public async Task<TestExecutionResult> ExecuteTestCaseAsync(TestCaseDto testCaseDto, Domain.Models.Environment environment, DataSet? dataSet = null)
//        {
//            var testCase = await _testCaseRepository.GetByIdWithDetailsAsync(testCaseDto.Id);
//            return await ExecuteTestCaseAsync(testCase, environment);
//        }

//        public Task<List<TestExecutionResult>> ExecuteEnvironmentAsync(Domain.Models.Environment environment, DataSet? dataSet = null)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
