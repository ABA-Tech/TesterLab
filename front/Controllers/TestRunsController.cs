using Microsoft.AspNetCore.Mvc;
using TesterLab.Applications.Services;
using TesterLab.Domain.interfaces.Services;
using TesterLab.Domain.Models;
using TesterLab.Models;

namespace TesterLab.Controllers
{
  public class TestRunsController : Controller
  {
    private readonly ITestExecutionService _testExecutionService;
    private readonly ITestExecutionService2 _testExecutionService2;
    private readonly ITestCaseService _testCaseService;
    private readonly IFeatureService _featureService;
    private readonly IEnvironmentService _environmentService;
    private readonly ITestDataService _testDataService;

    public TestRunsController(
        ITestExecutionService testExecutionService,
        ITestExecutionService2 testExecutionService2,
        ITestCaseService testCaseService,
        IFeatureService featureService,
        IEnvironmentService environmentService,
        ITestDataService testDataService)
    {
      _testExecutionService = testExecutionService;
      _testExecutionService2 = testExecutionService2;
      _testCaseService = testCaseService;
      _featureService = featureService;
      _environmentService = environmentService;
      _testDataService = testDataService;
    }

    // GET: TestRuns
    public async Task<IActionResult> Index(int? applicationId)
    {
      var currentApp = STATIC_CURRENT_APP.CurrentApp;
      var appId = applicationId ?? currentApp?.Id ?? 0;

      if (appId == 0)
        return RedirectToAction("Index", "Applications");

      var testRuns = await _testExecutionService.GetRecentRunsAsync(appId);
      ViewBag.ApplicationId = appId;
      ViewBag.ApplicationName = currentApp?.Name;

      return View(testRuns);
    }

    // GET: TestRuns/Create
    public async Task<IActionResult> Create(int? applicationId)
    {
      var currentApp = STATIC_CURRENT_APP.CurrentApp;
      var appId = applicationId ?? currentApp?.Id ?? 0;

      if (appId == 0)
        return RedirectToAction("Index", "Applications");

      var features = await _featureService.GetFeaturesByApplicationAsync(appId);
      var testCases = await _testCaseService.GetTestCasesByApplicationAsync(appId);
      var environments = await _environmentService.GetEnvironmentsByApplicationAsync(appId);
      var testData = await _testDataService.GetTestDataByApplicationAsync(appId);

      ViewBag.Features = features;
      ViewBag.TestCases = testCases;
      ViewBag.Environments = environments;
      ViewBag.TestData = testData;

      var testRun = new TestRun
      {
        ApplicationId = appId,
        Name = $"Test Run - {DateTime.Now:dd/MM/yyyy HH:mm}",
        Browser = "Chrome",
        Headless = true
      };

      return View(testRun);
    }

    // POST: TestRuns/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TestRun testRun)
    {
      if (ModelState.IsValid)
      {
        try
        {
          var result = await _testExecutionService.CreateTestRunAsync(testRun);

          if (result != null)
          {
            // Auto-start if requested
            if (Request.Form.ContainsKey("startNow"))
            {
              await _testExecutionService.StartTestRunAsync(result.Id);
            }

            return RedirectToAction(nameof(Details), new { id = result.Id });
          }

          ViewBag.Error = "Erreur lors de l'enregistrement";
        }
        catch (Exception ex)
        {
          ViewBag.Error = ex.Message;
        }
      }

      // Reload dropdowns
      var features = await _featureService.GetFeaturesByApplicationAsync(testRun.ApplicationId);
      var testCases = await _testCaseService.GetTestCasesByApplicationAsync(testRun.ApplicationId);
      var environments = await _environmentService.GetEnvironmentsByApplicationAsync(testRun.ApplicationId);
      var testData = await _testDataService.GetTestDataByApplicationAsync(testRun.ApplicationId);

      ViewBag.Features = features;
      ViewBag.TestCases = testCases;
      ViewBag.Environments = environments;
      ViewBag.TestData = testData;

      return View(testRun);
    }

    // GET: TestRuns/Details/5
    public async Task<IActionResult> Details(int id)
    {
      var testRun = await _testExecutionService.GetTestRunByIdAsync(id);
      if (testRun == null)
        return RedirectToAction(nameof(Index));

      return View(testRun);
    }

    // POST: TestRuns/Start/5
    [HttpPost]
    public async Task<IActionResult> Start(int id)
    {
      try
      {
        var testRun = await _testExecutionService2.StartTestRunAsync(id);
        return Json(new { success = true, status = testRun.Status });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, error = ex.Message });
      }
    }

    // POST: TestRuns/Stop/5
    [HttpPost]
    public async Task<IActionResult> Stop(int id)
    {
      try
      {
        var testRun = await _testExecutionService.CompleteTestRunAsync(id, "Stopped", "{}");
        return Json(new { success = true, status = testRun.Status });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, error = ex.Message });
      }
    }

    // GET: TestRuns/Report/5
    public async Task<IActionResult> Report(int id)
    {
      try
      {
        var reportPath = await _testExecutionService.GenerateReportAsync(id);
        return Json(new { success = true, reportPath });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, error = ex.Message });
      }
    }

    // GET: TestRuns/Statistics
    public async Task<IActionResult> Statistics(int? applicationId)
    {
      var currentApp = STATIC_CURRENT_APP.CurrentApp;
      var appId = applicationId ?? currentApp?.Id ?? 0;

      if (appId == 0)
        return RedirectToAction("Index", "Applications");

      var stats = await _testExecutionService.GetRunStatisticsAsync(appId);
      ViewBag.ApplicationId = appId;
      ViewBag.ApplicationName = currentApp?.Name;

      return View(stats);
    }

    // API: Get progress of a test run
    public async Task<IActionResult> GetProgress(int id)
    {
      var testRun = await _testExecutionService.GetTestRunByIdAsync(id);
      if (testRun == null)
        return Json(new { found = false });

      return Json(new
      {
        found = true,
        id = testRun.Id,
        status = testRun.Status,
        progress = testRun.ProgressPercentage,
        passed = testRun.PassedCount,
        failed = testRun.FailedCount,
        skipped = testRun.SkippedCount,
        startedAt = testRun.StartedAt,
        completedAt = testRun.CompletedAt
      });
    }

    // API: Get running tests
    public async Task<IActionResult> GetRunningTests()
    {
      var runningTests = await _testExecutionService.GetRunningTestsAsync();

      var result = runningTests.Select(tr => new
      {
        id = tr.Id,
        name = tr.Name,
        progress = tr.ProgressPercentage,
        status = tr.Status,
        startedAt = tr.StartedAt
      });

      return Json(result);
    }
  }
}
