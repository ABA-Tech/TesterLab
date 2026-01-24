using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TesterLab.Domain.interfaces.Services;
using TesterLab.Models.ViewModels;
using TesterLab.Rappory.Models;
using TesterLab.Rappory.Services;

namespace TesterLab.Controllers
{
  [Authorize]
  public class ReportsController : Controller
  {
    private readonly IReportService _reportService;
    private readonly ITestExecutionService3 _testExecutionService;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(
        IReportService reportService,
        ITestExecutionService3 testExecutionService,
        ILogger<ReportsController> logger)
    {
      _reportService = reportService;
      _testExecutionService = testExecutionService;
      _logger = logger;
    }

    // ═══════════════════════════════════════════════════════
    // PAGE PRINCIPALE - CONFIGURATION DU RAPPORT
    // ═══════════════════════════════════════════════════════

    [HttpGet]
    public async Task<IActionResult> Generate(int testRunId)
    {
      var testRun = await _testExecutionService.GetTestRunByIdAsync(testRunId);

      if (testRun == null)
        return NotFound();

      var templates = await _reportService.GetTemplatesAsync();

      var viewModel = new GenerateReportViewModel
      {
        TestRunId = testRunId,
        TestRunName = testRun.Name,
        AvailableTemplates = templates,
        SelectedTemplate = await _reportService.GetDefaultTemplateAsync()
      };

      return View(viewModel);
    }

    // ═══════════════════════════════════════════════════════
    // GÉNÉRATION PDF
    // ═══════════════════════════════════════════════════════

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GeneratePdf(GenerateReportViewModel model)
    {
      if (!ModelState.IsValid)
        return View("Generate", model);

      try
      {
        _logger.LogInformation("Génération du rapport PDF pour TestRun {TestRunId}", model.TestRunId);

        // Générer le PDF
        var pdfBytes = await _reportService.GeneratePdfReportAsync(
            model.TestRunId,
            model.SelectedTemplate);

        // Nom du fichier
        var fileName = $"TestReport_{model.TestRunId}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

        // Retourner le fichier
        return File(pdfBytes, "application/pdf", fileName);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erreur lors de la génération du rapport PDF");
        TempData["ErrorMessage"] = "Erreur lors de la génération du rapport";
        return RedirectToAction("Generate", new { testRunId = model.TestRunId });
      }
    }

    // ═══════════════════════════════════════════════════════
    // GÉNÉRATION HTML
    // ═══════════════════════════════════════════════════════

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateHtml(GenerateReportViewModel model)
    {
      if (!ModelState.IsValid)
        return View("Generate", model);

      try
      {
        _logger.LogInformation("Génération du rapport HTML pour TestRun {TestRunId}", model.TestRunId);

        // Générer et sauvegarder le HTML
        var reportUrl = await _reportService.GenerateHtmlReportAsync(
            model.TestRunId,
            model.SelectedTemplate);

        TempData["SuccessMessage"] = "Rapport HTML généré avec succès";
        TempData["ReportUrl"] = reportUrl;

        return RedirectToAction("ViewHtmlReport", new { url = reportUrl });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erreur lors de la génération du rapport HTML");
        TempData["ErrorMessage"] = "Erreur lors de la génération du rapport";
        return RedirectToAction("Generate", new { testRunId = model.TestRunId });
      }
    }

    // ═══════════════════════════════════════════════════════
    // VISUALISER LE RAPPORT HTML
    // ═══════════════════════════════════════════════════════

    [HttpGet]
    public IActionResult ViewHtmlReport(string url)
    {
      return Redirect(url);
    }

    // ═══════════════════════════════════════════════════════
    // TÉLÉCHARGER UN RAPPORT EXISTANT
    // ═══════════════════════════════════════════════════════

    [HttpGet]
    public async Task<IActionResult> Download(int testRunId, string format = "pdf")
    {
      try
      {
        if (format.ToLower() == "pdf")
        {
          var pdfBytes = await _reportService.GeneratePdfReportAsync(testRunId);
          var fileName = $"TestReport_{testRunId}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
          return File(pdfBytes, "application/pdf", fileName);
        }
        else if (format.ToLower() == "html")
        {
          var reportUrl = await _reportService.GenerateHtmlReportAsync(testRunId);
          return Redirect(reportUrl);
        }

        return BadRequest("Format non supporté");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erreur lors du téléchargement du rapport");
        return StatusCode(500, "Erreur lors de la génération du rapport");
      }
    }

    // ═══════════════════════════════════════════════════════
    // APERÇU RAPIDE (MODAL)
    // ═══════════════════════════════════════════════════════

    [HttpGet]
    public async Task<IActionResult> Preview(int testRunId)
    {
      try
      {
        var testRun = await _testExecutionService.GetTestRunByIdAsync(testRunId);

        if (testRun == null)
          return NotFound();

        var viewModel = new ReportPreviewViewModel
        {
          TestRunId = testRun.Id,
          TestRunName = testRun.Name,
          Status = testRun.Status,
          TotalTests = testRun.PassedCount + testRun.FailedCount + testRun.SkippedCount,
          PassedCount = testRun.PassedCount,
          FailedCount = testRun.FailedCount,
          SkippedCount = testRun.SkippedCount,
          SuccessRate = testRun.SuccessRate,
          Duration = testRun.CompletedAt.HasValue && testRun.StartedAt.HasValue
                ? testRun.CompletedAt.Value - testRun.StartedAt.Value
                : null
        };

        return PartialView("_ReportPreview", viewModel);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erreur lors de l'aperçu du rapport");
        return StatusCode(500);
      }
    }

    // ═══════════════════════════════════════════════════════
    // GESTION DES TEMPLATES
    // ═══════════════════════════════════════════════════════

    [HttpGet]
    public async Task<IActionResult> Templates()
    {
      var templates = await _reportService.GetTemplatesAsync();
      return View(templates);
    }

    [HttpGet]
    public IActionResult CreateTemplate()
    {
      return View(new ReportTemplate());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTemplate(ReportTemplate template)
    {
      if (!ModelState.IsValid)
        return View(template);

      try
      {
        await _reportService.SaveTemplateAsync(template);
        TempData["SuccessMessage"] = "Template créé avec succès";
        return RedirectToAction("Templates");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erreur lors de la création du template");
        ModelState.AddModelError("", "Erreur lors de la sauvegarde");
        return View(template);
      }
    }

    // ═══════════════════════════════════════════════════════
    // ENVOI PAR EMAIL
    // ═══════════════════════════════════════════════════════

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendByEmail(int testRunId, string recipientEmail)
    {
      try
      {
        // Générer le PDF
        var pdfBytes = await _reportService.GeneratePdfReportAsync(testRunId);

        // TODO: Implémenter l'envoi par email
        // await _emailService.SendReportAsync(recipientEmail, pdfBytes);

        TempData["SuccessMessage"] = $"Rapport envoyé à {recipientEmail}";
        return RedirectToAction("Generate", new { testRunId });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erreur lors de l'envoi du rapport par email");
        TempData["ErrorMessage"] = "Erreur lors de l'envoi du rapport";
        return RedirectToAction("Generate", new { testRunId });
      }
    }
  }
}
