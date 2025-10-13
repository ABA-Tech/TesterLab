using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using System.Text.Json;
using System.Text.RegularExpressions;
using TesterLab.Domain.Models;
using Microsoft.Extensions.Logging;
using TesterLab.Domain.interfaces.Services;

namespace TesterLab.Infrastructure.Selenium
{
    public class SeleniumTestExecutor : ITestExecutor
    {
        private readonly ILogger<SeleniumTestExecutor> _logger;
        private readonly string _screenshotsPath;

        public SeleniumTestExecutor(ILogger<SeleniumTestExecutor> logger)
        {
            _logger = logger;
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
                _logger.LogInformation($"Starting execution of test case: {testCase.Name}");

                // Vérifier que l'environnement est configuré
                if (testRun.Environment == null)
                {
                    throw new InvalidOperationException("Test run environment is not configured");
                }

                if (string.IsNullOrEmpty(testRun.Environment.BaseUrl))
                {
                    throw new InvalidOperationException("Environment base URL is not configured");
                }

                // Vérifier qu'il y a des steps à exécuter
                if (testCase.TestSteps == null || !testCase.TestSteps.Any())
                {
                    throw new InvalidOperationException($"Test case '{testCase.Name}' has no test steps");
                }

                // Initialiser le driver
                driver = InitializeDriver(testRun.Browser, testRun.Headless);
                _logger.LogInformation($"WebDriver initialized: {testRun.Browser} (Headless: {testRun.Headless})");

                // Charger les données de test
                var testData = LoadTestData(testRun.TestData);
                _logger.LogInformation($"Test data loaded: {testData.Count} variables");

                // Naviguer vers l'URL de base
                _logger.LogInformation($"Navigating to base URL: {testRun.Environment.BaseUrl}");
                driver.Navigate().GoToUrl(testRun.Environment.BaseUrl);
                await Task.Delay(2000); // Attendre le chargement initial

                // Exécuter chaque étape
                var orderedSteps = testCase.TestSteps.OrderBy(s => s.Order).ToList();
                _logger.LogInformation($"Executing {orderedSteps.Count} test steps");

                foreach (var step in orderedSteps)
                {
                    _logger.LogInformation($"Executing step {step.Order}/{orderedSteps.Count}: {step.Action} - {step.Description}");

                    var stepResult = await ExecuteTestStepAsync(step, driver, testData);
                    result.StepResults.Add(stepResult);

                    if (!stepResult.Success)
                    {
                        if (!step.IsOptional)
                        {
                            result.Success = false;
                            result.Message = $"Step {step.Order} failed: {stepResult.ErrorMessage}";
                            result.ErrorDetails = stepResult.ErrorMessage;

                            // Prendre une capture d'écran de l'échec
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

                            break;
                        }
                        else
                        {
                            _logger.LogWarning($"Optional step {step.Order} failed but continuing: {stepResult.ErrorMessage}");
                        }
                    }
                }

                // Si toutes les étapes ont réussi ou que seules les optionnelles ont échoué
                var mandatorySteps = result.StepResults.Where(sr =>
                    testCase.TestSteps.First(ts => ts.Id == sr.StepId).IsOptional == false);

                if (mandatorySteps.All(sr => sr.Success))
                {
                    result.Success = true;
                    var failedOptional = result.StepResults.Count(sr => !sr.Success);
                    result.Message = failedOptional > 0
                        ? $"Test completed with {failedOptional} optional step(s) failed"
                        : $"Test completed successfully with {result.StepResults.Count} steps";
                }

                result.Duration = DateTime.UtcNow - startTime;

                _logger.LogInformation($"Test case execution completed. Success: {result.Success}, Duration: {result.Duration.TotalSeconds:F2}s");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error executing test case: {testCase.Name}");

                result.Success = false;
                result.Message = "Test execution failed with exception";
                result.ErrorDetails = $"{ex.Message}\n{ex.StackTrace}";
                result.Duration = DateTime.UtcNow - startTime;

                // Capture d'écran de l'erreur
                if (driver != null)
                {
                    try
                    {
                        var screenshot = await TakeScreenshotAsync(driver);
                        var screenshotPath = SaveScreenshot(screenshot, $"error_{testCase.Id}");
                        result.Screenshots.Add(screenshotPath);
                    }
                    catch (Exception screenshotEx)
                    {
                        _logger.LogWarning(screenshotEx, "Failed to take screenshot after error");
                    }
                }
            }
            finally
            {
                // Fermer le driver
                if (driver != null)
                {
                    try
                    {
                        driver.Quit();
                        driver.Dispose();
                        _logger.LogInformation("WebDriver closed successfully");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error closing WebDriver");
                    }
                }
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

                _logger.LogDebug($"Step {testStep.Order}: Action={testStep.Action}, Selector={selector}, Value={value}");

                // Attendre que l'élément soit disponible si nécessaire
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(testStep.TimeoutSeconds));

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

                _logger.LogInformation($"Step {testStep.Order} completed successfully");
            }
            catch (NoSuchElementException ex)
            {
                stepResult.Success = false;
                stepResult.ErrorMessage = $"Element not found: {testStep.Selector}";
                _logger.LogError(ex, stepResult.ErrorMessage);
            }
            catch (TimeoutException ex)
            {
                stepResult.Success = false;
                stepResult.ErrorMessage = $"Timeout waiting for element: {testStep.Selector} (timeout: {testStep.TimeoutSeconds}s)";
                _logger.LogError(ex, stepResult.ErrorMessage);
            }
            catch (ElementNotInteractableException ex)
            {
                stepResult.Success = false;
                stepResult.ErrorMessage = $"Element not interactable: {testStep.Selector}";
                _logger.LogError(ex, stepResult.ErrorMessage);
            }
            catch (StaleElementReferenceException ex)
            {
                stepResult.Success = false;
                stepResult.ErrorMessage = $"Element became stale: {testStep.Selector}";
                _logger.LogError(ex, stepResult.ErrorMessage);
            }
            catch (Exception ex)
            {
                stepResult.Success = false;
                stepResult.ErrorMessage = $"{ex.GetType().Name}: {ex.Message}";
                _logger.LogError(ex, $"Error executing step {testStep.Order}");
            }
            finally
            {
                stepResult.Duration = DateTime.UtcNow - startTime;

                // Prendre une capture d'écran après chaque étape importante
                if (!stepResult.Success || testStep.Action.ToLower() == "assert")
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
            }

            return stepResult;
        }

        // ===============================================
        // Actions Selenium
        // ===============================================

        private async Task ExecuteNavigateAsync(IWebDriver driver, string url, TestStep testStep)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("URL cannot be empty for navigate action");
            }

            if (url.StartsWith("/"))
            {
                // URL relative - utiliser le domaine actuel
                var currentUri = new Uri(driver.Url);
                var baseUrl = $"{currentUri.Scheme}://{currentUri.Host}{(currentUri.Port != 80 && currentUri.Port != 443 ? ":" + currentUri.Port : "")}";
                var fullUrl = baseUrl + url;
                _logger.LogDebug($"Navigating to relative URL: {fullUrl}");
                driver.Navigate().GoToUrl(fullUrl);
            }
            else
            {
                _logger.LogDebug($"Navigating to absolute URL: {url}");
                driver.Navigate().GoToUrl(url);
            }

            await Task.Delay(1500); // Attendre le chargement de la page
        }

        private async Task ExecuteClickAsync(IWebDriver driver, WebDriverWait wait, TestStep testStep)
        {
            var element = FindElement(driver, wait, testStep);

            // Scroll vers l'élément si nécessaire
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({behavior: 'smooth', block: 'center'});", element);
            await Task.Delay(500);

            // Attendre que l'élément soit cliquable
            wait.Until(d => {
                try
                {
                    return element.Displayed && element.Enabled;
                }
                catch
                {
                    return false;
                }
            });

            // Essayer de cliquer, sinon utiliser JavaScript
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
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({behavior: 'smooth', block: 'center'});", element);
            await Task.Delay(500);

            // Attendre que l'élément soit visible
            wait.Until(d => {
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

            // Saisir le texte caractère par caractère pour simuler la saisie humaine
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

            // Récupérer le texte de l'élément
            var actualValue = element.Text;

            // Comparaison
            if (!actualValue.Contains(expectedValue, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception($"Assertion failed. Expected text containing: '{expectedValue}', but got: '{actualValue}'");
            }

            _logger.LogInformation($"Assertion passed: found '{expectedValue}' in '{actualValue}'");
            await Task.CompletedTask;
        }

        private async Task ExecuteWaitAsync(IWebDriver driver, TestStep testStep, string value)
        {
            if (int.TryParse(value, out int seconds))
            {
                // Attendre un nombre de secondes
                _logger.LogDebug($"Waiting for {seconds} seconds");
                await Task.Delay(seconds * 1000);
            }
            else if (!string.IsNullOrEmpty(testStep.Selector))
            {
                // Attendre qu'un élément soit présent
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

            // Essayer de sélectionner par texte visible
            try
            {
                select.SelectByText(value);
                _logger.LogDebug($"Selected by text: {value}");
            }
            catch
            {
                // Si échec, essayer par valeur
                try
                {
                    select.SelectByValue(value);
                    _logger.LogDebug($"Selected by value: {value}");
                }
                catch
                {
                    // Dernier essai: par index si c'est un nombre
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
                // Revenir au contenu principal
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

        // ===============================================
        // Méthodes utilitaires
        // ===============================================
        private IWebDriver InitializeDriver(string browser, bool headless)
        {
            IWebDriver driver = null;

            try
            {
                switch (browser?.ToLower() ?? "chrome")
                {
                    case "chrome":
                        // Détection du système
                        string baseDir = AppContext.BaseDirectory;
                        string osFolder = OperatingSystem.IsWindows() ? "win" : "linux";
                        string driverFolder = Path.Combine(baseDir, "Drivers", "Chrome");

                        // Chemins binaires
                        string chromeBinary = OperatingSystem.IsWindows()
                            ? Path.Combine(driverFolder, "chrome.exe")
                            : Path.Combine(driverFolder, "chrome");

                        string chromeDriverPath = OperatingSystem.IsWindows()
                            ? Path.Combine(driverFolder, "chromedriver.exe")
                            : Path.Combine(driverFolder, "chromedriver");

                        // Vérification d'existence
                        if (!File.Exists(chromeBinary))
                            throw new FileNotFoundException($"Chrome binary not found at: {chromeBinary}");

                        if (!File.Exists(chromeDriverPath))
                            throw new FileNotFoundException($"ChromeDriver not found at: {chromeDriverPath}");

                        // Essaye d'ajouter les permissions d'exécution sous Linux
                        if (!OperatingSystem.IsWindows())
                        {
                            try
                            {
                                var chmod = new System.Diagnostics.ProcessStartInfo
                                {
                                    FileName = "/bin/chmod",
                                    Arguments = $"+x \"{chromeBinary}\" \"{chromeDriverPath}\"",
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true,
                                    UseShellExecute = false
                                };
                                using var p = System.Diagnostics.Process.Start(chmod);
                                p?.WaitForExit(2000);
                            }
                            catch { /* ignore */ }
                        }

                        // Options Chrome
                        var chromeOptions = new ChromeOptions();
                        chromeOptions.BinaryLocation = chromeBinary;

                        if (headless)
                        {
                            chromeOptions.AddArgument("--headless=new");
                            chromeOptions.AddArgument("--disable-gpu");
                        }

                        chromeOptions.AddArgument("--no-sandbox");
                        chromeOptions.AddArgument("--disable-dev-shm-usage");
                        chromeOptions.AddArgument("--disable-blink-features=AutomationControlled");
                        chromeOptions.AddArgument("--window-size=1920,1080");
                        chromeOptions.AddArgument("--disable-extensions");
                        chromeOptions.AddArgument("--disable-software-rasterizer");
                        chromeOptions.AddUserProfilePreference("credentials_enable_service", false);
                        chromeOptions.AddUserProfilePreference("profile.password_manager_enabled", false);

                        // Crée le service ChromeDriver dans le dossier fourni
                        var chromeService = ChromeDriverService.CreateDefaultService(driverFolder);
                        chromeService.HideCommandPromptWindow = true;
                        chromeService.SuppressInitialDiagnosticInformation = true;

                        driver = new ChromeDriver(chromeService, chromeOptions, TimeSpan.FromSeconds(60));
                        break;

                    case "firefox":
                        var firefoxOptions = new FirefoxOptions();
                        if (headless)
                            firefoxOptions.AddArgument("--headless");

                        firefoxOptions.SetPreference("dom.webdriver.enabled", false);
                        firefoxOptions.SetPreference("useAutomationExtension", false);
                        driver = new FirefoxDriver(firefoxOptions);
                        break;

                    case "edge":
                        var edgeOptions = new EdgeOptions();
                        if (headless)
                            edgeOptions.AddArgument("--headless=new");

                        edgeOptions.AddArgument("--no-sandbox");
                        edgeOptions.AddArgument("--disable-dev-shm-usage");
                        driver = new EdgeDriver(edgeOptions);
                        break;

                    default:
                        throw new NotSupportedException($"Browser '{browser}' is not supported");
                }

                // Configuration commune
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
                driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60);
                driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(30);

                if (!headless)
                {
                    driver.Manage().Window.Maximize();
                }

                _logger.LogInformation($"WebDriver initialized successfully: {browser}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to initialize WebDriver for browser: {browser}");
                throw new InvalidOperationException(
                    $"Failed to initialize {browser} driver. Make sure the driver and Chrome binary are installed and accessible.",
                    ex
                );
            }

            return driver;
        }

        private IWebElement FindElement(IWebDriver driver, WebDriverWait wait, TestStep testStep)
        {
            if (string.IsNullOrEmpty(testStep.Selector))
            {
                throw new ArgumentException($"Selector is required for action '{testStep.Action}'");
            }

            var by = GetBy(testStep.Selector);

            // Attendre que l'élément soit présent
            try
            {
                wait.Until(d => d.FindElement(by));
            }
            catch (WebDriverTimeoutException)
            {
                throw new NoSuchElementException($"Element not found within {testStep.TimeoutSeconds} seconds: {testStep.Selector}");
            }

            return driver.FindElement(by);
        }

        private By GetBy(string selector)
        {
            if (string.IsNullOrEmpty(selector))
                throw new ArgumentException("Selector cannot be null or empty");

            // Déterminer le type de sélecteur
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
                // CSS Attribute selector
                return By.CssSelector(selector);
            }
            else
            {
                // Par défaut, utiliser CSS Selector
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

            // Remplacer les variables au format ${variable}
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
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var fileName = $"{filename}_{timestamp}.png";
                var filePath = Path.Combine(_screenshotsPath, fileName);

                File.WriteAllBytes(filePath, screenshot);

                // Retourner le chemin relatif pour l'accès web
                return $"/screenshots/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save screenshot");
                throw;
            }
        }

        public async Task<bool> ValidateStepAsync(TestStep testStep)
        {
            if (string.IsNullOrWhiteSpace(testStep.Action))
            {
                _logger.LogWarning("Step validation failed: Action is required");
                return false;
            }

            // Actions nécessitant un sélecteur
            var actionsNeedingSelector = new[] { "click", "type", "select", "check", "checkbox", "assert", "hover", "clear" };
            if (actionsNeedingSelector.Contains(testStep.Action.ToLower()) && string.IsNullOrWhiteSpace(testStep.Selector))
            {
                _logger.LogWarning($"Step validation failed: Selector is required for action '{testStep.Action}'");
                return false;
            }

            // Actions nécessitant une valeur
            var actionsNeedingValue = new[] { "type", "navigate", "assert", "select" };
            if (actionsNeedingValue.Contains(testStep.Action.ToLower()) && string.IsNullOrWhiteSpace(testStep.Value))
            {
                _logger.LogWarning($"Step validation failed: Value is required for action '{testStep.Action}'");
                return false;
            }

            return await Task.FromResult(true);
        }
    }
}
