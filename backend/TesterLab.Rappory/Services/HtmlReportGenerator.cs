using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesterLab.Rappory.Models;

namespace TesterLab.Rappory.Services
{
    public interface IHtmlReportGenerator
    {
        Task<string> GenerateAsync(TestRunReportData data, ReportTemplate template);
    }

    public class HtmlReportGenerator : IHtmlReportGenerator
    {
        public async Task<string> GenerateAsync(TestRunReportData data, ReportTemplate template)
        {
            return await Task.Run(() =>
            {
                var html = new StringBuilder();

                // HTML Head
                html.AppendLine("<!DOCTYPE html>");
                html.AppendLine("<html lang='fr'>");
                html.AppendLine("<head>");
                html.AppendLine("<meta charset='UTF-8'>");
                html.AppendLine("<meta name='viewport' content='width=device-width, initial-scale=1.0'>");
                html.AppendLine($"<title>Test Report - {data.TestRunName}</title>");
                html.AppendLine("<link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css' rel='stylesheet'>");
                html.AppendLine("<link rel='stylesheet' href='https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css'>");
                html.AppendLine("<style>");
                html.AppendLine(GetCustomCss(template));
                html.AppendLine("</style>");
                html.AppendLine("</head>");

                html.AppendLine("<body>");
                html.AppendLine("<div class='container-fluid py-4'>");

                // Header
                html.AppendLine(GenerateHeader(data, template));

                // Summary Section
                if (template.IncludeSummary)
                {
                    html.AppendLine(GenerateSummarySection(data));
                }

                // Charts Section
                if (template.IncludeCharts)
                {
                    html.AppendLine(GenerateChartsSection(data));
                }

                // Test Results
                html.AppendLine(GenerateTestResultsSection(data, template));

                // Footer
                html.AppendLine(GenerateFooter());

                html.AppendLine("</div>"); // container
                html.AppendLine("<script src='https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js'></script>");
                html.AppendLine("<script src='https://cdn.jsdelivr.net/npm/chart.js'></script>");
                html.AppendLine("<script>");
                html.AppendLine(GenerateChartScripts(data));
                html.AppendLine("</script>");
                html.AppendLine("</body>");
                html.AppendLine("</html>");

                return html.ToString();
            });
        }

        private string GetCustomCss(ReportTemplate template)
        {
            return $@"
                :root {{
                    --primary-color: {template.PrimaryColor};
                }}
                body {{
                    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                    background-color: #f8f9fa;
                }}
                .report-header {{
                    background: linear-gradient(135deg, var(--primary-color) 0%, {AdjustColor(template.PrimaryColor, -20)} 100%);
                    color: white;
                    padding: 40px;
                    border-radius: 10px;
                    margin-bottom: 30px;
                    box-shadow: 0 4px 15px rgba(0,0,0,0.1);
                }}
                .stat-card {{
                    background: white;
                    border-radius: 10px;
                    padding: 20px;
                    box-shadow: 0 2px 10px rgba(0,0,0,0.08);
                    margin-bottom: 20px;
                    transition: transform 0.2s;
                }}
                .stat-card:hover {{
                    transform: translateY(-5px);
                }}
                .test-case-card {{
                    background: white;
                    border-left: 4px solid var(--primary-color);
                    padding: 20px;
                    margin-bottom: 15px;
                    border-radius: 5px;
                    box-shadow: 0 2px 8px rgba(0,0,0,0.05);
                }}
                .status-passed {{ color: #28a745; }}
                .status-failed {{ color: #dc3545; }}
                .status-skipped {{ color: #ffc107; }}
                .badge-passed {{ background-color: #28a745; }}
                .badge-failed {{ background-color: #dc3545; }}
                .badge-skipped {{ background-color: #ffc107; }}
                .chart-container {{
                    background: white;
                    padding: 30px;
                    border-radius: 10px;
                    box-shadow: 0 2px 10px rgba(0,0,0,0.08);
                    margin-bottom: 30px;
                }}
                .step-table {{
                    font-size: 14px;
                }}
                @media print {{
                    .no-print {{ display: none; }}
                    body {{ background-color: white; }}
                }}
            ";
        }

        private string GenerateHeader(TestRunReportData data, ReportTemplate template)
        {
            var statusClass = data.Summary.SuccessRate >= 90 ? "success" : data.Summary.SuccessRate >= 70 ? "warning" : "danger";
            var statusIcon = data.Summary.SuccessRate >= 90 ? "fa-check-circle" : data.Summary.SuccessRate >= 70 ? "fa-exclamation-triangle" : "fa-times-circle";

            return $@"
                <div class='report-header'>
                    <div class='row align-items-center'>
                        <div class='col-md-8'>
                            <h1 class='mb-2'>
                                <i class='fas fa-file-alt'></i> Test Execution Report
                            </h1>
                            <h3>{data.TestRunName}</h3>
                            <p class='mb-1'>
                                <i class='fas fa-laptop-code'></i> {data.ApplicationName} 
                                <span class='ms-3'><i class='fas fa-server'></i> {data.EnvironmentName}</span>
                            </p>
                            <p class='mb-0'>
                                <i class='fas fa-calendar'></i> {data.ExecutionDate:yyyy-MM-dd HH:mm} 
                                <span class='ms-3'><i class='fas fa-clock'></i> Duration: {FormatDuration(data.Duration)}</span>
                            </p>
                        </div>
                        <div class='col-md-4 text-center'>
                            <div class='bg-white text-dark rounded p-4'>
                                <i class='fas {statusIcon} fa-3x text-{statusClass} mb-2'></i>
                                <h2 class='mb-0'>{data.Summary.SuccessRate:F1}%</h2>
                                <p class='mb-0'>Success Rate</p>
                            </div>
                        </div>
                    </div>
                </div>
            ";
        }

        private string GenerateSummarySection(TestRunReportData data)
        {
            return $@"
                <div class='row mb-4'>
                    <div class='col-md-3'>
                        <div class='stat-card text-center'>
                            <i class='fas fa-list fa-2x text-primary mb-2'></i>
                            <h3 class='mb-0'>{data.Summary.TotalTests}</h3>
                            <p class='text-muted mb-0'>Total Tests</p>
                        </div>
                    </div>
                    <div class='col-md-3'>
                        <div class='stat-card text-center'>
                            <i class='fas fa-check-circle fa-2x text-success mb-2'></i>
                            <h3 class='mb-0 status-passed'>{data.Summary.PassedCount}</h3>
                            <p class='text-muted mb-0'>Passed</p>
                        </div>
                    </div>
                    <div class='col-md-3'>
                        <div class='stat-card text-center'>
                            <i class='fas fa-times-circle fa-2x text-danger mb-2'></i>
                            <h3 class='mb-0 status-failed'>{data.Summary.FailedCount}</h3>
                            <p class='text-muted mb-0'>Failed</p>
                        </div>
                    </div>
                    <div class='col-md-3'>
                        <div class='stat-card text-center'>
                            <i class='fas fa-minus-circle fa-2x text-warning mb-2'></i>
                            <h3 class='mb-0 status-skipped'>{data.Summary.SkippedCount}</h3>
                            <p class='text-muted mb-0'>Skipped</p>
                        </div>
                    </div>
                </div>

                <div class='row mb-4'>
                    <div class='col-md-12'>
                        <div class='stat-card'>
                            <h5><i class='fas fa-info-circle'></i> Additional Metrics</h5>
                            <div class='row mt-3'>
                                <div class='col-md-4'>
                                    <p><strong>Total Steps:</strong> {data.Summary.TotalSteps}</p>
                                </div>
                                <div class='col-md-4'>
                                    <p><strong>Critical Failures:</strong> {data.Summary.CriticalTestsFailed}</p>
                                </div>
                                <div class='col-md-4'>
                                    <p><strong>Avg Duration:</strong> {data.Summary.AverageDurationSeconds:F2}s</p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            ";
        }

        private string GenerateChartsSection(TestRunReportData data)
        {
            return $@"
                <div class='row mb-4'>
                    <div class='col-md-6'>
                        <div class='chart-container'>
                            <h5 class='mb-3'><i class='fas fa-chart-pie'></i> Test Results Distribution</h5>
                            <canvas id='pieChart' height='250'></canvas>
                        </div>
                    </div>
                    <div class='col-md-6'>
                        <div class='chart-container'>
                            <h5 class='mb-3'><i class='fas fa-chart-bar'></i> Test Duration</h5>
                            <canvas id='durationChart' height='250'></canvas>
                        </div>
                    </div>
                </div>
            ";
        }

        private string GenerateTestResultsSection(TestRunReportData data, ReportTemplate template)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<div class='row'>");
            sb.AppendLine("<div class='col-12'>");
            sb.AppendLine("<h4 class='mb-4'><i class='fas fa-list-check'></i> Test Results Details</h4>");

            var testsToShow = template.IncludeFailedTestsOnly
                ? data.TestCases.Where(tc => tc.Status == "Failed").ToList()
                : data.TestCases;

            foreach (var testCase in testsToShow)
            {
                sb.AppendLine(GenerateTestCaseCard(testCase, template));
            }

            sb.AppendLine("</div>");
            sb.AppendLine("</div>");

            return sb.ToString();
        }

        private string GenerateTestCaseCard(TestCaseResultData testCase, ReportTemplate template)
        {
            var statusClass = testCase.Status == "Passed" ? "success" : testCase.Status == "Failed" ? "danger" : "warning";
            var statusIcon = testCase.Status == "Passed" ? "fa-check-circle" : testCase.Status == "Failed" ? "fa-times-circle" : "fa-minus-circle";

            var sb = new StringBuilder();

            sb.AppendLine("<div class='test-case-card'>");
            sb.AppendLine("<div class='row'>");
            sb.AppendLine("<div class='col-md-10'>");
            sb.AppendLine($"<h5><i class='fas {statusIcon} status-{testCase.Status.ToLower()}'></i> {testCase.TestCaseName}</h5>");

            if (!string.IsNullOrEmpty(testCase.Description))
            {
                sb.AppendLine($"<p class='text-muted'>{testCase.Description}</p>");
            }

            sb.AppendLine($"<p class='mb-1'>");
            sb.AppendLine($"<span class='badge bg-secondary me-2'>Duration: {testCase.DurationSeconds:F2}s</span>");
            sb.AppendLine($"<span class='badge bg-info me-2'>Criticality: {GetCriticalityText(testCase.CriticalityLevel)}</span>");
            sb.AppendLine($"<span class='badge bg-light text-dark'>{testCase.StartedAt:HH:mm:ss}</span>");
            sb.AppendLine("</p>");

            sb.AppendLine("</div>");
            sb.AppendLine("<div class='col-md-2 text-end'>");
            sb.AppendLine($"<span class='badge badge-{testCase.Status.ToLower()} fs-6'>{testCase.Status}</span>");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");

            // Error message
            if (testCase.Status == "Failed" && !string.IsNullOrEmpty(testCase.ErrorMessage))
            {
                sb.AppendLine("<div class='alert alert-danger mt-3'>");
                sb.AppendLine("<strong><i class='fas fa-exclamation-triangle'></i> Error:</strong>");
                sb.AppendLine($"<pre class='mb-0 mt-2'>{System.Web.HttpUtility.HtmlEncode(testCase.ErrorMessage)}</pre>");
                sb.AppendLine("</div>");
            }

            // Steps details
            if (template.IncludeStepDetails && testCase.Steps.Count > 0)
            {
                sb.AppendLine("<div class='mt-3'>");
                sb.AppendLine("<h6><i class='fas fa-tasks'></i> Steps:</h6>");
                sb.AppendLine("<table class='table table-sm step-table'>");
                sb.AppendLine("<thead><tr><th>#</th><th>Action</th><th>Status</th><th>Duration</th></tr></thead>");
                sb.AppendLine("<tbody>");

                foreach (var step in testCase.Steps)
                {
                    var stepStatusClass = step.Status == "Passed" ? "success" : step.Status == "Failed" ? "danger" : "warning";
                    sb.AppendLine($"<tr class='table-{(step.Status == "Failed" ? "danger" : "")}'>");
                    sb.AppendLine($"<td>{step.StepOrder}</td>");
                    sb.AppendLine($"<td>{step.Action} - {step.Description}</td>");
                    sb.AppendLine($"<td><span class='badge badge-{step.Status.ToLower()}'>{step.Status}</span></td>");
                    sb.AppendLine($"<td>{step.DurationMs:F0}ms</td>");
                    sb.AppendLine("</tr>");
                }

                sb.AppendLine("</tbody>");
                sb.AppendLine("</table>");
                sb.AppendLine("</div>");
            }

            // Screenshots
            if (template.IncludeScreenshots && testCase.ScreenshotPaths.Count > 0)
            {
                sb.AppendLine("<div class='mt-3'>");
                sb.AppendLine("<h6><i class='fas fa-camera'></i> Screenshots:</h6>");
                sb.AppendLine("<div class='row'>");

                foreach (var screenshot in testCase.ScreenshotPaths.Take(3))
                {
                    if (File.Exists(screenshot))
                    {
                        sb.AppendLine("<div class='col-md-4'>");
                        sb.AppendLine($"<img src='{screenshot}' class='img-fluid rounded' />");
                        sb.AppendLine("</div>");
                    }
                }

                sb.AppendLine("</div>");
                sb.AppendLine("</div>");
            }

            sb.AppendLine("</div>");

            return sb.ToString();
        }

        private string GenerateFooter()
        {
            return $@"
                <div class='text-center mt-5 pt-4 border-top'>
                    <p class='text-muted'>
                        Generated by <strong>TesterLab</strong> on {DateTime.Now:yyyy-MM-dd HH:mm:ss}
                    </p>
                    <button class='btn btn-primary no-print' onclick='window.print()'>
                        <i class='fas fa-print'></i> Print Report
                    </button>
                </div>
            ";
        }

        private string GenerateChartScripts(TestRunReportData data)
        {
            return $@"
                // Pie Chart
                const pieCtx = document.getElementById('pieChart').getContext('2d');
                new Chart(pieCtx, {{
                    type: 'pie',
                    data: {{
                        labels: ['Passed', 'Failed', 'Skipped'],
                        datasets: [{{
                            data: [{data.Summary.PassedCount}, {data.Summary.FailedCount}, {data.Summary.SkippedCount}],
                            backgroundColor: ['#28a745', '#dc3545', '#ffc107']
                        }}]
                    }},
                    options: {{
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: {{
                            legend: {{ position: 'bottom' }}
                        }}
                    }}
                }});

                // Duration Chart
                const durationCtx = document.getElementById('durationChart').getContext('2d');
                new Chart(durationCtx, {{
                    type: 'bar',
                    data: {{
                        labels: {System.Text.Json.JsonSerializer.Serialize(data.TestCases.Take(10).Select(tc => tc.TestCaseName).ToList())},
                        datasets: [{{
                            label: 'Duration (seconds)',
                            data: {System.Text.Json.JsonSerializer.Serialize(data.TestCases.Take(10).Select(tc => tc.DurationSeconds).ToList())},
                            backgroundColor: '#007bff'
                        }}]
                    }},
                    options: {{
                        responsive: true,
                        maintainAspectRatio: false,
                        scales: {{
                            y: {{ beginAtZero: true }}
                        }}
                    }}
                }});
            ";
        }

        private string FormatDuration(TimeSpan? duration)
        {
            if (!duration.HasValue) return "N/A";
            if (duration.Value.TotalHours >= 1)
                return $"{duration.Value.Hours}h {duration.Value.Minutes}m";
            if (duration.Value.TotalMinutes >= 1)
                return $"{duration.Value.Minutes}m {duration.Value.Seconds}s";
            return $"{duration.Value.Seconds}s";
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

        private string AdjustColor(string color, int adjustment)
        {
            // Simple color adjustment (darken/lighten)
            return color; // Simplification pour l'exemple
        }
    }
}
