using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System;
using TesterLab.Domain.interfaces.Services;
using TesterLab.Domain.Models;
using TesterLab.Models;

namespace TesterLab.Controllers
{
  public class FeaturesController : Controller
  {
    private readonly IFeatureService _featureService;
    private readonly IApplicationService _applicationService;

    public FeaturesController(IFeatureService featureService, IApplicationService applicationService)
    {
      _featureService = featureService;
      _applicationService = applicationService;
    }

    // GET: Features
    public async Task<IActionResult> Index()
    {
      var app = STATIC_CURRENT_APP.CurrentApp;
      if (app == null) { return View("Error"); }

      ViewBag.ApplicationName = app.Name;
      var features = await _featureService.GetFeaturesByApplicationAsync(app.Id);
      return View(features);
    }

    // GET: Features/Create
    public IActionResult Create()
    {
      return View(new Feature() { Active = true, ApplicationId = STATIC_CURRENT_APP.CurrentApp.Id });
    }

    // POST: Features/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Feature feature)
    {
      feature.Active = true;
      if (ModelState.IsValid)
      {
        var result = await _featureService.CreateFeatureAsync(feature);

        if (result != null)
          return RedirectToAction(nameof(Index));

        ViewBag.Error = "Erreur lors de l'enregistrement";
        return View(feature);
      }

      return View(feature);
    }

    // GET: Features/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
      var feature = await _featureService.GetFeatureByIdAsync(id);
      if (feature == null)
        return RedirectToAction(nameof(Index));

      var dto = new FeatureDto
      {
        Id = feature.Id,
        Name = feature.Name,
        Active = feature.Active,
        Description = feature.Description,
        ApplicationId = feature.ApplicationId,
        Complexity = feature.Complexity,
        Icon = feature.Icon,
        BusinessPriority = feature.BusinessPriority,
        CreatedAt = feature.CreatedAt,
        UpdatedAt = feature.UpdatedAt
      };

      return View(dto);
    }

    // POST: Features/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, FeatureDto feature)
    {
      if (id != feature.Id)
        return NotFound();

      if (ModelState.IsValid)
      {
        var result = await _featureService.GetFeatureByIdAsync(feature.Id);
        if (result != null)
        {
          result.Name = feature.Name;
          result.Active = feature.Active;
          result.Description = feature.Description;
          result.ApplicationId = feature.ApplicationId;
          result.Complexity = feature.Complexity;
          result.Icon = feature.Icon;
          result.BusinessPriority = feature.BusinessPriority;

          await _featureService.UpdateFeatureAsync(result);
          return RedirectToAction(nameof(Index));
        }
      }

      ViewBag.Error = "Erreur lors de la mise à jour";
      return View(feature);
    }

    // GET: Features/Details/5
    public async Task<IActionResult> Details(int id)
    {
      var feature = await _featureService.GetFeatureByIdAsync(id);
      if (feature == null)
        return RedirectToAction(nameof(Index));

      return View(feature);
    }

    // GET: Features/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
      var feature = await _featureService.GetFeatureByIdAsync(id);
      if (feature == null)
        return RedirectToAction(nameof(Index));

      return View(feature);
    }

    // POST: Features/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var feature = await _featureService.GetFeatureByIdAsync(id);
      if (feature != null)
      {
        await _featureService.DeleteFeatureAsync(id);
      }

      return RedirectToAction(nameof(Index));
    }
  }
}
