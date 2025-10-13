using Microsoft.AspNetCore.Mvc;
using TesterLab.Domain.interfaces.Services;
using TesterLab.Domain.Models;
using TesterLab.Models;

namespace TesterLab.Controllers
{
  public class ApplicationsController : Controller
  {
    private readonly IApplicationService _applicationService;

    public ApplicationsController(IApplicationService applicationService)
    {
      _applicationService = applicationService;
    }

    // GET: Applications
    public async Task<IActionResult> Index()
    {
      var applications = await _applicationService.GetAllApplicationsAsync();
      return View(applications);
    }

    // GET: Applications/Create
    public IActionResult Create()
    {
      return View(new Application());
    }

    // POST: Applications/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Application application)
    {
      if (ModelState.IsValid)
      {
        var result = await _applicationService.CreateApplicationAsync(application);

        if (result != null)
          return RedirectToAction(nameof(Index));

        ViewBag.Error = "Erreur lors de l'enregistrement";
        return View(application);
      }

      return View(application);
    }

    // GET: Applications/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
      var application = await _applicationService.GetApplicationByIdAsync(id);
      if (application == null)
        return RedirectToAction(nameof(Index));

      var dto = new ApplicationDto
      {
        Id = application.Id,
        Name = application.Name,
        Active = application.Active,
        AppType = application.AppType,
        Description = application.Description,
        MainUrl = application.MainUrl,
        CreatedAt = application.CreatedAt,
        UpdatedAt = application.UpdatedAt
      };

      return View(dto);
    }

    // POST: Applications/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ApplicationDto application)
    {
      if (id != application.Id)
        return NotFound();

      if (ModelState.IsValid)
      {
        var result = await _applicationService.GetApplicationByIdAsync(application.Id);
        if (result != null)
        {
          result.Name = application.Name;
          result.Active = application.Active;
          result.AppType = application.AppType;
          result.Description = application.Description;
          result.MainUrl = application.MainUrl;
          result.UpdatedAt = DateTime.UtcNow;

          await _applicationService.UpdateApplicationAsync(result);
          return RedirectToAction(nameof(Index));
        }
      }

      ViewBag.Error = "Erreur lors de la mise à jour";
      return View(application);
    }

    // GET: Applications/Details/5
    public async Task<IActionResult> Details(int id)
    {
      var application = await _applicationService.GetApplicationByIdAsync(id);
      if (application == null)
        return RedirectToAction(nameof(Index));

      return View(application);
    }

    // GET: Applications/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
      var application = await _applicationService.GetApplicationByIdAsync(id);
      if (application == null)
        return RedirectToAction(nameof(Index));

      return View(application);
    }

    // POST: Applications/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var application = await _applicationService.GetApplicationByIdAsync(id);
      if (application != null)
      {
        await _applicationService.DeleteApplicationAsync(id);
      }

      return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> SelectApplication(int id)
    {
      await _applicationService.SetSelectedAsync(id);

      return RedirectToAction(nameof(Index), "Dashboards");
    }

    public async Task<IActionResult> GetApplications()
    {
      var apps = await _applicationService.GetAllApplicationsAsync();

      var result = apps.Select(a => new
      {
        id = a.Id,
        name = a.Name,
        selected = a.Selected
      });
      var currentApp = apps.FirstOrDefault(x=>x.Selected);
      if(currentApp != null)
      {
        STATIC_CURRENT_APP.CurrentApp = currentApp;
      }
      return Json(result);
    }

  }
}
