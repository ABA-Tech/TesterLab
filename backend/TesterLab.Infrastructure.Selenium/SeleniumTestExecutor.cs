using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.RegularExpressions;
using TesterLab.Domain.Models;
using Microsoft.Extensions.Logging;
using TesterLab.Domain.interfaces.Services;

namespace TesterLab.Infrastructure.Selenium
{
    /// <summary>
    /// Exécuteur de tests Selenium optimisé pour les exécutions parallèles
    /// Thread-safe avec isolation complète des instances WebDriver
    /// </summary>
    public class SeleniumTestExecutor : ITestExecutor
    {
        private readonly ILogger<SeleniumTestExecutor> _logger;
        private readonly string _screenshotsPath;

        // Compteur thread-safe pour les ports de débogage uniques
        private static int _debugPortCounter = 9222;
        private static readonly object _portLock = new object();

        public SeleniumTestExecutor(ILogger<SeleniumTestExecutor> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _screenshotsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "screenshots");

            if (!Directory.Exists(_screenshotsPath))
            {
                Directory.CreateDirectory(_screenshotsPath);
            }
        }

        public async Task<TestExecutionResult> ExecuteTestCaseAsync(TestCase testCase, TestRun testRun)
        {
            var result = new TestExecutionResult();
            var startTime = DateTime.UtcNow;
            IWebDriver driver = null;

            try
            {
                _logger.LogInformation($"[TestCase {testCase.Id}] Starting execution: {testCase.Name}");

                // Validations préalables
                ValidateTestRunConfiguration(testRun);
                ValidateTestCase(testCase);

                // Initialiser le driver avec isolation complète
                driver = InitializeDriver(testRun.Browser, testRun.Headless);
                _logger.LogInformation($"[TestCase {testCase.Id}] WebDriver initialized: {testRun.Browser} (Headless: {testRun.Headless})");

                // Charger les données de test
                var testData = LoadTestData(testRun.TestData);
                _logger.LogDebug($"[TestCase {testCase.Id}] Test data loaded: {testData.Count} variables");

                // Naviguer vers l'URL de base
                _logger.LogInformation($"[TestCase {testCase.Id}] Navigating to: {testRun.Environment.BaseUrl}");
                driver.Navigate().GoToUrl(testRun.Environment.BaseUrl);
                await Task.Delay(2000); // Attendre le chargement initial

                // Exécuter chaque étape dans l'ordre
                var orderedSteps = testCase.TestSteps.OrderBy(s => s.Order).ToList();
                _logger.LogInformation($"[TestCase {testCase.Id}] Executing {orderedSteps.Count} test steps");

                foreach (var step in orderedSteps)
                {
                    _logger.LogInformation($"[TestCase {testCase.Id}] Step {step.Order}/{orderedSteps.Count}: {step.Action} - {step.Description}");

                    var stepResult = await ExecuteTestStepAsync(step, driver, testData);
                    result.StepResults.Add(stepResult);

                    if (!stepResult.Success)
                    {
                        if (!step.IsOptional)
                        {
                            result.Success = false;
                            result.Message = $"Step {step.Order} failed: {stepResult.ErrorMessage}";
                            result.ErrorDetails = stepResult.ErrorMessage;

                            // Capture d'écran de l'échec
                            await CaptureFailureScreenshotAsync(driver, testCase, step, result);
                            break;
                        }
                        else
                        {
                            _logger.LogWarning($"[TestCase {testCase.Id}] Optional step {step.Order} failed but continuing: {stepResult.ErrorMessage}");
                        }
                    }
                }

                // Déterminer le résultat final
                var mandatorySteps = result.StepResults.Where(sr =>
                {
                    var step = testCase.TestSteps.FirstOrDefault(ts => ts.Id == sr.StepId);
                    return step != null && !step.IsOptional;
                }).ToList();

                if (mandatorySteps.Any() && mandatorySteps.All(sr => sr.Success))
                {
                    result.Success = true;
                    var failedOptional = result.StepResults.Count(sr => !sr.Success);
                    result.Message = failedOptional > 0
                        ? $"Test completed with {failedOptional} optional step(s) failed"
                        : $"Test completed successfully with {result.StepResults.Count} steps";
                }

                result.Duration = DateTime.UtcNow - startTime;

                _logger.LogInformation($"[TestCase {testCase.Id}] Execution completed. Success: {result.Success}, Duration: {result.Duration.TotalSeconds:F2}s");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[TestCase {testCase.Id}] Fatal error executing test case: {testCase.Name}");

                result.Success = false;
                result.Message = "Test execution failed with exception";
                result.ErrorDetails = $"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}";
                result.Duration = DateTime.UtcNow - startTime;

                // Capture d'écran de l'erreur
                if (driver != null)
                {
                    await CaptureErrorScreenshotAsync(driver, testCase, result);
                }
            }
            finally
            {
                // Fermer et disposer le driver de manière sécurisée
                await DisposeDriverSafelyAsync(driver, testCase.Id);
            }

            return result;
        }

        public async Task<StepResult> ExecuteTestStepAsync(TestStep testStep, IWebDriver driver, Dictionary<string, string> testData)
        {
            var stepResult = new StepResult
            {
                StepId = testStep.Id,
                Order = testStep.Order,
                Action = testStep.Action
            };

            var startTime = DateTime.UtcNow;

            try
            {
                // Remplacer les variables dans les valeurs
                var value = ReplaceVariables(testStep.Value, testData);
                var target = ReplaceVariables(testStep.Target, testData);
                var selector = ReplaceVariables(testStep.Selector, testData);

                _logger.LogDebug($"[Step {testStep.Id}] Action={testStep.Action}, Selector={selector}, Value={value}");

                // Créer un WebDriverWait avec timeout approprié
                var timeoutSeconds = Math.Max(testStep.TimeoutSeconds, 10);
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));

                // Exécuter l'action appropriée
                switch (testStep.Action.ToLower())
                {
                    case "navigate":
                        await ExecuteNavigateAsync(driver, value, testStep);
                        break;

                    case "click":
                        await ExecuteClickAsync(driver, wait, testStep);
                        break;

                    case "type":
                        await ExecuteTypeAsync(driver, wait, testStep, value);
                        break;

                    case "assert":
                        await ExecuteAssertAsync(driver, wait, testStep, value);
                        break;

                    case "assert_enabled":
                        await ExecuteEnabledCheckAsync(driver, wait, testStep, true);
                        break;

                    case "assert_disabled":
                        await ExecuteDisabledCheckAsync(driver, wait, testStep, true);
                        break;

                    case "verify_enabled":
                        await ExecuteEnabledCheckAsync(driver, wait, testStep, false);
                        break;

                    case "verify_disabled":
                        await ExecuteDisabledCheckAsync(driver, wait, testStep, false);
                        break;

                    case "wait":
                        await ExecuteWaitAsync(driver, testStep, value);
                        break;

                    case "select":
                        await ExecuteSelectAsync(driver, wait, testStep, value);
                        break;

                    case "check":
                    case "checkbox":
                        await ExecuteCheckAsync(driver, wait, testStep);
                        break;

                    case "scroll":
                        await ExecuteScrollAsync(driver, testStep, value);
                        break;

                    case "hover":
                        await ExecuteHoverAsync(driver, wait, testStep);
                        break;

                    case "clear":
                        await ExecuteClearAsync(driver, wait, testStep);
                        break;

                    case "switchframe":
                    case "switch_frame":
                        await ExecuteSwitchFrameAsync(driver, testStep);
                        break;

                    case "acceptalert":
                    case "accept_alert":
                        driver.SwitchTo().Alert().Accept();
                        await Task.Delay(300);
                        break;

                    case "dismissalert":
                    case "dismiss_alert":
                        driver.SwitchTo().Alert().Dismiss();
                        await Task.Delay(300);
                        break;

                    default:
                        throw new NotSupportedException($"Action '{testStep.Action}' is not supported");
                }

                stepResult.Success = true;
                stepResult.Message = $"Step executed successfully: {testStep.Description ?? testStep.Action}";

                _logger.LogInformation($"[Step {testStep.Id}] Completed successfully");
            }
            catch (NoSuchElementException ex)
            {
                stepResult.Success = false;
                stepResult.ErrorMessage = $"Element not found: {testStep.Selector}";
                _logger.LogError(ex, $"[Step {testStep.Id}] {stepResult.ErrorMessage}");
            }
            catch (WebDriverTimeoutException ex)
            {
                stepResult.Success = false;
                stepResult.ErrorMessage = $"Timeout waiting for element: {testStep.Selector} (timeout: {testStep.TimeoutSeconds}s)";
                _logger.LogError(ex, $"[Step {testStep.Id}] {stepResult.ErrorMessage}");
            }
            catch (ElementNotInteractableException ex)
            {
                stepResult.Success = false;
                stepResult.ErrorMessage = $"Element not interactable: {testStep.Selector}";
                _logger.LogError(ex, $"[Step {testStep.Id}] {stepResult.ErrorMessage}");
            }
            catch (StaleElementReferenceException ex)
            {
                stepResult.Success = false;
                stepResult.ErrorMessage = $"Element became stale: {testStep.Selector}";
                _logger.LogError(ex, $"[Step {testStep.Id}] {stepResult.ErrorMessage}");
            }
            catch (Exception ex)
            {
                stepResult.Success = false;
                stepResult.ErrorMessage = $"{ex.GetType().Name}: {ex.Message}";
                _logger.LogError(ex, $"[Step {testStep.Id}] Error executing step");
            }
            finally
            {
                stepResult.Duration = DateTime.UtcNow - startTime;

                // Capture d'écran pour les échecs et assertions
                if (!stepResult.Success ||
                    testStep.Action.ToLower().StartsWith("assert") ||
                    testStep.Action.ToLower().StartsWith("verify"))
                {
                    await CaptureStepScreenshotAsync(driver, testStep, stepResult);
                }
            }

            return stepResult;
        }

        #region WebDriver Initialization (Thread-Safe)

        /// <summary>
        /// Initialise un WebDriver avec isolation complète pour les exécutions parallèles
        /// Chaque instance a son propre profil utilisateur et port de débogage
        /// </summary>
        private IWebDriver InitializeDriver(string browser, bool headless)
        {
            IWebDriver driver = null;

            try
            {
                switch (browser?.ToLower() ?? "chrome")
                {
                    case "chrome":
                        driver = InitializeChromeDriver(headless);
                        break;

                    case "firefox":
                        driver = InitializeFirefoxDriver(headless);
                        break;

                    case "edge":
                        driver = InitializeEdgeDriver(headless);
                        break;

                    default:
                        throw new NotSupportedException($"Browser '{browser}' is not supported");
                }

                // Configuration commune
                ConfigureDriverTimeouts(driver);
                ConfigureDriverWindow(driver, headless);

                _logger.LogInformation($"WebDriver initialized successfully: {browser} (Headless: {headless})");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to initialize WebDriver for browser: {browser}");
                throw new InvalidOperationException(
                    $"Failed to initialize {browser} driver. Make sure the browser is installed on your system.",
                    ex
                );
            }

            return driver;
        }

        /// <summary>
        /// Initialise Chrome avec isolation complète (profil unique + port unique)
        /// </summary>
        private IWebDriver InitializeChromeDriver(bool headless)
        {
            var chromeOptions = new ChromeOptions();

            // 1. ISOLATION TOTALE : Profil utilisateur unique par instance
            var uniqueProfilePath = Path.Combine(
                Path.GetTempPath(),
                "selenium-chrome-profiles",
                $"profile-{Guid.NewGuid()}"
            );
            Directory.CreateDirectory(uniqueProfilePath);
            chromeOptions.AddArgument($"--user-data-dir={uniqueProfilePath}");

            // 2. ISOLATION TOTALE : Port de débogage unique
            int debugPort = GetNextAvailableDebugPort();
            //chromeOptions.AddArgument($"--remote-debugging-port={debugPort}");

            // 3. Localisation du binaire Chrome selon l'OS
            ConfigureChromeBinaryLocation(chromeOptions);

            // 4. Mode headless si nécessaire
            if (headless || !OperatingSystem.IsWindows())
            {
                chromeOptions.AddArgument("--headless=new");
                chromeOptions.AddArgument("--disable-gpu");
            }

            // 5. Arguments essentiels pour la stabilité et les conteneurs
            chromeOptions.AddArgument("--no-sandbox");
            chromeOptions.AddArgument("--disable-dev-shm-usage");
            chromeOptions.AddArgument("--disable-blink-features=AutomationControlled");
            chromeOptions.AddArgument("--window-size=1920,1080");
            chromeOptions.AddArgument("--disable-extensions");
            chromeOptions.AddArgument("--disable-software-rasterizer");
            chromeOptions.AddArgument("--disable-setuid-sandbox");
            chromeOptions.AddArgument("--disable-background-networking");
            chromeOptions.AddArgument("--disable-background-timer-throttling");
            chromeOptions.AddArgument("--disable-backgrounding-occluded-windows");
            chromeOptions.AddArgument("--disable-breakpad");
            chromeOptions.AddArgument("--disable-component-extensions-with-background-pages");
            chromeOptions.AddArgument("--disable-features=TranslateUI");
            chromeOptions.AddArgument("--disable-ipc-flooding-protection");
            chromeOptions.AddArgument("--disable-renderer-backgrounding");
            chromeOptions.AddArgument("--enable-features=NetworkService,NetworkServiceInProcess");
            chromeOptions.AddArgument("--hide-scrollbars");
            chromeOptions.AddArgument("--metrics-recording-only");
            chromeOptions.AddArgument("--mute-audio");

            // 6. Préférences utilisateur
            chromeOptions.AddUserProfilePreference("credentials_enable_service", false);
            chromeOptions.AddUserProfilePreference("profile.password_manager_enabled", false);
            chromeOptions.AddUserProfilePreference("download.prompt_for_download", false);
            chromeOptions.AddUserProfilePreference("download.default_directory", Path.GetTempPath());

            // 7. Créer le service ChromeDriver
            var service = CreateChromeDriverService();

            _logger.LogDebug($"Chrome profile: {uniqueProfilePath}, Debug port: {debugPort}");

            return new ChromeDriver(service, chromeOptions, TimeSpan.FromSeconds(90));
        }

        /// <summary>
        /// Configure la localisation du binaire Chrome selon l'OS
        /// </summary>
        private void ConfigureChromeBinaryLocation(ChromeOptions chromeOptions)
        {
            string chromeBinary = null;

            if (OperatingSystem.IsMacOS())
            {
                chromeBinary = "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome";
            }
            else if (OperatingSystem.IsLinux())
            {
                chromeBinary = System.Environment.GetEnvironmentVariable("CHROME_BIN");

                // Chemins par défaut Linux
                if (string.IsNullOrEmpty(chromeBinary))
                {
                    var possiblePaths = new[]
                    {
                        "/usr/bin/google-chrome",
                        "/usr/bin/chromium-browser",
                        "/usr/bin/chromium",
                        "/snap/bin/chromium"
                    };

                    chromeBinary = possiblePaths.FirstOrDefault(File.Exists);
                }
            }

            if (!string.IsNullOrEmpty(chromeBinary) && File.Exists(chromeBinary))
            {
                chromeOptions.BinaryLocation = chromeBinary;
                _logger.LogDebug($"Chrome binary location: {chromeBinary}");
            }
        }

        /// <summary>
        /// Crée le service ChromeDriver avec configuration optimale
        /// </summary>
        private ChromeDriverService CreateChromeDriverService()
        {
            ChromeDriverService service;

            // Vérifier si un chemin custom est défini
            string chromeDriverPath = System.Environment.GetEnvironmentVariable("CHROMEDRIVER_PATH");

            if (!string.IsNullOrEmpty(chromeDriverPath) && File.Exists(chromeDriverPath))
            {
                var directory = Path.GetDirectoryName(chromeDriverPath);
                service = ChromeDriverService.CreateDefaultService(directory);
            }
            else
            {
                service = ChromeDriverService.CreateDefaultService();
            }

            service.HideCommandPromptWindow = true;
            service.SuppressInitialDiagnosticInformation = true;
            service.EnableVerboseLogging = false;

            return service;
        }

        /// <summary>
        /// Obtient le prochain port de débogage disponible (thread-safe)
        /// </summary>
        private int GetNextAvailableDebugPort()
        {
            lock (_portLock)
            {
                int port = _debugPortCounter++;

                // Si on dépasse 65535, on recommence à 9222
                if (_debugPortCounter > 65535)
                {
                    _debugPortCounter = 9222;
                }

                return port;
            }
        }

        /// <summary>
        /// Initialise Firefox avec isolation
        /// </summary>
        private IWebDriver InitializeFirefoxDriver(bool headless)
        {
            var firefoxOptions = new FirefoxOptions();

            if (headless)
            {
                firefoxOptions.AddArgument("--headless");
            }

            firefoxOptions.SetPreference("dom.webdriver.enabled", false);
            firefoxOptions.SetPreference("useAutomationExtension", false);

            return new FirefoxDriver(firefoxOptions);
        }

        /// <summary>
        /// Initialise Edge avec isolation
        /// </summary>
        private IWebDriver InitializeEdgeDriver(bool headless)
        {
            var edgeOptions = new EdgeOptions();

            if (headless)
            {
                edgeOptions.AddArgument("--headless=new");
            }

            edgeOptions.AddArgument("--no-sandbox");
            edgeOptions.AddArgument("--disable-dev-shm-usage");

            return new EdgeDriver(edgeOptions);
        }

        /// <summary>
        /// Configure les timeouts du driver
        /// </summary>
        private void ConfigureDriverTimeouts(IWebDriver driver)
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(90);
            driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// Configure la fenêtre du driver
        /// </summary>
        private void ConfigureDriverWindow(IWebDriver driver, bool headless)
        {
            if (!headless && OperatingSystem.IsWindows())
            {
                try
                {
                    driver.Manage().Window.Maximize();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to maximize window");
                }
            }
        }

        /// <summary>
        /// Dispose le driver de manière sécurisée
        /// </summary>
        private async Task DisposeDriverSafelyAsync(IWebDriver driver, int testCaseId)
        {
            if (driver == null)
                return;

            try
            {
                _logger.LogDebug($"[TestCase {testCaseId}] Closing WebDriver...");

                driver.Quit();
                driver.Dispose();

                _logger.LogInformation($"[TestCase {testCaseId}] WebDriver closed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"[TestCase {testCaseId}] Error closing WebDriver");
            }

            await Task.CompletedTask;
        }

        #endregion

        #region Selenium Actions

        private async Task ExecuteNavigateAsync(IWebDriver driver, string url, TestStep testStep)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("URL cannot be empty for navigate action");
            }

            if (url.StartsWith("/"))
            {
                // URL relative
                var currentUri = new Uri(driver.Url);
                var baseUrl = $"{currentUri.Scheme}://{currentUri.Host}";
                if (currentUri.Port != 80 && currentUri.Port != 443)
                {
                    baseUrl += $":{currentUri.Port}";
                }
                var fullUrl = baseUrl + url;
                _logger.LogDebug($"Navigating to relative URL: {fullUrl}");
                driver.Navigate().GoToUrl(fullUrl);
            }
            else
            {
                _logger.LogDebug($"Navigating to absolute URL: {url}");
                driver.Navigate().GoToUrl(url);
            }

            await Task.Delay(1500);
        }

        private async Task ExecuteClickAsync(IWebDriver driver, WebDriverWait wait, TestStep testStep)
        {
            var element = FindElement(driver, wait, testStep);

            // Scroll vers l'élément
            ((IJavaScriptExecutor)driver).ExecuteScript(
                "arguments[0].scrollIntoView({behavior: 'smooth', block: 'center'});",
                element);
            await Task.Delay(500);

            // Attendre que l'élément soit cliquable
            wait.Until(d =>
            {
                try
                {
                    return element.Displayed && element.Enabled;
                }
                catch
                {
                    return false;
                }
            });

            // Essayer le clic normal, sinon JavaScript
            try
            {
                element.Click();
            }
            catch (ElementClickInterceptedException)
            {
                _logger.LogWarning("Normal click intercepted, using JavaScript click");
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", element);
            }

            await Task.Delay(500);
        }

        private async Task ExecuteTypeAsync(IWebDriver driver, WebDriverWait wait, TestStep testStep, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Value cannot be empty for type action");
            }

            var element = FindElement(driver, wait, testStep);

            // Scroll vers l'élément
            ((IJavaScriptExecutor)driver).ExecuteScript(
                "arguments[0].scrollIntoView({behavior: 'smooth', block: 'center'});",
                element);
            await Task.Delay(500);

            // Attendre que l'élément soit visible
            wait.Until(d =>
            {
                try
                {
                    return element.Displayed && element.Enabled;
                }
                catch
                {
                    return false;
                }
            });

            // Effacer d'abord
            element.Clear();
            await Task.Delay(200);

            // Saisir le texte
            foreach (char c in value)
            {
                element.SendKeys(c.ToString());
                await Task.Delay(50);
            }

            await Task.Delay(300);
        }

        private async Task ExecuteAssertAsync(IWebDriver driver, WebDriverWait wait, TestStep testStep, string expectedValue)
        {
            if (string.IsNullOrEmpty(expectedValue))
            {
                throw new ArgumentException("Expected value cannot be empty for assert action");
            }

            var element = FindElement(driver, wait, testStep);
            var actualValue = element.Text;

            if (!actualValue.Contains(expectedValue, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception($"Assertion failed. Expected text containing: '{expectedValue}', but got: '{actualValue}'");
            }

            _logger.LogInformation($"Assertion passed: found '{expectedValue}' in '{actualValue}'");
            await Task.CompletedTask;
        }

        private async Task ExecuteEnabledCheckAsync(IWebDriver driver, WebDriverWait wait, TestStep testStep, bool throwOnFailure)
        {
            var element = FindElement(driver, wait, testStep);

            if (!element.Enabled)
            {
                var message = "Element is not enabled";

                if (throwOnFailure)
                {
                    throw new Exception($"Assertion failed. {message}");
                }

                _logger.LogWarning($"Verify failed: {message}");
            }
            else
            {
                _logger.LogInformation("Element is enabled");
            }

            await Task.CompletedTask;
        }

        private async Task ExecuteDisabledCheckAsync(IWebDriver driver, WebDriverWait wait, TestStep testStep, bool throwOnFailure)
        {
            var element = FindElement(driver, wait, testStep);

            if (element.Enabled)
            {
                var message = "Element is enabled but should be disabled";

                if (throwOnFailure)
                {
                    throw new Exception($"Assertion failed. {message}");
                }

                _logger.LogWarning($"Verify failed: {message}");
            }
            else
            {
                _logger.LogInformation("Element is disabled");
            }

            await Task.CompletedTask;
        }

        private async Task ExecuteWaitAsync(IWebDriver driver, TestStep testStep, string value)
        {
            if (int.TryParse(value, out int seconds))
            {
                _logger.LogDebug($"Waiting for {seconds} seconds");
                await Task.Delay(seconds * 1000);
            }
            else if (!string.IsNullOrEmpty(testStep.Selector))
            {
                _logger.LogDebug($"Waiting for element: {testStep.Selector}");
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(testStep.TimeoutSeconds));
                wait.Until(d => d.FindElement(GetBy(testStep.Selector)));
            }
            else
            {
                throw new ArgumentException("Wait action requires either a number of seconds or a selector");
            }
        }

        private async Task ExecuteSelectAsync(IWebDriver driver, WebDriverWait wait, TestStep testStep, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Value cannot be empty for select action");
            }

            var element = FindElement(driver, wait, testStep);
            var select = new SelectElement(element);

            try
            {
                select.SelectByText(value);
                _logger.LogDebug($"Selected by text: {value}");
            }
            catch
            {
                try
                {
                    select.SelectByValue(value);
                    _logger.LogDebug($"Selected by value: {value}");
                }
                catch
                {
                    if (int.TryParse(value, out int index))
                    {
                        select.SelectByIndex(index);
                        _logger.LogDebug($"Selected by index: {index}");
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            await Task.Delay(300);
        }

        private async Task ExecuteCheckAsync(IWebDriver driver, WebDriverWait wait, TestStep testStep)
        {
            var element = FindElement(driver, wait, testStep);

            if (!element.Selected)
            {
                element.Click();
                await Task.Delay(300);
                _logger.LogDebug("Checkbox checked");
            }
            else
            {
                _logger.LogDebug("Checkbox already checked");
            }
        }

        private async Task ExecuteScrollAsync(IWebDriver driver, TestStep testStep, string value)
        {
            var jsExecutor = (IJavaScriptExecutor)driver;

            if (string.IsNullOrEmpty(value) || value.ToLower() == "top")
            {
                jsExecutor.ExecuteScript("window.scrollTo({top: 0, behavior: 'smooth'});");
                _logger.LogDebug("Scrolled to top");
            }
            else if (value.ToLower() == "bottom")
            {
                jsExecutor.ExecuteScript("window.scrollTo({top: document.body.scrollHeight, behavior: 'smooth'});");
                _logger.LogDebug("Scrolled to bottom");
            }
            else if (int.TryParse(value, out int pixels))
            {
                jsExecutor.ExecuteScript($"window.scrollBy({{top: {pixels}, behavior: 'smooth'}});");
                _logger.LogDebug($"Scrolled by {pixels} pixels");
            }
            else if (!string.IsNullOrEmpty(testStep.Selector))
            {
                var element = driver.FindElement(GetBy(testStep.Selector));
                jsExecutor.ExecuteScript("arguments[0].scrollIntoView({behavior: 'smooth', block: 'center'});", element);
                _logger.LogDebug($"Scrolled to element: {testStep.Selector}");
            }

            await Task.Delay(500);
        }

        private async Task ExecuteHoverAsync(IWebDriver driver, WebDriverWait wait, TestStep testStep)
        {
            var element = FindElement(driver, wait, testStep);

            var actions = new OpenQA.Selenium.Interactions.Actions(driver);
            actions.MoveToElement(element).Perform();

            _logger.LogDebug($"Hovered over element: {testStep.Selector}");
            await Task.Delay(500);
        }

        private async Task ExecuteClearAsync(IWebDriver driver, WebDriverWait wait, TestStep testStep)
        {
            var element = FindElement(driver, wait, testStep);
            element.Clear();
            _logger.LogDebug($"Cleared element: {testStep.Selector}");
            await Task.Delay(300);
        }

        private async Task ExecuteSwitchFrameAsync(IWebDriver driver, TestStep testStep)
        {
            if (string.IsNullOrEmpty(testStep.Value))
            {
                driver.SwitchTo().DefaultContent();
                _logger.LogDebug("Switched to default content");
            }
            else if (int.TryParse(testStep.Value, out int frameIndex))
            {
                driver.SwitchTo().Frame(frameIndex);
                _logger.LogDebug($"Switched to frame by index: {frameIndex}");
            }
            else if (!string.IsNullOrEmpty(testStep.Selector))
            {
                var frameElement = driver.FindElement(GetBy(testStep.Selector));
                driver.SwitchTo().Frame(frameElement);
                _logger.LogDebug($"Switched to frame by selector: {testStep.Selector}");
            }
            else
            {
                driver.SwitchTo().Frame(testStep.Value);
                _logger.LogDebug($"Switched to frame by name: {testStep.Value}");
            }

            await Task.Delay(500);
        }

        #endregion

        #region Helper Methods

        private IWebElement FindElement(IWebDriver driver, WebDriverWait wait, TestStep testStep)
        {
            if (string.IsNullOrEmpty(testStep.Selector))
            {
                throw new ArgumentException($"Selector is required for action '{testStep.Action}'");
            }

            var by = GetBy(testStep.Selector);

            try
            {
                wait.Until(d => d.FindElement(by));
            }
            catch (WebDriverTimeoutException)
            {
                throw new NoSuchElementException(
                    $"Element not found within {testStep.TimeoutSeconds} seconds: {testStep.Selector}");
            }

            return driver.FindElement(by);
        }

        private By GetBy(string selector)
        {
            if (string.IsNullOrEmpty(selector))
                throw new ArgumentException("Selector cannot be null or empty");

            if (selector.StartsWith("//") || selector.StartsWith("(//"))
            {
                return By.XPath(selector);
            }
            else if (selector.StartsWith("#"))
            {
                return By.Id(selector.Substring(1));
            }
            else if (selector.StartsWith("."))
            {
                return By.ClassName(selector.Substring(1));
            }
            else if (selector.StartsWith("[name=") && selector.EndsWith("]"))
            {
                var name = selector.Substring(6, selector.Length - 7);
                return By.Name(name);
            }
            else if (selector.StartsWith("[") && selector.Contains("=") && selector.EndsWith("]"))
            {
                return By.CssSelector(selector);
            }
            else
            {
                return By.CssSelector(selector);
            }
        }

        private Dictionary<string, string> LoadTestData(TestData testData)
        {
            var data = new Dictionary<string, string>();

            if (testData != null && !string.IsNullOrEmpty(testData.DataJson))
            {
                try
                {
                    var jsonData = JsonSerializer.Deserialize<Dictionary<string, string>>(testData.DataJson);
                    if (jsonData != null)
                    {
                        foreach (var kvp in jsonData)
                        {
                            data[kvp.Key] = kvp.Value;
                        }
                    }
                    _logger.LogInformation($"Loaded {data.Count} test data variables");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to parse test data JSON");
                }
            }

            return data;
        }

        private string ReplaceVariables(string text, Dictionary<string, string> testData)
        {
            if (string.IsNullOrEmpty(text) || testData == null || !testData.Any())
                return text;

            var regex = new Regex(@"\$\{([^}]+)\}");

            var result = regex.Replace(text, match =>
            {
                var key = match.Groups[1].Value;
                if (testData.ContainsKey(key))
                {
                    _logger.LogDebug($"Replaced variable ${{{key}}} with value: {testData[key]}");
                    return testData[key];
                }
                _logger.LogWarning($"Variable ${{{key}}} not found in test data");
                return match.Value;
            });

            return result;
        }

        #endregion

        #region Screenshot Methods

        public async Task<byte[]> TakeScreenshotAsync(IWebDriver driver)
        {
            try
            {
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                return await Task.FromResult(screenshot.AsByteArray);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to take screenshot");
                throw;
            }
        }

        private string SaveScreenshot(byte[] screenshot, string filename)
        {
            try
            {
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
                var fileName = $"{filename}_{timestamp}.png";
                var filePath = Path.Combine(_screenshotsPath, fileName);

                File.WriteAllBytes(filePath, screenshot);

                return $"/screenshots/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save screenshot");
                throw;
            }
        }

        private async Task CaptureFailureScreenshotAsync(IWebDriver driver, TestCase testCase, TestStep step, TestExecutionResult result)
        {
            try
            {
                var screenshot = await TakeScreenshotAsync(driver);
                var screenshotPath = SaveScreenshot(screenshot, $"failure_{testCase.Id}_{step.Order}");
                result.Screenshots.Add(screenshotPath);
                _logger.LogInformation($"Failure screenshot saved: {screenshotPath}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to capture failure screenshot");
            }
        }

        private async Task CaptureErrorScreenshotAsync(IWebDriver driver, TestCase testCase, TestExecutionResult result)
        {
            try
            {
                var screenshot = await TakeScreenshotAsync(driver);
                var screenshotPath = SaveScreenshot(screenshot, $"error_{testCase.Id}");
                result.Screenshots.Add(screenshotPath);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to take screenshot after error");
            }
        }

        private async Task CaptureStepScreenshotAsync(IWebDriver driver, TestStep testStep, StepResult stepResult)
        {
            try
            {
                var screenshot = await TakeScreenshotAsync(driver);
                var screenshotPath = SaveScreenshot(screenshot, $"step_{testStep.Id}");
                stepResult.Screenshot = screenshotPath;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to take screenshot for step");
            }
        }

        #endregion

        #region Validation Methods

        private void ValidateTestRunConfiguration(TestRun testRun)
        {
            if (testRun.Environment == null)
            {
                throw new InvalidOperationException("Test run environment is not configured");
            }

            if (string.IsNullOrEmpty(testRun.Environment.BaseUrl))
            {
                throw new InvalidOperationException("Environment base URL is not configured");
            }
        }

        private void ValidateTestCase(TestCase testCase)
        {
            if (testCase.TestSteps == null || !testCase.TestSteps.Any())
            {
                throw new InvalidOperationException($"Test case '{testCase.Name}' has no test steps");
            }
        }

        public async Task<bool> ValidateStepAsync(TestStep testStep)
        {
            if (string.IsNullOrWhiteSpace(testStep.Action))
            {
                _logger.LogWarning("Step validation failed: Action is required");
                return false;
            }

            var actionsNeedingSelector = new[]
            {
                "click", "type", "select", "check", "checkbox",
                "assert", "hover", "clear", "assert_enabled", "assert_disabled"
            };

            if (actionsNeedingSelector.Contains(testStep.Action.ToLower()) &&
                string.IsNullOrWhiteSpace(testStep.Selector))
            {
                _logger.LogWarning($"Step validation failed: Selector is required for action '{testStep.Action}'");
                return false;
            }

            var actionsNeedingValue = new[] { "type", "navigate", "assert", "select" };

            if (actionsNeedingValue.Contains(testStep.Action.ToLower()) &&
                string.IsNullOrWhiteSpace(testStep.Value))
            {
                _logger.LogWarning($"Step validation failed: Value is required for action '{testStep.Action}'");
                return false;
            }

            return await Task.FromResult(true);
        }

        #endregion
    }
}