using Microsoft.AspNetCore.Mvc;
using TesterLab.Domain.interfaces.Services;
using TesterLab.Domain.Models;
using TesterLab.Models;

namespace TesterLab.Controllers
{
  public class TestDataController : Controller
  {
    private readonly ITestDataService _testDataService;
    private readonly IApplicationService _applicationService;
    private readonly IEnvironmentService _environmentService;

    public TestDataController(
        ITestDataService testDataService,
        IApplicationService applicationService,
        IEnvironmentService environmentService)
    {
      _testDataService = testDataService;
      _applicationService = applicationService;
      _environmentService = environmentService;
    }

    // GET: TestData
    public async Task<IActionResult> Index(int? applicationId)
    {
      var currentApp = STATIC_CURRENT_APP.CurrentApp;
      var appId = applicationId ?? currentApp?.Id ?? 0;

      if (appId == 0)
        return RedirectToAction("Index", "Applications");

      var testData = await _testDataService.GetTestDataByApplicationAsync(appId);
      ViewBag.ApplicationId = appId;
      ViewBag.ApplicationName = currentApp?.Name;

      return View(testData);
    }

    // GET: TestData/Create
    public async Task<IActionResult> Create(int? applicationId)
    {
      var currentApp = STATIC_CURRENT_APP.CurrentApp;
      var appId = applicationId ?? currentApp?.Id ?? 0;

      if (appId == 0)
        return RedirectToAction("Index", "Applications");

      var applications = await _applicationService.GetAllApplicationsAsync();
      var environments = await _environmentService.GetEnvironmentsByApplicationAsync(appId);

      ViewBag.Application = currentApp;
      ViewBag.Applications = applications;
      ViewBag.Environments = environments;

      var testData = new TestData
      {
        ApplicationId = appId,
        DataJson = "{}"
      };

      return View(testData);
    }

    // POST: TestData/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TestData testData)
    {
      if (ModelState.IsValid)
      {
        try
        {
          var result = await _testDataService.CreateTestDataAsync(testData);

          if (result != null)
            return RedirectToAction(nameof(Index), new { applicationId = testData.ApplicationId });

          ViewBag.Error = "Erreur lors de l'enregistrement";
        }
        catch (Exception ex)
        {
          ViewBag.Error = ex.Message;
        }
      }

      var applications = await _applicationService.GetAllApplicationsAsync();
      var environments = await _environmentService.GetEnvironmentsByApplicationAsync(testData.ApplicationId);
      ViewBag.Environments = environments;

      return View(testData);
    }

    // GET: TestData/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
      var testData = await _testDataService.GetTestDataByIdAsync(id);
      if (testData == null)
        return RedirectToAction(nameof(Index));

      var applications = await _applicationService.GetAllApplicationsAsync();
      var environments = await _environmentService.GetEnvironmentsByApplicationAsync(testData.ApplicationId);

      ViewBag.Environments = environments;

      var dto = new TestDataDto
      {
        Id = testData.Id,
        ApplicationId = testData.ApplicationId,
        Name = testData.Name,
        Description = testData.Description,
        DataType = testData.DataType,
        DataJson = testData.DataJson,
        IsTemplate = testData.IsTemplate,
        SpecificEnvironmentId = testData.SpecificEnvironmentId
      };

      return View(dto);
    }

    // POST: TestData/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TestDataDto testDataDto)
    {
      if (id != testDataDto.Id)
        return NotFound();

      if (ModelState.IsValid)
      {
        try
        {
          var result = await _testDataService.GetTestDataByIdAsync(testDataDto.Id);
          if (result != null)
          {
            result.Name = testDataDto.Name;
            result.Description = testDataDto.Description;
            result.DataType = testDataDto.DataType;
            result.DataJson = testDataDto.DataJson;
            result.IsTemplate = testDataDto.IsTemplate;
            result.SpecificEnvironmentId = testDataDto.SpecificEnvironmentId;

            await _testDataService.UpdateTestDataAsync(result);
            return RedirectToAction(nameof(Index), new { applicationId = result.ApplicationId });
          }
        }
        catch (Exception ex)
        {
          ViewBag.Error = ex.Message;
        }
      }

      var applications = await _applicationService.GetAllApplicationsAsync();
      var environments = await _environmentService.GetEnvironmentsByApplicationAsync(testDataDto.ApplicationId);
      ViewBag.Applications = applications;
      ViewBag.Environments = environments;

      return View(testDataDto);
    }

    // GET: TestData/Details/5
    public async Task<IActionResult> Details(int id)
    {
      var testData = await _testDataService.GetTestDataByIdAsync(id);
      if (testData == null)
        return RedirectToAction(nameof(Index));

      return View(testData);
    }

    // GET: TestData/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
      var testData = await _testDataService.GetTestDataByIdAsync(id);
      if (testData == null)
        return RedirectToAction(nameof(Index));

      return View(testData);
    }

    // POST: TestData/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      try
      {
        var testData = await _testDataService.GetTestDataByIdAsync(id);
        if (testData != null)
        {
          await _testDataService.DeleteTestDataAsync(id);
          return RedirectToAction(nameof(Index), new { applicationId = testData.ApplicationId });
        }
      }
      catch (Exception ex)
      {
        ViewBag.Error = ex.Message;
      }

      return RedirectToAction(nameof(Index));
    }

  }
}
