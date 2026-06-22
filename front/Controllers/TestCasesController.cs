using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using TesterLab.Domain.interfaces.Services;
using TesterLab.Domain.Models;
using TesterLab.Models;
using TesterLab.Models.Extentions;
using Microsoft.AspNetCore.SignalR;
using TesterLab.Hubs;
using TesterLab.JobScheduler.Services;

namespace TesterLab.Controllers
{
  [Authorize]
  public class TestCasesController : Controller
  {
    private readonly ITestCaseService _testCaseService;
    private readonly IFeatureService _featureService;
    private readonly ITestStepService _testStepService;
    private readonly IEnvironmentService _environmentService;
    private readonly ITestDataService _testDataService;
    private readonly ITestExecutionService3 _testExecutionService3;
    private readonly ITestStepImportService _importService;
    private readonly IHubContext<TestCaseHub> _hubContext;
    private readonly JobRepository _jobRepository;

    public TestCasesController(
        ITestCaseService testCaseService,
        IFeatureService featureService,
        ITestStepService testStepService,
        IEnvironmentService environmentService,
        ITestDataService testDataService,
        ITestExecutionService3 testExecutionService3,
        ITestStepImportService importService,
        IHubContext<TestCaseHub> hubContext,
        JobRepository jobRepository)
    {
      _testCaseService = testCaseService;
      _featureService = featureService;
      _testStepService = testStepService;
      _testExecutionService3 = testExecutionService3;
      _environmentService = environmentService;
      _testDataService = testDataService;
      _importService = importService;
      _hubContext = hubContext;
      _jobRepository = jobRepository;
    }

    // GET: TestCases
    public async Task<IActionResult> Index(int? featureId, int? applicationId)
    {
      var currentApp = STATIC_CURRENT_APP.CurrentApp;
      var appId = applicationId ?? currentApp?.Id ?? 0;

      if (appId == 0)
        return RedirectToAction("Index", "Applications");

      IEnumerable<TestCase> testCases;

      if (featureId.HasValue)
      {
        testCases = await _testCaseService.GetTestCasesByFeatureAsync(featureId.Value);
        var feature = await _featureService.GetFeatureByIdAsync(featureId.Value);
        ViewBag.FeatureName = feature?.Name;
        ViewBag.FeatureId = featureId;
      }
      else
      {
        testCases = await _testCaseService.GetTestCasesByApplicationAsync(appId);
      }

      var features = await _featureService.GetFeaturesByApplicationAsync(appId);
      ViewBag.Features = features;
      ViewBag.ApplicationId = appId;
      ViewBag.ApplicationName = currentApp?.Name;

      return View(testCases);
    }

    // GET: TestCases/Create
    public async Task<IActionResult> Create(int? featureId)
    {
      var currentApp = STATIC_CURRENT_APP.CurrentApp;
      if (currentApp == null)
        return RedirectToAction("Index", "Applications");

      var features = await _featureService.GetFeaturesByApplicationAsync(currentApp.Id);
      ViewBag.Features = features;

      var testCase = new TestCase();
      if (featureId.HasValue)
      {
        testCase.FeatureId = featureId.Value;
      }

      return View(testCase);
    }

    // POST: TestCases/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TestCase testCase)
    {
      if (ModelState.IsValid)
      {
        var result = await _testCaseService.CreateTestCaseAsync(testCase);

        if (result != null)
        {
          //if (testCase.TestSteps != null)
          //{
          //  foreach (var step in testCase.TestSteps)
          //  {
          //    step.TestCaseId = result.Id;
          //    await _testStepService.CreateAsync(step);
          //  }
          //}

          return RedirectToAction(nameof(Index), new { featureId = testCase.FeatureId });
        }

        ViewBag.Error = "Erreur lors de l'enregistrement";
      }

      var currentApp = STATIC_CURRENT_APP.CurrentApp;
      if (currentApp != null)
      {
        var features = await _featureService.GetFeaturesByApplicationAsync(currentApp.Id);
        ViewBag.Features = features;
      }

      return View(testCase);
    }

    // GET: TestCases/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
      var testCase = await _testCaseService.GetTestCaseWithStepsAsync(id);
      if (testCase == null)
        return RedirectToAction(nameof(Index));

      var currentApp = STATIC_CURRENT_APP.CurrentApp;
      if (currentApp != null)
      {
        var features = await _featureService.GetFeaturesByApplicationAsync(currentApp.Id);
        ViewBag.Features = features;
      }

      var dto = new TestCaseViewModel
      {
        Id = testCase.Id,
        FeatureId = testCase.FeatureId,
        Name = testCase.Name,
        Description = testCase.Description,
        CriticalityLevel = testCase.CriticalityLevel,
        ExecutionFrequency = testCase.ExecutionFrequency,
        Tags = testCase.Tags,
        EstimatedMinutes = testCase.EstimatedMinutes,
        UserPersona = testCase.UserPersona,
        Active = testCase.Active,
        TestSteps = testCase.TestSteps
                      .Select(x => new TestStepDto
                      {
                        Id = x.Id,
                        TestCaseId = x.TestCaseId,
                        Action = x.Action,
                        Description = x.Description,
                        IsOptional = x.IsOptional,
                        Order = x.Order,
                        Selector = x.Selector,
                        Target = x.Target,
                        TimeoutSeconds = x.TimeoutSeconds,
                        Value = x.Value
                      }
              ).ToList()
      };

      return View(dto);
    }

    // POST: TestCases/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TestCaseViewModel testCaseDto)
    {
      if (id != testCaseDto.Id)
        return NotFound();

      if (ModelState.IsValid)
      {
        var result = await _testCaseService.GetTestCaseWithStepsAsync(testCaseDto.Id);
        if (result != null)
        {
          result.Name = testCaseDto.Name;
          result.Description = testCaseDto.Description;
          result.FeatureId = testCaseDto.FeatureId;
          result.CriticalityLevel = testCaseDto.CriticalityLevel;
          result.ExecutionFrequency = testCaseDto.ExecutionFrequency;
          result.Tags = testCaseDto.Tags;
          result.EstimatedMinutes = testCaseDto.EstimatedMinutes;
          result.UserPersona = testCaseDto.UserPersona;
          result.Active = testCaseDto.Active;
          result.TestSteps = testCaseDto.TestSteps.ToModelCollection().ToList();
          await _testCaseService.UpdateTestCaseAsync(result);
          return RedirectToAction(nameof(Index), new { featureId = result.FeatureId });
        }
      }

      var currentApp = STATIC_CURRENT_APP.CurrentApp;
      if (currentApp != null)
      {
        var features = await _featureService.GetFeaturesByApplicationAsync(currentApp.Id);
        ViewBag.Features = features;
      }

      ViewBag.Error = "Erreur lors de la mise à jour";
      return View(testCaseDto);
    }

    // GET: TestCases/Details/5
    public async Task<IActionResult> Details(int id)
    {

      var currentApp = STATIC_CURRENT_APP.CurrentApp;
      if(currentApp == null) return NotFound();

      ViewBag.urlAction = currentApp?.MainUrl ?? "";
      var testCase = await _testCaseService.GetTestCaseWithStepsAsync(id);
      ViewBag.Environment = await _environmentService.GetEnvironmentsByApplicationAsync(currentApp.Id);
      ViewBag.TestData = await _testDataService.GetTestDataByApplicationAsync(currentApp.Id);
      ViewBag.Jobs = await _jobRepository.GetJobByTestCaseId(id);
      ViewBag.Executions = (await _testExecutionService3.GetTestRunByTestCaseIdAsync(id))
        .Select(x=> new
        {
          x.Status,
          date = ToRelativeTime(x.StartedAt),
          duration = FormatDuration(x.StartedAt, x.CompletedAt)
        });
      if (testCase == null)
        return RedirectToAction(nameof(Index));

      return View(testCase);
    }

    // GET: TestCases/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
      var testCase = await _testCaseService.GetTestCaseWithStepsAsync(id);
      if (testCase == null)
        return RedirectToAction(nameof(Index));

      return View(testCase);
    }

    // POST: TestCases/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var testCase = await _testCaseService.GetTestCaseWithStepsAsync(id);
      if (testCase != null)
      {
        await _testCaseService.DeleteTestCaseAsync(id);
      }

      return RedirectToAction(nameof(Index));
    }

    // GET: TestCases/Duplicate/5
    public async Task<IActionResult> Duplicate(int id)
    {
      var testCase = await _testCaseService.GetTestCaseWithStepsAsync(id);
      if (testCase == null)
        return RedirectToAction(nameof(Index));

      ViewBag.OriginalName = testCase.Name;
      return View(new { Id = id, NewName = testCase.Name + " (Copie)" });
    }

    // POST: TestCases/Duplicate/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Duplicate(int id, string newName)
    {
      if (string.IsNullOrWhiteSpace(newName))
      {
        ViewBag.Error = "Le nom est requis";
        var originalCase = await _testCaseService.GetTestCaseWithStepsAsync(id);
        ViewBag.OriginalName = originalCase?.Name;
        return View(new { Id = id, NewName = newName });
      }

      try
      {
        var duplicated = await _testCaseService.DuplicateTestCaseAsync(id, newName);
        return RedirectToAction(nameof(Details), new { id = duplicated.Id });
      }
      catch (Exception ex)
      {
        ViewBag.Error = ex.Message;
        var originalCase = await _testCaseService.GetTestCaseWithStepsAsync(id);
        ViewBag.OriginalName = originalCase?.Name;
        return View(new { Id = id, NewName = newName });
      }
    }

    // GET: TestCases/Steps/5
    public async Task<IActionResult> Steps(int id)
    {
      var testCase = await _testCaseService.GetTestCaseWithStepsAsync(id);
      if (testCase == null)
        return RedirectToAction(nameof(Index));

      return View(testCase);
    }

    // API: Search test cases
    public async Task<IActionResult> Search(string query)
    {
      var currentApp = STATIC_CURRENT_APP.CurrentApp;
      if (currentApp == null)
        return Json(new List<object>());

      var testCases = await _testCaseService.SearchTestCasesAsync(currentApp.Id, query ?? "");

      var result = testCases.Select(tc => new
      {
        id = tc.Id,
        name = tc.Name,
        description = tc.Description,
        featureName = tc.Feature?.Name,
        criticality = tc.CriticalityLevel,
        tags = tc.Tags?.Split(',') ?? new string[0],
        active = tc.Active
      });

      return Json(result);
    }

    // API: Get by feature
    public async Task<IActionResult> GetByFeature(int featureId)
    {
      var testCases = await _testCaseService.GetTestCasesByFeatureAsync(featureId);

      var result = testCases.Select(tc => new
      {
        id = tc.Id,
        name = tc.Name,
        criticality = tc.CriticalityLevel,
        stepsCount = tc.TestSteps?.Count ?? 0,
        active = tc.Active
      });

      return Json(result);
    }

    [HttpPost]
    public async Task<IActionResult> ExecuteTestCase(int testCaseId, int environmentId, int? testDataId = null)
    {

      var currentApp = STATIC_CURRENT_APP.CurrentApp;
      var appId = currentApp?.Id ?? 0;
      // Créer un TestRun pour ce TestCase spécifique
      var testRun = new TestRun
      {
        ApplicationId = appId, // ID de l'application
        Name = $"Manual execution - TestCase #{testCaseId}",
        ExecutionType = "testcase", // Type important !
        TargetIds = JsonSerializer.Serialize(new[] { testCaseId }), // Un seul ID
        EnvironmentId = environmentId,
        TestDataId = testDataId, // Optionnel
        Browser = "Chrome",
        Headless = true,
        Trigger = "Manual"
      };

      // Créer le TestRun
      var createdRun = await _testExecutionService3.CreateTestRunAsync(testRun);

      // Lancer l'exécution
      await _testExecutionService3.StartTestRunAsync(createdRun.Id);

      return RedirectToAction("TestRunDetails", "Dashboards", new { id = createdRun.Id });
    }

    public IActionResult enregistrerLesEtapes(int id)
    {
      ViewData["Layout"] = null;
      ViewBag.testCaseId = id;
      var baseApp = STATIC_CURRENT_APP.CurrentApp;
      ViewBag.urlAction = baseApp?.MainUrl ?? "";
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> ImportStepsFromJson(int testCaseId, string stepsJson)
    {
        if (string.IsNullOrWhiteSpace(stepsJson))
        {
            return BadRequest("Le contenu JSON est vide.");
        }

        var result = await _importService.ImportFromJsonAsync(
            testCaseId,
            stepsJson,
            replaceExisting: true); // ou false pour ajouter aux étapes existantes

        if (!result.Success)
        {
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        return Json(new
        {
            success = result.Success,
            count = result.ImportedCount,
            message = result.Message
        });
    }

    [HttpPost("api/testcases/{testCaseId}/append-steps-json")]
    [Produces("application/json")]
    public async Task<IActionResult> AppendStepsFromJsonApi([FromRoute] int testCaseId, [FromBody] JsonElement body)
    {
      string stepsJson = body.ValueKind == JsonValueKind.String
        ? body.GetString() ?? ""
        : body.GetRawText();
      if (string.IsNullOrWhiteSpace(stepsJson))
      {
          return BadRequest("Le contenu JSON est vide.");
      }

      var result = await _importService.ImportFromJsonAsync(
          testCaseId,
          stepsJson,
          replaceExisting: false); // Ajouter aux étapes existantes

      if (!result.Success)
      {
          return BadRequest(new { message = result.Message, errors = result.Errors });
      }

      var payload = new { success = result.Success, count = result.ImportedCount, message = result.Message, testCaseId };
      await _hubContext.Clients.Group($"testcase-{testCaseId}").SendAsync("StepsImported", payload);

      return Json(new
      {
          success = result.Success,
          count = result.ImportedCount,
          message = result.Message
      });
    }

    [HttpGet]
    public async Task<IActionResult> StepsPartial(int testCaseId)
    {
        var testCase = await _testCaseService.GetTestCaseWithStepsAsync(testCaseId);
        if (testCase == null) return NotFound();
        return PartialView("_TestStepsPartial", testCase);
    }

    private string ToRelativeTime(DateTime dateTime)
    {
      var now = DateTime.Now;
      var diff = now - dateTime;

      if (diff.TotalSeconds < 60)
        return "À l'instant";

      if (diff.TotalMinutes < 60)
        return $"Il y a {(int)diff.TotalMinutes} min";

      if (diff.TotalHours < 24)
      {
        int hours = (int)diff.TotalHours;
        int minutes = diff.Minutes;

        return minutes > 0
          ? $"Il y a {hours}h {minutes}min"
          : $"Il y a {hours}h";
      }

      if (dateTime.Date == now.Date.AddDays(-1))
        return $"Hier à {dateTime:HH:mm}";

      if (diff.TotalDays < 7)
        return $"Il y a {(int)diff.TotalDays} jours";

      if (dateTime.Year == now.Year)
        return dateTime.ToString("dd/MM à HH:mm");

      return dateTime.ToString("dd/MM/yyyy");
    }
    private string FormatDuration(DateTime start, DateTime? end)
    {
      if (!end.HasValue)
        return "En cours";

      TimeSpan duration = end.Value - start;

      if (duration.TotalDays >= 1)
        return $"{(int)duration.TotalDays}j {duration.Hours}h";

      if (duration.TotalHours >= 1)
        return $"{(int)duration.TotalHours}h {duration.Minutes}m";

      if (duration.TotalMinutes >= 1)
        return $"{duration.Minutes}m {duration.Seconds}s";

      return $"{duration.Seconds}s";
    }
  }
}
