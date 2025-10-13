using Microsoft.AspNetCore.Mvc;
using TesterLab.Domain.interfaces.Services;
using TesterLab.Models;
using Environment = TesterLab.Domain.Models.Environment;

namespace TesterLab.Controllers
{
  public class EnvironmentsController : Controller
  {
    private readonly IEnvironmentService _environmentService;
    private readonly IApplicationService _applicationService;

    public EnvironmentsController(
        IEnvironmentService environmentService,
        IApplicationService applicationService)
    {
      _environmentService = environmentService;
      _applicationService = applicationService;
    }

    // GET: Environments
    public async Task<IActionResult> Index(int? applicationId)
    {
      var currentApp = STATIC_CURRENT_APP.CurrentApp;
      var appId = applicationId ?? currentApp?.Id ?? 0;

      if (appId == 0)
        return RedirectToAction("Index", "Applications");

      var environments = await _environmentService.GetEnvironmentsByApplicationAsync(appId);
      ViewBag.ApplicationId = appId;
      ViewBag.ApplicationName = currentApp?.Name;

      return View(environments);
    }

    // GET: Environments/Create
    public async Task<IActionResult> Create(int? applicationId)
    {
      var currentApp = STATIC_CURRENT_APP.CurrentApp;
      var appId = applicationId ?? currentApp?.Id ?? 0;

      if (appId == 0)
        return RedirectToAction("Index", "Applications");

      var applications = await _applicationService.GetAllApplicationsAsync();
      ViewBag.Applications = applications;

      var environment = new Environment { ApplicationId = appId };
      return View(environment);
    }

    // POST: Environments/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Environment environment)
    {
      if (ModelState.IsValid)
      {
        try
        {
          var result = await _environmentService.CreateEnvironmentAsync(environment);

          if (result != null)
            return RedirectToAction(nameof(Index), new { applicationId = environment.ApplicationId });

          ViewBag.Error = "Erreur lors de l'enregistrement";
        }
        catch (Exception ex)
        {
          ViewBag.Error = ex.Message;
        }
      }

      var applications = await _applicationService.GetAllApplicationsAsync();
      ViewBag.Applications = applications;
      return View(environment);
    }

    // GET: Environments/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
      var environment = await _environmentService.GetEnvironmentByIdAsync(id);
      if (environment == null)
        return RedirectToAction(nameof(Index));

      var applications = await _applicationService.GetAllApplicationsAsync();
      ViewBag.Applications = applications;

      var dto = new EnvironmentDto
      {
        Id = environment.Id,
        ApplicationId = environment.ApplicationId,
        Name = environment.Name,
        BaseUrl = environment.BaseUrl,
        Type = environment.Type,
        Description = environment.Description,
        RequiresAuth = environment.RequiresAuth,
        AccessInfo = environment.AccessInfo,
        Active = environment.Active
      };

      return View(dto);
    }

    // POST: Environments/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EnvironmentDto environmentDto)
    {
      if (id != environmentDto.Id)
        return NotFound();

      if (ModelState.IsValid)
      {
        try
        {
          var result = await _environmentService.GetEnvironmentByIdAsync(environmentDto.Id);
          if (result != null)
          {
            result.Name = environmentDto.Name;
            result.BaseUrl = environmentDto.BaseUrl;
            result.Type = environmentDto.Type;
            result.Description = environmentDto.Description;
            result.RequiresAuth = environmentDto.RequiresAuth;
            result.AccessInfo = environmentDto.AccessInfo;
            result.Active = environmentDto.Active;

            await _environmentService.UpdateEnvironmentAsync(result);
            return RedirectToAction(nameof(Index), new { applicationId = result.ApplicationId });
          }
        }
        catch (Exception ex)
        {
          ViewBag.Error = ex.Message;
        }
      }

      var applications = await _applicationService.GetAllApplicationsAsync();
      ViewBag.Applications = applications;
      return View(environmentDto);
    }

    // GET: Environments/Details/5
    public async Task<IActionResult> Details(int id)
    {
      var environment = await _environmentService.GetEnvironmentByIdAsync(id);
      if (environment == null)
        return RedirectToAction(nameof(Index));

      return View(environment);
    }

    // GET: Environments/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
      var environment = await _environmentService.GetEnvironmentByIdAsync(id);
      if (environment == null)
        return RedirectToAction(nameof(Index));

      return View(environment);
    }

    // POST: Environments/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var environment = await _environmentService.GetEnvironmentByIdAsync(id);
      if (environment != null)
      {
        await _environmentService.DeleteEnvironmentAsync(id);
      }

      return RedirectToAction(nameof(Index));
    }

    // POST: Environments/TestConnectivity/5
    [HttpPost]
    public async Task<IActionResult> TestConnectivity(int id)
    {
      try
      {
        var isAccessible = await _environmentService.TestEnvironmentConnectivityAsync(id);
        return Json(new { success = true, accessible = isAccessible });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, error = ex.Message });
      }
    }

    // GET: Environments/Clone/5
    public async Task<IActionResult> Clone(int id)
    {
      var environment = await _environmentService.GetEnvironmentByIdAsync(id);
      if (environment == null)
        return RedirectToAction(nameof(Index));

      ViewBag.OriginalEnvironment = environment;
      return View(new { Id = id, NewName = environment.Name + " (Copie)", NewUrl = environment.BaseUrl });
    }

    // POST: Environments/Clone/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Clone(int id, string newName, string newUrl)
    {
      if (string.IsNullOrWhiteSpace(newName))
      {
        ViewBag.Error = "Le nom est requis";
        var originalEnv = await _environmentService.GetEnvironmentByIdAsync(id);
        ViewBag.OriginalEnvironment = originalEnv;
        return View(new { Id = id, NewName = newName, NewUrl = newUrl });
      }

      try
      {
        var cloned = new Environment();// _environmentService.CloneEnvironmentAsync(id, newName, newUrl);
        return RedirectToAction(nameof(Details), new { id = cloned.Id });
      }
      catch (Exception ex)
      {
        ViewBag.Error = ex.Message;
        var originalEnv = await _environmentService.GetEnvironmentByIdAsync(id);
        ViewBag.OriginalEnvironment = originalEnv;
        return View(new { Id = id, NewName = newName, NewUrl = newUrl });
      }
    }

    // API: Get environments by application
    public async Task<IActionResult> GetByApplication(int applicationId)
    {
      var environments = await _environmentService.GetEnvironmentsByApplicationAsync(applicationId);

      var result = environments.Select(e => new
      {
        id = e.Id,
        name = e.Name,
        type = e.Type,
        baseUrl = e.BaseUrl,
        requiresAuth = e.RequiresAuth,
        active = e.Active
      });

      return Json(result);
    }
  }
}
