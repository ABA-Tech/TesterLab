using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TesterLab.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActionTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    Icon = table.Column<string>(type: "text", nullable: false),
                    Example = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    MainUrl = table.Column<string>(type: "text", nullable: true),
                    AppType = table.Column<string>(type: "text", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    Selected = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    NextExecutionTimeUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FrequencyInMinutes = table.Column<int>(type: "integer", nullable: true),
                    IsRunning = table.Column<bool>(type: "boolean", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LastExecutionTimeUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastExecutionStatus = table.Column<string>(type: "text", nullable: true),
                    ConsecutiveFailures = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TestCaseId = table.Column<int>(type: "integer", nullable: false),
                    EnvironmentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DataType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsEncrypted = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Environments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApplicationId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    BaseUrl = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    RequiresAuth = table.Column<bool>(type: "boolean", nullable: false),
                    AccessInfo = table.Column<string>(type: "text", nullable: true),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApplicationId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Icon = table.Column<string>(type: "text", nullable: true),
                    BusinessPriority = table.Column<int>(type: "integer", nullable: false),
                    Complexity = table.Column<string>(type: "text", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApplicationId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DataType = table.Column<string>(type: "text", nullable: false),
                    DataJson = table.Column<string>(type: "text", nullable: false),
                    IsTemplate = table.Column<bool>(type: "boolean", nullable: false),
                    SpecificEnvironmentId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    FeatureId = table.Column<int>(type: "integer", nullable: false),
                    CriticalityLevel = table.Column<int>(type: "integer", nullable: false),
                    ExecutionFrequency = table.Column<string>(type: "text", nullable: false),
                    Tags = table.Column<string>(type: "text", nullable: true),
                    EstimatedMinutes = table.Column<int>(type: "integer", nullable: false),
                    UserPersona = table.Column<string>(type: "text", nullable: true),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    Selected = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApplicationId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Trigger = table.Column<string>(type: "text", nullable: false),
                    ExecutionType = table.Column<string>(type: "text", nullable: false),
                    TargetIds = table.Column<string>(type: "text", nullable: false),
                    EnvironmentId = table.Column<int>(type: "integer", nullable: false),
                    TestDataId = table.Column<int>(type: "integer", nullable: true),
                    Browser = table.Column<string>(type: "text", nullable: false),
                    Headless = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    ProgressPercentage = table.Column<int>(type: "integer", nullable: false),
                    PassedCount = table.Column<int>(type: "integer", nullable: false),
                    FailedCount = table.Column<int>(type: "integer", nullable: false),
                    SkippedCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DetailedResults = table.Column<string>(type: "text", nullable: true),
                    ExecutionLogs = table.Column<string>(type: "text", nullable: true),
                    Screenshots = table.Column<string>(type: "text", nullable: true),
                    ReportPath = table.Column<string>(type: "text", nullable: true),
                    AverageDurationMs = table.Column<double>(type: "double precision", nullable: false),
                    SuccessRate = table.Column<double>(type: "double precision", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestCaseId = table.Column<int>(type: "integer", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    Target = table.Column<string>(type: "text", nullable: true),
                    Selector = table.Column<string>(type: "text", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsOptional = table.Column<bool>(type: "boolean", nullable: false),
                    TimeoutSeconds = table.Column<int>(type: "integer", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestRunId = table.Column<int>(type: "integer", nullable: false),
                    TestCaseId = table.Column<int>(type: "integer", nullable: false),
                    TestCaseName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DurationMs = table.Column<int>(type: "integer", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: false),
                    ErrorStackTrace = table.Column<string>(type: "text", nullable: false),
                    TotalSteps = table.Column<int>(type: "integer", nullable: false),
                    PassedSteps = table.Column<int>(type: "integer", nullable: false),
                    FailedSteps = table.Column<int>(type: "integer", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestRunId = table.Column<int>(type: "integer", nullable: false),
                    TestCaseExecutionId = table.Column<int>(type: "integer", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Level = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    Details = table.Column<string>(type: "text", nullable: false),
                    StackTrace = table.Column<string>(type: "text", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestRunId = table.Column<int>(type: "integer", nullable: false),
                    TestCaseExecutionId = table.Column<int>(type: "integer", nullable: true),
                    MetricName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: false),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Context = table.Column<string>(type: "text", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestCaseExecutionId = table.Column<int>(type: "integer", nullable: false),
                    TestStepId = table.Column<int>(type: "integer", nullable: false),
                    StepOrder = table.Column<int>(type: "integer", nullable: false),
                    Action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Selector = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DurationMs = table.Column<int>(type: "integer", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: false),
                    ScreenshotPath = table.Column<string>(type: "text", nullable: false),
                    IsOptional = table.Column<bool>(type: "boolean", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestRunId = table.Column<int>(type: "integer", nullable: false),
                    TestCaseExecutionId = table.Column<int>(type: "integer", nullable: true),
                    TestStepExecutionId = table.Column<int>(type: "integer", nullable: true),
                    FilePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CapturedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
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

            migrationBuilder.InsertData(
                table: "SystemSettings",
                columns: new[] { "Id", "Category", "DataType", "Description", "IsEncrypted", "Key", "UpdatedAt", "UpdatedBy", "Value" },
                values: new object[,]
                {
                    { 1, "General", "String", "Nom de l'application", false, "General.ApplicationName", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2340), null, "TesterLab" },
                    { 2, "General", "String", null, false, "General.BaseUrl", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2340), null, "http://localhost" },
                    { 3, "General", "String", null, false, "General.TimeZone", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2350), null, "UTC" },
                    { 4, "General", "String", null, false, "General.DefaultLanguage", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2350), null, "fr-FR" },
                    { 5, "General", "Boolean", null, false, "General.MaintenanceMode", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2350), null, "false" },
                    { 6, "General", "Boolean", null, false, "General.AllowRegistration", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2350), null, "true" },
                    { 7, "General", "Boolean", null, false, "General.RequireEmailConfirmation", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2350), null, "true" },
                    { 10, "Email", "String", null, false, "Email.SmtpHost", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2350), null, "smtp.gmail.com" },
                    { 11, "Email", "Integer", null, false, "Email.SmtpPort", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2360), null, "587" },
                    { 12, "Email", "Boolean", null, false, "Email.EnableSsl", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2360), null, "true" },
                    { 13, "Email", "String", null, false, "Email.FromEmail", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2360), null, "noreply@testerlab.com" },
                    { 14, "Email", "String", null, false, "Email.FromName", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2360), null, "TesterLab" },
                    { 15, "Email", "Boolean", null, false, "Email.EmailEnabled", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2360), null, "true" },
                    { 20, "Testing", "Integer", null, false, "Testing.DefaultTimeoutSeconds", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2360), null, "30" },
                    { 21, "Testing", "Integer", null, false, "Testing.MaxRetries", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2360), null, "3" },
                    { 22, "Testing", "String", null, false, "Testing.DefaultBrowser", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2360), null, "Chrome" },
                    { 23, "Testing", "Boolean", null, false, "Testing.DefaultHeadlessMode", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2370), null, "true" },
                    { 24, "Testing", "Boolean", null, false, "Testing.CaptureScreenshotOnFailure", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2370), null, "true" },
                    { 25, "Testing", "Boolean", null, false, "Testing.CaptureScreenshotOnSuccess", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2370), null, "false" },
                    { 26, "Testing", "Integer", null, false, "Testing.RetentionDays", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2370), null, "30" },
                    { 27, "Testing", "Integer", null, false, "Testing.MaxParallelExecutions", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2370), null, "5" },
                    { 28, "Testing", "Boolean", null, false, "Testing.AutoRetryOnFailure", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2370), null, "true" },
                    { 30, "Security", "Integer", null, false, "Security.MinPasswordLength", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2370), null, "12" },
                    { 31, "Security", "Boolean", null, false, "Security.RequireUppercase", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2380), null, "true" },
                    { 32, "Security", "Boolean", null, false, "Security.RequireLowercase", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2380), null, "true" },
                    { 33, "Security", "Boolean", null, false, "Security.RequireDigit", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2380), null, "true" },
                    { 34, "Security", "Boolean", null, false, "Security.RequireNonAlphanumeric", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2380), null, "true" },
                    { 35, "Security", "Integer", null, false, "Security.MaxFailedLoginAttempts", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2380), null, "5" },
                    { 36, "Security", "Integer", null, false, "Security.LockoutDurationMinutes", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2380), null, "15" },
                    { 37, "Security", "Integer", null, false, "Security.SessionTimeoutMinutes", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2380), null, "720" },
                    { 40, "Branding", "String", null, false, "Branding.PrimaryColor", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2380), null, "#007bff" },
                    { 41, "Branding", "String", null, false, "Branding.SecondaryColor", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2380), null, "#6c757d" },
                    { 42, "Branding", "Boolean", null, false, "Branding.ShowCompanyName", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2390), null, "true" },
                    { 50, "Notifications", "Boolean", null, false, "Notifications.NotifyOnTestFailure", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2390), null, "true" },
                    { 51, "Notifications", "Boolean", null, false, "Notifications.NotifyOnTestSuccess", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2390), null, "false" },
                    { 52, "Notifications", "Boolean", null, false, "Notifications.NotifyOnScheduledRuns", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2390), null, "true" },
                    { 53, "Notifications", "Integer", null, false, "Notifications.MinSuccessRateForNotification", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2390), null, "90" },
                    { 60, "Storage", "String", null, false, "Storage.ScreenshotPath", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2390), null, "wwwroot/screenshots" },
                    { 61, "Storage", "String", null, false, "Storage.ReportsPath", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2390), null, "wwwroot/reports" },
                    { 62, "Storage", "Integer", null, false, "Storage.MaxFileSizeMB", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2390), null, "10" },
                    { 63, "Storage", "Boolean", null, false, "Storage.AutoCleanupEnabled", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2400), null, "true" },
                    { 64, "Storage", "Integer", null, false, "Storage.FileRetentionDays", new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2400), null, "30" }
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
                name: "IX_SystemSettings_Category",
                table: "SystemSettings",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_Key",
                table: "SystemSettings",
                column: "Key",
                unique: true);

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
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "PerformanceMetrics");

            migrationBuilder.DropTable(
                name: "Screenshots");

            migrationBuilder.DropTable(
                name: "SystemSettings");

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
