using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesterLab.Rappory.Models;

namespace TesterLab.Rappory.Services
{
    public interface IReportService
    {
        Task<byte[]> GeneratePdfReportAsync(int testRunId, ReportTemplate? template = null);
        Task<string> GenerateAndSavePdfReportAsync(int testRunId, ReportTemplate? template = null);
        Task<string> GenerateHtmlReportAsync(int testRunId, ReportTemplate? template = null);
        Task<ReportTemplate> GetDefaultTemplateAsync();
        Task<List<ReportTemplate>> GetTemplatesAsync();
        Task<ReportTemplate> SaveTemplateAsync(ReportTemplate template);
    }

    public class ReportService : IReportService
    {
        private readonly IReportDataService _reportDataService;
        private readonly IPdfReportGenerator _pdfGenerator;
        private readonly IHtmlReportGenerator _htmlGenerator;
        private readonly ILogger<ReportService> _logger;
        private readonly string _reportsPath;

        public ReportService(
            IReportDataService reportDataService,
            IPdfReportGenerator pdfGenerator,
            IHtmlReportGenerator htmlGenerator,
            ILogger<ReportService> logger,
            IWebHostEnvironment environment)
        {
            _reportDataService = reportDataService;
            _pdfGenerator = pdfGenerator;
            _htmlGenerator = htmlGenerator;
            _logger = logger;
            _reportsPath = Path.Combine(environment.WebRootPath, "reports");

            // Créer le dossier s'il n'existe pas
            if (!Directory.Exists(_reportsPath))
            {
                Directory.CreateDirectory(_reportsPath);
            }
        }

        public async Task<byte[]> GeneratePdfReportAsync(int testRunId, ReportTemplate? template = null)
        {
            template ??= await GetDefaultTemplateAsync();

            _logger.LogInformation("Génération du rapport PDF pour TestRun {TestRunId}", testRunId);

            // Collecter les données
            var reportData = await _reportDataService.CollectTestRunDataAsync(
                testRunId,
                template.IncludeHistoricalComparison);

            // Générer le PDF
            var pdfBytes = await _pdfGenerator.GenerateAsync(reportData, template);

            _logger.LogInformation("Rapport PDF généré avec succès ({Size} bytes)", pdfBytes.Length);

            return pdfBytes;
        }

        public async Task<string> GenerateAndSavePdfReportAsync(int testRunId, ReportTemplate? template = null)
        {
            template ??= await GetDefaultTemplateAsync();

            var reportData = await _reportDataService.CollectTestRunDataAsync(
                testRunId,
                template.IncludeHistoricalComparison);

            // Générer un nom de fichier unique
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var sanitizedName = SanitizeFileName(reportData.TestRunName);
            var fileName = $"Report_{sanitizedName}_{timestamp}.pdf";
            var outputPath = Path.Combine(_reportsPath, fileName);

            await _pdfGenerator.GenerateAndSaveAsync(reportData, template, outputPath);

            _logger.LogInformation("Rapport PDF sauvegardé: {FilePath}", outputPath);

            return $"/reports/{fileName}"; // URL relative
        }

        public async Task<string> GenerateHtmlReportAsync(int testRunId, ReportTemplate? template = null)
        {
            template ??= await GetDefaultTemplateAsync();

            var reportData = await _reportDataService.CollectTestRunDataAsync(
                testRunId,
                template.IncludeHistoricalComparison);

            var html = await _htmlGenerator.GenerateAsync(reportData, template);

            // Sauvegarder le HTML
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var sanitizedName = SanitizeFileName(reportData.TestRunName);
            var fileName = $"Report_{sanitizedName}_{timestamp}.html";
            var outputPath = Path.Combine(_reportsPath, fileName);

            await File.WriteAllTextAsync(outputPath, html);

            _logger.LogInformation("Rapport HTML sauvegardé: {FilePath}", outputPath);

            return $"/reports/{fileName}";
        }

        public async Task<ReportTemplate> GetDefaultTemplateAsync()
        {
            return await Task.FromResult(new ReportTemplate
            {
                Name = "Standard Report",
                Type = "Standard",
                IncludeSummary = true,
                IncludeCharts = true,
                IncludeScreenshots = true,
                IncludeFailedTestsOnly = false,
                IncludeStepDetails = true,
                IncludePerformanceMetrics = true,
                IncludeHistoricalComparison = true,
                OutputFormat = "PDF",
                PrimaryColor = "#007bff"
            });
        }

        public async Task<List<ReportTemplate>> GetTemplatesAsync()
        {
            // TODO: Charger depuis la base de données
            return await Task.FromResult(new List<ReportTemplate>
            {
                new ReportTemplate
                {
                    Id = 1,
                    Name = "Executive Summary",
                    Type = "Executive",
                    IncludeSummary = true,
                    IncludeCharts = true,
                    IncludeScreenshots = false,
                    IncludeFailedTestsOnly = false,
                    IncludeStepDetails = false,
                    PrimaryColor = "#007bff"
                },
                new ReportTemplate
                {
                    Id = 2,
                    Name = "Detailed Technical Report",
                    Type = "Detailed",
                    IncludeSummary = true,
                    IncludeCharts = true,
                    IncludeScreenshots = true,
                    IncludeFailedTestsOnly = false,
                    IncludeStepDetails = true,
                    IncludePerformanceMetrics = true,
                    IncludeHistoricalComparison = true,
                    PrimaryColor = "#28a745"
                },
                new ReportTemplate
                {
                    Id = 3,
                    Name = "Failures Only",
                    Type = "Minimal",
                    IncludeSummary = true,
                    IncludeCharts = false,
                    IncludeScreenshots = true,
                    IncludeFailedTestsOnly = true,
                    IncludeStepDetails = true,
                    PrimaryColor = "#dc3545"
                }
            });
        }

        public async Task<ReportTemplate> SaveTemplateAsync(ReportTemplate template)
        {
            // TODO: Sauvegarder en base de données
            _logger.LogInformation("Template sauvegardé: {TemplateName}", template.Name);
            return await Task.FromResult(template);
        }

        private string SanitizeFileName(string fileName)
        {
            var invalid = Path.GetInvalidFileNameChars();
            return string.Join("_", fileName.Split(invalid, StringSplitOptions.RemoveEmptyEntries))
                .Replace(" ", "_");
        }
    }
}
