using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TesterLab.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class sqLiteMigration1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActionTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    Icon = table.Column<string>(type: "TEXT", nullable: false),
                    Example = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    MainUrl = table.Column<string>(type: "TEXT", nullable: true),
                    AppType = table.Column<string>(type: "TEXT", nullable: false),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false),
                    Selected = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Environments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApplicationId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    BaseUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    RequiresAuth = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessInfo = table.Column<string>(type: "TEXT", nullable: true),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Environments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Environments_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Features",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApplicationId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Icon = table.Column<string>(type: "TEXT", nullable: true),
                    BusinessPriority = table.Column<int>(type: "INTEGER", nullable: false),
                    Complexity = table.Column<string>(type: "TEXT", nullable: false),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Features_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestDataSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApplicationId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    DataType = table.Column<string>(type: "TEXT", nullable: false),
                    DataJson = table.Column<string>(type: "TEXT", nullable: false),
                    IsTemplate = table.Column<bool>(type: "INTEGER", nullable: false),
                    SpecificEnvironmentId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestDataSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestDataSets_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestDataSets_Environments_SpecificEnvironmentId",
                        column: x => x.SpecificEnvironmentId,
                        principalTable: "Environments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TestCases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    FeatureId = table.Column<int>(type: "INTEGER", nullable: false),
                    CriticalityLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    ExecutionFrequency = table.Column<string>(type: "TEXT", nullable: false),
                    Tags = table.Column<string>(type: "TEXT", nullable: true),
                    EstimatedMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    UserPersona = table.Column<string>(type: "TEXT", nullable: true),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false),
                    Selected = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestCases_Features_FeatureId",
                        column: x => x.FeatureId,
                        principalTable: "Features",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestRuns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApplicationId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Trigger = table.Column<string>(type: "TEXT", nullable: false),
                    ExecutionType = table.Column<string>(type: "TEXT", nullable: false),
                    TargetIds = table.Column<string>(type: "TEXT", nullable: false),
                    EnvironmentId = table.Column<int>(type: "INTEGER", nullable: false),
                    TestDataId = table.Column<int>(type: "INTEGER", nullable: true),
                    Browser = table.Column<string>(type: "TEXT", nullable: false),
                    Headless = table.Column<bool>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    ProgressPercentage = table.Column<int>(type: "INTEGER", nullable: false),
                    PassedCount = table.Column<int>(type: "INTEGER", nullable: false),
                    FailedCount = table.Column<int>(type: "INTEGER", nullable: false),
                    SkippedCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DetailedResults = table.Column<string>(type: "TEXT", nullable: true),
                    ExecutionLogs = table.Column<string>(type: "TEXT", nullable: true),
                    Screenshots = table.Column<string>(type: "TEXT", nullable: true),
                    ReportPath = table.Column<string>(type: "TEXT", nullable: true),
                    AverageDurationMs = table.Column<double>(type: "REAL", nullable: false),
                    SuccessRate = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestRuns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestRuns_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestRuns_Environments_EnvironmentId",
                        column: x => x.EnvironmentId,
                        principalTable: "Environments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestRuns_TestDataSets_TestDataId",
                        column: x => x.TestDataId,
                        principalTable: "TestDataSets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TestSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TestCaseId = table.Column<int>(type: "INTEGER", nullable: false),
                    Action = table.Column<string>(type: "TEXT", nullable: false),
                    Target = table.Column<string>(type: "TEXT", nullable: true),
                    Selector = table.Column<string>(type: "TEXT", nullable: true),
                    Value = table.Column<string>(type: "TEXT", nullable: true),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    IsOptional = table.Column<bool>(type: "INTEGER", nullable: false),
                    TimeoutSeconds = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestSteps_TestCases_TestCaseId",
                        column: x => x.TestCaseId,
                        principalTable: "TestCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestCaseExecutions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TestRunId = table.Column<int>(type: "INTEGER", nullable: false),
                    TestCaseId = table.Column<int>(type: "INTEGER", nullable: false),
                    TestCaseName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DurationMs = table.Column<int>(type: "INTEGER", nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: false),
                    ErrorStackTrace = table.Column<string>(type: "TEXT", nullable: false),
                    TotalSteps = table.Column<int>(type: "INTEGER", nullable: false),
                    PassedSteps = table.Column<int>(type: "INTEGER", nullable: false),
                    FailedSteps = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCaseExecutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestCaseExecutions_TestCases_TestCaseId",
                        column: x => x.TestCaseId,
                        principalTable: "TestCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestCaseExecutions_TestRuns_TestRunId",
                        column: x => x.TestRunId,
                        principalTable: "TestRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExecutionLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TestRunId = table.Column<int>(type: "INTEGER", nullable: false),
                    TestCaseExecutionId = table.Column<int>(type: "INTEGER", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Level = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    Details = table.Column<string>(type: "TEXT", nullable: false),
                    StackTrace = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExecutionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExecutionLogs_TestCaseExecutions_TestCaseExecutionId",
                        column: x => x.TestCaseExecutionId,
                        principalTable: "TestCaseExecutions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExecutionLogs_TestRuns_TestRunId",
                        column: x => x.TestRunId,
                        principalTable: "TestRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PerformanceMetrics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TestRunId = table.Column<int>(type: "INTEGER", nullable: false),
                    TestCaseExecutionId = table.Column<int>(type: "INTEGER", nullable: true),
                    MetricName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Value = table.Column<double>(type: "REAL", nullable: false),
                    Unit = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Context = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerformanceMetrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerformanceMetrics_TestCaseExecutions_TestCaseExecutionId",
                        column: x => x.TestCaseExecutionId,
                        principalTable: "TestCaseExecutions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PerformanceMetrics_TestRuns_TestRunId",
                        column: x => x.TestRunId,
                        principalTable: "TestRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestStepExecutions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TestCaseExecutionId = table.Column<int>(type: "INTEGER", nullable: false),
                    TestStepId = table.Column<int>(type: "INTEGER", nullable: false),
                    StepOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    Action = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Selector = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DurationMs = table.Column<int>(type: "INTEGER", nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: false),
                    ScreenshotPath = table.Column<string>(type: "TEXT", nullable: false),
                    IsOptional = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestStepExecutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestStepExecutions_TestCaseExecutions_TestCaseExecutionId",
                        column: x => x.TestCaseExecutionId,
                        principalTable: "TestCaseExecutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestStepExecutions_TestSteps_TestStepId",
                        column: x => x.TestStepId,
                        principalTable: "TestSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Screenshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TestRunId = table.Column<int>(type: "INTEGER", nullable: false),
                    TestCaseExecutionId = table.Column<int>(type: "INTEGER", nullable: true),
                    TestStepExecutionId = table.Column<int>(type: "INTEGER", nullable: true),
                    FilePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CapturedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FileSizeBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Screenshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Screenshots_TestCaseExecutions_TestCaseExecutionId",
                        column: x => x.TestCaseExecutionId,
                        principalTable: "TestCaseExecutions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Screenshots_TestRuns_TestRunId",
                        column: x => x.TestRunId,
                        principalTable: "TestRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Screenshots_TestStepExecutions_TestStepExecutionId",
                        column: x => x.TestStepExecutionId,
                        principalTable: "TestStepExecutions",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "ActionTemplates",
                columns: new[] { "Id", "Category", "Description", "Example", "Icon", "Name" },
                values: new object[,]
                {
                    { 1, "Navigation", "Naviguer vers une page", "Aller à /login", "🏠", "Aller à la page" },
                    { 2, "Interaction", "Cliquer sur un élément", "Cliquer sur le bouton Se connecter", "👆", "Cliquer sur" },
                    { 3, "Form", "Remplir un champ", "Saisir l'email dans le champ Email", "⌨️", "Saisir du texte" },
                    { 4, "Validation", "Valider un résultat", "Vérifier que le message Bienvenue s'affiche", "✅", "Vérifier que" },
                    { 5, "Wait", "Attendre un délai", "Attendre 3 secondes", "⏱️", "Attendre" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Environment_ApplicationId_Active",
                table: "Environments",
                columns: new[] { "ApplicationId", "Active" });

            migrationBuilder.CreateIndex(
                name: "IX_ExecutionLogs_TestCaseExecutionId",
                table: "ExecutionLogs",
                column: "TestCaseExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExecutionLogs_TestRunId",
                table: "ExecutionLogs",
                column: "TestRunId");

            migrationBuilder.CreateIndex(
                name: "IX_Features_ApplicationId",
                table: "Features",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceMetrics_TestCaseExecutionId",
                table: "PerformanceMetrics",
                column: "TestCaseExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceMetrics_TestRunId",
                table: "PerformanceMetrics",
                column: "TestRunId");

            migrationBuilder.CreateIndex(
                name: "IX_Screenshots_TestCaseExecutionId",
                table: "Screenshots",
                column: "TestCaseExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Screenshots_TestRunId",
                table: "Screenshots",
                column: "TestRunId");

            migrationBuilder.CreateIndex(
                name: "IX_Screenshots_TestStepExecutionId",
                table: "Screenshots",
                column: "TestStepExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseExecutions_TestCaseId",
                table: "TestCaseExecutions",
                column: "TestCaseId");

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseExecutions_TestRunId",
                table: "TestCaseExecutions",
                column: "TestRunId");

            migrationBuilder.CreateIndex(
                name: "IX_TestCases_FeatureId_Active",
                table: "TestCases",
                columns: new[] { "FeatureId", "Active" });

            migrationBuilder.CreateIndex(
                name: "IX_TestDataSets_ApplicationId",
                table: "TestDataSets",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_TestDataSets_SpecificEnvironmentId",
                table: "TestDataSets",
                column: "SpecificEnvironmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRuns_ApplicationId_Status",
                table: "TestRuns",
                columns: new[] { "ApplicationId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TestRuns_EnvironmentId",
                table: "TestRuns",
                column: "EnvironmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRuns_TestDataId",
                table: "TestRuns",
                column: "TestDataId");

            migrationBuilder.CreateIndex(
                name: "IX_TestStepExecutions_TestCaseExecutionId",
                table: "TestStepExecutions",
                column: "TestCaseExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_TestStepExecutions_TestStepId",
                table: "TestStepExecutions",
                column: "TestStepId");

            migrationBuilder.CreateIndex(
                name: "IX_TestSteps_TestCaseId",
                table: "TestSteps",
                column: "TestCaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionTemplates");

            migrationBuilder.DropTable(
                name: "ExecutionLogs");

            migrationBuilder.DropTable(
                name: "PerformanceMetrics");

            migrationBuilder.DropTable(
                name: "Screenshots");

            migrationBuilder.DropTable(
                name: "TestStepExecutions");

            migrationBuilder.DropTable(
                name: "TestCaseExecutions");

            migrationBuilder.DropTable(
                name: "TestSteps");

            migrationBuilder.DropTable(
                name: "TestRuns");

            migrationBuilder.DropTable(
                name: "TestCases");

            migrationBuilder.DropTable(
                name: "TestDataSets");

            migrationBuilder.DropTable(
                name: "Features");

            migrationBuilder.DropTable(
                name: "Environments");

            migrationBuilder.DropTable(
                name: "Applications");
        }
    }
}
