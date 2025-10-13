using Microsoft.AspNetCore.Mvc;
using TesterLab.Domain.interfaces.Services;
using TesterLab.Domain.Models;
using TesterLab.Models;

namespace TesterLab.Controllers
{
  public class TestStepsController : Controller
  {
    private readonly ITestStepService _testStepService;
    private readonly ITestCaseService _testCaseService;
    private readonly IActionTemplateService _actionTemplateService;

    public TestStepsController(
        ITestStepService testStepService,
        ITestCaseService testCaseService,
        IActionTemplateService actionTemplateService)
    {
      _testStepService = testStepService;
      _testCaseService = testCaseService;
      _actionTemplateService = actionTemplateService;
    }

    // GET: TestSteps/Create?testCaseId=5
    public async Task<IActionResult> Create(int testCaseId)
    {
      var testCase = await _testCaseService.GetTestCaseWithStepsAsync(testCaseId);
      if (testCase == null)
        return RedirectToAction("Index", "TestCases");

      var templates = await _actionTemplateService.GetAllTemplatesAsync();
      ViewBag.ActionTemplates = templates;
      ViewBag.TestCase = testCase;

      var testStep = new TestStep
      {
        TestCaseId = testCaseId,
        TimeoutSeconds = 10
      };

      return View(testStep);
    }

    // POST: TestSteps/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TestStep testStep)
    {
      if (ModelState.IsValid)
      {
        var result = await _testStepService.CreateStepAsync(testStep);

        if (result != null)
          return RedirectToAction("Steps", "TestCases", new { id = testStep.TestCaseId });

        ViewBag.Error = "Erreur lors de l'enregistrement";
      }

      var testCase = await _testCaseService.GetTestCaseWithStepsAsync(testStep.TestCaseId);
      var templates = await _actionTemplateService.GetAllTemplatesAsync();
      ViewBag.ActionTemplates = templates;
      ViewBag.TestCase = testCase;

      return View(testStep);
    }

    // GET: TestSteps/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
      var testStep = await _testStepService.GetByIdAsync(id);
      if (testStep == null)
        return RedirectToAction("Index", "TestCases");

      var testCase = await _testCaseService.GetTestCaseWithStepsAsync(testStep.TestCaseId);
      var templates = await _actionTemplateService.GetAllTemplatesAsync();

      ViewBag.ActionTemplates = templates;
      ViewBag.TestCase = testCase;

      var dto = new TestStepDto
      {
        Id = testStep.Id,
        TestCaseId = testStep.TestCaseId,
        Action = testStep.Action,
        Target = testStep.Target,
        Selector = testStep.Selector,
        Value = testStep.Value,
        Order = testStep.Order,
        Description = testStep.Description,
        IsOptional = testStep.IsOptional,
        TimeoutSeconds = testStep.TimeoutSeconds
      };

      return View(dto);
    }

    // POST: TestSteps/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TestStepDto testStepDto)
    {
      if (id != testStepDto.Id)
        return NotFound();

      if (ModelState.IsValid)
      {
        var result = await _testStepService.GetByIdAsync(testStepDto.Id);
        if (result != null)
        {
          result.Action = testStepDto.Action;
          result.Target = testStepDto.Target;
          result.Selector = testStepDto.Selector;
          result.Value = testStepDto.Value;
          result.Order = testStepDto.Order;
          result.Description = testStepDto.Description;
          result.IsOptional = testStepDto.IsOptional;
          result.TimeoutSeconds = testStepDto.TimeoutSeconds;

          await _testStepService.UpdateStepAsync(result);
          return RedirectToAction("Steps", "TestCases", new { id = result.TestCaseId });
        }
      }

      var testCase = await _testCaseService.GetTestCaseWithStepsAsync(testStepDto.TestCaseId);
      var templates = await _actionTemplateService.GetAllTemplatesAsync();
      ViewBag.ActionTemplates = templates;
      ViewBag.TestCase = testCase;
      ViewBag.Error = "Erreur lors de la mise à jour";

      return View(testStepDto);
    }

    // GET: TestSteps/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
      var testStep = await _testStepService.GetByIdAsync(id);
      if (testStep == null)
        return RedirectToAction("Index", "TestCases");

      return View(testStep);
    }

    // POST: TestSteps/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var testStep = await _testStepService.GetByIdAsync(id);
      if (testStep != null)
      {
        var testCaseId = testStep.TestCaseId;
        await _testStepService.DeleteStepAsync(id);
        return RedirectToAction("Steps", "TestCases", new { id = testCaseId });
      }

      return RedirectToAction("Index", "TestCases");
    }

    // POST: TestSteps/Reorder
    [HttpPost]
    public async Task<IActionResult> Reorder(int testCaseId, int[] stepIds)
    {
      try
      {
        await _testStepService.ReorderStepsAsync(testCaseId, stepIds.ToList());
        return Json(new { success = true });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, error = ex.Message });
      }
    }

    // POST: TestSteps/InsertAfter
    [HttpPost]
    public async Task<IActionResult> InsertAfter(int afterStepId, TestStep newStep)
    {
      try
      {
        var result = await _testStepService.InsertStepAsync(newStep, afterStepId);
        return Json(new { success = true, stepId = result.Id });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, error = ex.Message });
      }
    }

    // API: Get steps by test case
    public async Task<IActionResult> GetByTestCase(int testCaseId)
    {
      var steps = await _testStepService.GetStepsByTestCaseAsync(testCaseId);

      var result = steps.Select(s => new
      {
        id = s.Id,
        action = s.Action,
        target = s.Target,
        selector = s.Selector,
        value = s.Value,
        order = s.Order,
        description = s.Description,
        isOptional = s.IsOptional,
        timeout = s.TimeoutSeconds
      });

      return Json(result);
    }
  }
}
