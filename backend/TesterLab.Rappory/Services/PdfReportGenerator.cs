using Microsoft.Extensions.Logging;
using QuestPDF.Infrastructure;
using TesterLab.Rappory.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using ScottPlot;
using System.IO;
using Colors = QuestPDF.Helpers.Colors;

namespace TesterLab.Rappory.Services
{
    public interface IPdfReportGenerator
    {
        Task<byte[]> GenerateAsync(TestRunReportData data, ReportTemplate template);
        Task<string> GenerateAndSaveAsync(TestRunReportData data, ReportTemplate template, string outputPath);
    }

    public class PdfReportGenerator : IPdfReportGenerator
    {
        private readonly ILogger<PdfReportGenerator> _logger;

        public PdfReportGenerator(ILogger<PdfReportGenerator> logger)
        {
            _logger = logger;

            // Configuration QuestPDF (requis pour la version Community)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> GenerateAsync(TestRunReportData data, ReportTemplate template)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var document = Document.Create(container =>
                    {
                        container.Page(page =>
                        {
                            page.Size(PageSizes.A4);
                            page.Margin(2, Unit.Centimetre);
                            page.PageColor(Colors.White);
                            page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                            // En-tête
                            page.Header().Element(c => ComposeHeader(c, data, template));

                            // Contenu principal
                            page.Content().Element(c => ComposeContent(c, data, template));

                            // Pied de page
                            page.Footer().Element(c => ComposeFooter(c, data));
                        });
                    });

                    return document.GeneratePdf();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur lors de la génération du PDF");
                    throw;
                }
            });
        }

        public async Task<string> GenerateAndSaveAsync(TestRunReportData data, ReportTemplate template, string outputPath)
        {
            var pdfBytes = await GenerateAsync(data, template);

            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllBytesAsync(outputPath, pdfBytes);

            _logger.LogInformation("Rapport PDF généré: {OutputPath}", outputPath);
            return outputPath;
        }

        // ═══════════════════════════════════════════════════════
        // COMPOSITION DU DOCUMENT
        // ═══════════════════════════════════════════════════════

        private void ComposeHeader(IContainer container, TestRunReportData data, ReportTemplate template)
        {
            container.Row(row =>
            {
                // Logo (si fourni)
                if (!string.IsNullOrEmpty(template.BrandingLogo) && File.Exists(template.BrandingLogo))
                {
                    row.ConstantItem(100).Image(template.BrandingLogo).FitWidth();
                }

                // Titre
                row.RelativeItem().Column(column =>
                {
                    column.Item().AlignCenter().Text("TEST EXECUTION REPORT")
                        .FontSize(24)
                        .Bold()
                        .FontColor(template.PrimaryColor);

                    column.Item().AlignCenter().Text(data.ApplicationName)
                        .FontSize(16)
                        .SemiBold();

                    column.Item().AlignCenter().Text($"Environment: {data.EnvironmentName}")
                        .FontSize(12)
                        .FontColor(Colors.Grey.Darken2);
                });
            });

            container.PaddingTop(10).BorderBottom(2).BorderColor(template.PrimaryColor);
        }

        private void ComposeContent(IContainer container, TestRunReportData data, ReportTemplate template)
        {
            container.Column(column =>
            {
                // Section: Résumé exécutif
                if (template.IncludeSummary)
                {
                    column.Item().Element(c => ComposeSummarySection(c, data, template));
                    column.Item().PaddingVertical(10);
                }

                // Section: Graphiques
                if (template.IncludeCharts)
                {
                    column.Item().Element(c => ComposeChartsSection(c, data, template));
                    column.Item().PaddingVertical(10);
                }

                // Section: Résultats des tests
                column.Item().Element(c => ComposeTestResultsSection(c, data, template));

                // Section: Comparaison historique
                if (template.IncludeHistoricalComparison && data.HistoricalData != null)
                {
                    column.Item().PaddingVertical(10);
                    column.Item().Element(c => ComposeHistoricalSection(c, data));
                }
            });
        }

        private void ComposeFooter(IContainer container, TestRunReportData data)
        {
            container.Row(row =>
            {
                row.RelativeItem().AlignLeft()
                    .Text($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}")
                    .FontSize(8)
                    .FontColor(Colors.Grey.Medium);

                row.RelativeItem().AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Page ").FontSize(80);
                        text.CurrentPageNumber().FontSize(80);
                        text.Span(" of ").FontSize(80);
                        text.TotalPages().FontSize(80);
                    });

                row.RelativeItem().AlignRight()
                    .Text("Powered by TesterLab")
                    .FontSize(8)
                    .FontColor(Colors.Grey.Medium);
            });
        }

        // ═══════════════════════════════════════════════════════
        // SECTIONS INDIVIDUELLES
        // ═══════════════════════════════════════════════════════

        private void ComposeSummarySection(IContainer container, TestRunReportData data, ReportTemplate template)
        {
            container.Column(column =>
            {
                // Titre de section
                column.Item().Text("Executive Summary")
                    .FontSize(18)
                    .Bold()
                    .FontColor(template.PrimaryColor);

                column.Item().PaddingTop(5).PaddingBottom(10);

                // Informations générales
                column.Item().Row(row =>
                {
                    // Colonne gauche
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text($"Test Run: {data.TestRunName}").SemiBold();
                        col.Item().Text($"Execution Date: {data.ExecutionDate:yyyy-MM-dd HH:mm}");
                        col.Item().Text($"Duration: {FormatDuration(data.Duration)}");
                        col.Item().Text($"Browser: {data.Browser} {(data.Headless ? "(Headless)" : "")}");
                        col.Item().Text($"Trigger: {data.Trigger}");
                    });

                    // Colonne droite - Statistiques
                    row.RelativeItem().Column(col =>
                    {
                        var statusColor = data.Summary.Status == "Completed" ? Colors.Green.Medium : Colors.Red.Medium;

                        col.Item().Text($"Status: {data.Summary.Status}")
                            .SemiBold()
                            .FontColor(statusColor);

                        col.Item().Text($"Total Tests: {data.Summary.TotalTests}");
                        col.Item().Text($"Passed: {data.Summary.PassedCount}").FontColor(Colors.Green.Darken1);
                        col.Item().Text($"Failed: {data.Summary.FailedCount}").FontColor(Colors.Red.Darken1);
                        col.Item().Text($"Skipped: {data.Summary.SkippedCount}").FontColor(Colors.Orange.Darken1);
                        col.Item().Text($"Success Rate: {data.Summary.SuccessRate:F2}%").Bold();
                    });
                });

                // Carte de statut global
                column.Item().PaddingTop(15).Background(GetStatusColor(data.Summary.SuccessRate))
                    .Padding(20)
                    .Column(col =>
                    {
                        col.Item().AlignCenter().Text(GetStatusText(data.Summary.SuccessRate))
                            .FontSize(24)
                            .Bold()
                            .FontColor(Colors.White);

                        col.Item().AlignCenter().Text($"{data.Summary.SuccessRate:F1}% Success Rate")
                            .FontSize(16)
                            .FontColor(Colors.White);
                    });
            });
        }

        private void ComposeChartsSection(IContainer container, TestRunReportData data, ReportTemplate template)
        {
            container.Column(column =>
            {
                column.Item().Text("Test Results Overview")
                    .FontSize(18)
                    .Bold()
                    .FontColor(template.PrimaryColor);

                column.Item().PaddingTop(10);

                // Graphique en camembert
                var chartPath = GeneratePieChart(data);
                if (File.Exists(chartPath))
                {
                    column.Item().Height(250).Image(chartPath);
                }

                // Graphique de durée par test
                if (data.TestCases.Count > 0)
                {
                    column.Item().PaddingTop(20);
                    var durationChartPath = GenerateDurationChart(data);
                    if (File.Exists(durationChartPath))
                    {
                        column.Item().Height(200).Image(durationChartPath);
                    }
                }
            });
        }

        private void ComposeTestResultsSection(IContainer container, TestRunReportData data, ReportTemplate template)
        {
            container.Column(column =>
            {
                column.Item().PageBreak();

                column.Item().Text("Detailed Test Results")
                    .FontSize(18)
                    .Bold()
                    .FontColor(template.PrimaryColor);

                column.Item().PaddingTop(10);

                var testsToShow = template.IncludeFailedTestsOnly
                    ? data.TestCases.Where(tc => tc.Status == "Failed").ToList()
                    : data.TestCases;

                foreach (var testCase in testsToShow)
                {
                    column.Item().PaddingVertical(5)
                        .Element(c => ComposeTestCaseResult(c, testCase, template));
                }
            });
        }

        private void ComposeTestCaseResult(IContainer container, TestCaseResultData testCase, ReportTemplate template)
        {
            container.Border(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(10)
                .Column(column =>
                {
                    // En-tête du test case
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Text(testCase.TestCaseName)
                            .FontSize(14)
                            .Bold();

                        row.ConstantItem(80).AlignRight()
                            .Text(testCase.Status)
                            .FontColor(GetStatusTextColor(testCase.Status))
                            .Bold();
                    });

                    // Description
                    if (!string.IsNullOrEmpty(testCase.Description))
                    {
                        column.Item().PaddingTop(5)
                            .Text(testCase.Description)
                            .FontSize(9)
                            .Italic()
                            .FontColor(Colors.Grey.Darken1);
                    }

                    // Métadonnées
                    column.Item().PaddingTop(5).Row(row =>
                    {
                        row.AutoItem().Text($"Duration: {testCase.DurationSeconds:F2}s");
                        row.AutoItem().PaddingLeft(20).Text($"Criticality: {GetCriticalityText(testCase.CriticalityLevel)}");
                        row.AutoItem().PaddingLeft(20).Text($"Started: {testCase.StartedAt:HH:mm:ss}");
                    });

                    // Message d'erreur (si échec)
                    if (testCase.Status == "Failed" && !string.IsNullOrEmpty(testCase.ErrorMessage))
                    {
                        column.Item().PaddingTop(10)
                            .Background(Colors.Red.Lighten4)
                            .Padding(10)
                            .Column(errorCol =>
                            {
                                errorCol.Item().Text("Error Message:")
                                    .FontSize(10)
                                    .Bold()
                                    .FontColor(Colors.Red.Darken2);

                                errorCol.Item().PaddingTop(5)
                                    .Text(testCase.ErrorMessage)
                                    .FontSize(9)
                                    .FontFamily("Courier New");
                            });
                    }

                    // Détails des steps (si inclus)
                    if (template.IncludeStepDetails && testCase.Steps.Count > 0)
                    {
                        column.Item().PaddingTop(10)
                            .Element(c => ComposeTestSteps(c, testCase.Steps));
                    }

                    // Screenshots (si inclus)
                    if (template.IncludeScreenshots && testCase.ScreenshotPaths.Count > 0)
                    {
                        column.Item().PaddingTop(10)
                            .Element(c => ComposeScreenshots(c, testCase.ScreenshotPaths));
                    }
                });
        }

        private void ComposeTestSteps(IContainer container, List<TestStepResultData> steps)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(40); // Order
                    columns.RelativeColumn(3); // Action
                    columns.RelativeColumn(2); // Description
                    columns.ConstantColumn(80); // Status
                    columns.ConstantColumn(70); // Duration
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("#").Bold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Action").Bold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Description").Bold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Status").Bold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Duration").Bold();
                });

                // Rows
                foreach (var step in steps)
                {
                    var bgColor = step.Status == "Failed" ? Colors.Red.Lighten4 : Colors.White;

                    table.Cell().Background(bgColor).Padding(5).Text(step.StepOrder.ToString());
                    table.Cell().Background(bgColor).Padding(5).Text(step.Action).FontSize(9);
                    table.Cell().Background(bgColor).Padding(5).Text(step.Description ?? "").FontSize(8);
                    table.Cell().Background(bgColor).Padding(5)
                        .Text(step.Status)
                        .FontColor(GetStatusTextColor(step.Status))
                        .FontSize(9);
                    table.Cell().Background(bgColor).Padding(5).Text($"{step.DurationMs:F0}ms").FontSize(9);
                }
            });
        }

        private void ComposeScreenshots(IContainer container, List<string> screenshotPaths)
        {
            container.Column(column =>
            {
                column.Item().Text("Screenshots:").FontSize(10).Bold();

                column.Item().PaddingTop(5).Row(row =>
                {
                    foreach (var screenshot in screenshotPaths.Take(3)) // Max 3 screenshots par test
                    {
                        if (File.Exists(screenshot))
                        {
                            row.RelativeItem().PaddingRight(5).Image(screenshot).FitArea();
                        }
                    }
                });
            });
        }

        private void ComposeHistoricalSection(IContainer container, TestRunReportData data)
        {
            if (data.HistoricalData == null || data.HistoricalData.Count == 0)
                return;

            container.Column(column =>
            {
                column.Item().PageBreak();

                column.Item().Text("Historical Trends")
                    .FontSize(18)
                    .Bold()
                    .FontColor("#007bff");

                column.Item().PaddingTop(10);

                var trendChartPath = GenerateTrendChart(data.HistoricalData);
                if (File.Exists(trendChartPath))
                {
                    column.Item().Height(250).Image(trendChartPath);
                }
            });
        }

        // ═══════════════════════════════════════════════════════
        // GÉNÉRATION DE GRAPHIQUES
        // ═══════════════════════════════════════════════════════

        private string GeneratePieChart(TestRunReportData data)
        {
            var plot = new ScottPlot.Plot(600, 400);

            double[] values = { data.Summary.PassedCount, data.Summary.FailedCount, data.Summary.SkippedCount };
            string[] labels = { $"Passed ({data.Summary.PassedCount})",
                              $"Failed ({data.Summary.FailedCount})",
                              $"Skipped ({data.Summary.SkippedCount})" };

            var pie = plot.AddPie(values);
            pie.SliceLabels = labels;
            pie.ShowLabels = true;
            pie.ShowPercentages = true;
            pie.Explode = true;

            // Couleurs
            pie.SliceFillColors = new[]
            {
                System.Drawing.Color.FromArgb(40, 167, 69),   // Green
                System.Drawing.Color.FromArgb(220, 53, 69),   // Red
                System.Drawing.Color.FromArgb(255, 193, 7)    // Yellow
            };

            plot.Title("Test Results Distribution", size: 20, bold: true);
            plot.Legend(false);

            var tempPath = Path.Combine(Path.GetTempPath(), $"pie_chart_{Guid.NewGuid()}.png");
            plot.SaveFig(tempPath);

            return tempPath;
        }

        private string GenerateDurationChart(TestRunReportData data)
        {
            var plot = new ScottPlot.Plot(800, 300);

            var testNames = data.TestCases.Select(tc => tc.TestCaseName).ToArray();
            var durations = data.TestCases.Select(tc => tc.DurationSeconds).ToArray();

            var bar = plot.AddBar(durations);
            bar.FillColor = System.Drawing.Color.FromArgb(0, 123, 255);

            plot.XTicks(Enumerable.Range(0, testNames.Length).Select(i => (double)i).ToArray(), testNames);
            plot.XAxis.TickLabelStyle(rotation: 45);
            plot.YLabel("Duration (seconds)");
            plot.Title("Test Execution Duration", size: 16, bold: true);

            var tempPath = Path.Combine(Path.GetTempPath(), $"duration_chart_{Guid.NewGuid()}.png");
            plot.SaveFig(tempPath);

            return tempPath;
        }

        private string GenerateTrendChart(List<HistoricalRunData> historicalData)
        {
            var plot = new ScottPlot.Plot(800, 300);

            var dates = historicalData.Select(h => h.Date.ToOADate()).ToArray();
            var successRates = historicalData.Select(h => h.SuccessRate).ToArray();

            var scatter = plot.AddScatter(dates, successRates, markerSize: 8);
            scatter.Color = System.Drawing.Color.FromArgb(40, 167, 69);
            scatter.LineWidth = 2;

            plot.XAxis.DateTimeFormat(true);
            plot.YLabel("Success Rate (%)");
            plot.YAxis.SetBoundary(0, 100);
            plot.Title("Success Rate Trend", size: 16, bold: true);

            var tempPath = Path.Combine(Path.GetTempPath(), $"trend_chart_{Guid.NewGuid()}.png");
            plot.SaveFig(tempPath);

            return tempPath;
        }

        // ═══════════════════════════════════════════════════════
        // MÉTHODES UTILITAIRES
        // ═══════════════════════════════════════════════════════

        private string FormatDuration(TimeSpan? duration)
        {
            if (!duration.HasValue)
                return "N/A";

            if (duration.Value.TotalHours >= 1)
                return $"{duration.Value.Hours}h {duration.Value.Minutes}m";
            if (duration.Value.TotalMinutes >= 1)
                return $"{duration.Value.Minutes}m {duration.Value.Seconds}s";

            return $"{duration.Value.Seconds}s";
        }

        private string GetStatusColor(double successRate)
        {
            if (successRate >= 90) return Colors.Green.Medium;
            if (successRate >= 70) return Colors.Orange.Medium;
            return Colors.Red.Medium;
        }

        private string GetStatusText(double successRate)
        {
            if (successRate >= 95) return "✓ EXCELLENT";
            if (successRate >= 90) return "✓ GOOD";
            if (successRate >= 70) return "⚠ WARNING";
            return "✗ CRITICAL";
        }

        private string GetStatusTextColor(string status)
        {
            return status switch
            {
                "Passed" => Colors.Green.Darken1,
                "Failed" => Colors.Red.Darken1,
                "Skipped" => Colors.Orange.Darken1,
                _ => Colors.Grey.Darken1
            };
        }

        private string GetCriticalityText(int level)
        {
            return level switch
            {
                5 => "Critical",
                4 => "High",
                3 => "Medium",
                2 => "Low",
                1 => "Minimal",
                _ => "Unknown"
            };
        }
    }
}
