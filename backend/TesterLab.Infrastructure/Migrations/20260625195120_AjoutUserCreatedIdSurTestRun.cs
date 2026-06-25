using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TesterLab.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AjoutUserCreatedIdSurTestRun : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "TestSteps",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TimeoutSeconds",
                table: "TestSteps",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "TestCaseId",
                table: "TestSteps",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Target",
                table: "TestSteps",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Selector",
                table: "TestSteps",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Order",
                table: "TestSteps",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<bool>(
                name: "IsOptional",
                table: "TestSteps",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TestSteps",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Action",
                table: "TestSteps",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "TestSteps",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Sqlite:Autoincrement", true)
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "TestStepExecutions",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "TestStepId",
                table: "TestStepExecutions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "TestCaseExecutionId",
                table: "TestStepExecutions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "StepOrder",
                table: "TestStepExecutions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "TestStepExecutions",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartedAt",
                table: "TestStepExecutions",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Selector",
                table: "TestStepExecutions",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ScreenshotPath",
                table: "TestStepExecutions",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsOptional",
                table: "TestStepExecutions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "ErrorMessage",
                table: "TestStepExecutions",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "DurationMs",
                table: "TestStepExecutions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TestStepExecutions",
                type: "TEXT",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CompletedAt",
                table: "TestStepExecutions",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Action",
                table: "TestStepExecutions",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "TestStepExecutions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Sqlite:Autoincrement", true)
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<string>(
                name: "Trigger",
                table: "TestRuns",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "TestDataId",
                table: "TestRuns",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TargetIds",
                table: "TestRuns",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<double>(
                name: "SuccessRate",
                table: "TestRuns",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "TestRuns",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartedAt",
                table: "TestRuns",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SkippedCount",
                table: "TestRuns",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Screenshots",
                table: "TestRuns",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReportPath",
                table: "TestRuns",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProgressPercentage",
                table: "TestRuns",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "PassedCount",
                table: "TestRuns",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TestRuns",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "Headless",
                table: "TestRuns",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<int>(
                name: "FailedCount",
                table: "TestRuns",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "ExecutionType",
                table: "TestRuns",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ExecutionLogs",
                table: "TestRuns",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EnvironmentId",
                table: "TestRuns",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "DetailedResults",
                table: "TestRuns",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "TestRuns",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CompletedAt",
                table: "TestRuns",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Browser",
                table: "TestRuns",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<double>(
                name: "AverageDurationMs",
                table: "TestRuns",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<int>(
                name: "ApplicationId",
                table: "TestRuns",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "TestRuns",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Sqlite:Autoincrement", true)
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "TestRuns",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SpecificEnvironmentId",
                table: "TestDataSets",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TestDataSets",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsTemplate",
                table: "TestDataSets",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TestDataSets",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DataType",
                table: "TestDataSets",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "DataJson",
                table: "TestDataSets",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "TestDataSets",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<int>(
                name: "ApplicationId",
                table: "TestDataSets",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "TestDataSets",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Sqlite:Autoincrement", true)
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<string>(
                name: "UserPersona",
                table: "TestCases",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Tags",
                table: "TestCases",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Selected",
                table: "TestCases",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TestCases",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "FeatureId",
                table: "TestCases",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "ExecutionFrequency",
                table: "TestCases",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "EstimatedMinutes",
                table: "TestCases",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TestCases",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CriticalityLevel",
                table: "TestCases",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "TestCases",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<bool>(
                name: "Active",
                table: "TestCases",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "TestCases",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Sqlite:Autoincrement", true)
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "TotalSteps",
                table: "TestCaseExecutions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "TestRunId",
                table: "TestCaseExecutions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "TestCaseName",
                table: "TestCaseExecutions",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<int>(
                name: "TestCaseId",
                table: "TestCaseExecutions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "TestCaseExecutions",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartedAt",
                table: "TestCaseExecutions",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<int>(
                name: "PassedSteps",
                table: "TestCaseExecutions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "FailedSteps",
                table: "TestCaseExecutions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "ErrorStackTrace",
                table: "TestCaseExecutions",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ErrorMessage",
                table: "TestCaseExecutions",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "DurationMs",
                table: "TestCaseExecutions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CompletedAt",
                table: "TestCaseExecutions",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "TestCaseExecutions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Sqlite:Autoincrement", true)
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "SystemSettings",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "SystemSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "SystemSettings",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "SystemSettings",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "IsEncrypted",
                table: "SystemSettings",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SystemSettings",
                type: "TEXT",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DataType",
                table: "SystemSettings",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "SystemSettings",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "SystemSettings",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Sqlite:Autoincrement", true)
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Screenshots",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "TestStepExecutionId",
                table: "Screenshots",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TestRunId",
                table: "Screenshots",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "TestCaseExecutionId",
                table: "Screenshots",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "FileSizeBytes",
                table: "Screenshots",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "Screenshots",
                type: "TEXT",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Screenshots",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CapturedAt",
                table: "Screenshots",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Screenshots",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Sqlite:Autoincrement", true)
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<double>(
                name: "Value",
                table: "PerformanceMetrics",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<string>(
                name: "Unit",
                table: "PerformanceMetrics",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "TestRunId",
                table: "PerformanceMetrics",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "TestCaseExecutionId",
                table: "PerformanceMetrics",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RecordedAt",
                table: "PerformanceMetrics",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "MetricName",
                table: "PerformanceMetrics",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Context",
                table: "PerformanceMetrics",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "PerformanceMetrics",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Sqlite:Autoincrement", true)
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "Jobs",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<int>(
                name: "TestCaseId",
                table: "Jobs",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NextExecutionTimeUtc",
                table: "Jobs",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Jobs",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastExecutionTimeUtc",
                table: "Jobs",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastExecutionStatus",
                table: "Jobs",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsRunning",
                table: "Jobs",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "IsEnabled",
                table: "Jobs",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<int>(
                name: "FrequencyInMinutes",
                table: "Jobs",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EnvironmentId",
                table: "Jobs",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Jobs",
                type: "TEXT",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Jobs",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<int>(
                name: "ConsecutiveFailures",
                table: "Jobs",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Jobs",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Sqlite:Autoincrement", true)
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Features",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Features",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "Features",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Features",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Features",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Complexity",
                table: "Features",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "BusinessPriority",
                table: "Features",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ApplicationId",
                table: "Features",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<bool>(
                name: "Active",
                table: "Features",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Features",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Sqlite:Autoincrement", true)
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "ExecutionLogs",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<int>(
                name: "TestRunId",
                table: "ExecutionLogs",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "TestCaseExecutionId",
                table: "ExecutionLogs",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StackTrace",
                table: "ExecutionLogs",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "ExecutionLogs",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Level",
                table: "ExecutionLogs",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Details",
                table: "ExecutionLogs",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ExecutionLogs",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Sqlite:Autoincrement", true)
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Environments",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "RequiresAuth",
                table: "Environments",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Environments",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Environments",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Environments",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "BaseUrl",
                table: "Environments",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "ApplicationId",
                table: "Environments",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<bool>(
                name: "Active",
                table: "Environments",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "AccessInfo",
                table: "Environments",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Environments",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Sqlite:Autoincrement", true)
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Applications",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<bool>(
                name: "Selected",
                table: "Applications",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Applications",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "MainUrl",
                table: "Applications",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Applications",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Applications",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "AppType",
                table: "Applications",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "Active",
                table: "Applications",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Applications",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Sqlite:Autoincrement", true)
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ActionTemplates",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "ActionTemplates",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Example",
                table: "ActionTemplates",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ActionTemplates",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "ActionTemplates",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ActionTemplates",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Sqlite:Autoincrement", true)
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1463));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1469));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1470));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1472));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1473));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1477));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1478));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 10,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1480));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 11,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1481));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 12,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1483));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 13,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1484));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 14,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1486));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 15,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1487));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 20,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1488));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 21,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1489));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 22,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1491));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 23,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1492));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 24,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1527));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 25,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1528));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 26,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1530));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 27,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1532));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 28,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1533));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 30,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1534));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 31,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1535));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 32,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1536));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 33,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1537));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 34,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1539));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 35,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1540));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 36,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1541));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 37,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1542));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 40,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1544));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 41,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1545));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 42,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1546));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 50,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1551));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 51,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1552));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 52,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1553));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 53,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1554));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 60,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1556));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 61,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1557));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 62,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1558));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 63,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1559));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 64,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1560));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "TestRuns");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "TestSteps",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TimeoutSeconds",
                table: "TestSteps",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "TestCaseId",
                table: "TestSteps",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Target",
                table: "TestSteps",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Selector",
                table: "TestSteps",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Order",
                table: "TestSteps",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<bool>(
                name: "IsOptional",
                table: "TestSteps",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TestSteps",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Action",
                table: "TestSteps",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "TestSteps",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true)
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "TestStepExecutions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "TestStepId",
                table: "TestStepExecutions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "TestCaseExecutionId",
                table: "TestStepExecutions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "StepOrder",
                table: "TestStepExecutions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "TestStepExecutions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartedAt",
                table: "TestStepExecutions",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Selector",
                table: "TestStepExecutions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "ScreenshotPath",
                table: "TestStepExecutions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<bool>(
                name: "IsOptional",
                table: "TestStepExecutions",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "ErrorMessage",
                table: "TestStepExecutions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "DurationMs",
                table: "TestStepExecutions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TestStepExecutions",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CompletedAt",
                table: "TestStepExecutions",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Action",
                table: "TestStepExecutions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "TestStepExecutions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true)
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<string>(
                name: "Trigger",
                table: "TestRuns",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "TestDataId",
                table: "TestRuns",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TargetIds",
                table: "TestRuns",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<double>(
                name: "SuccessRate",
                table: "TestRuns",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "TestRuns",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartedAt",
                table: "TestRuns",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SkippedCount",
                table: "TestRuns",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Screenshots",
                table: "TestRuns",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReportPath",
                table: "TestRuns",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProgressPercentage",
                table: "TestRuns",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "PassedCount",
                table: "TestRuns",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TestRuns",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<bool>(
                name: "Headless",
                table: "TestRuns",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "FailedCount",
                table: "TestRuns",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "ExecutionType",
                table: "TestRuns",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "ExecutionLogs",
                table: "TestRuns",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EnvironmentId",
                table: "TestRuns",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "DetailedResults",
                table: "TestRuns",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "TestRuns",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CompletedAt",
                table: "TestRuns",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Browser",
                table: "TestRuns",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<double>(
                name: "AverageDurationMs",
                table: "TestRuns",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AlterColumn<int>(
                name: "ApplicationId",
                table: "TestRuns",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "TestRuns",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true)
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "SpecificEnvironmentId",
                table: "TestDataSets",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TestDataSets",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<bool>(
                name: "IsTemplate",
                table: "TestDataSets",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TestDataSets",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DataType",
                table: "TestDataSets",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "DataJson",
                table: "TestDataSets",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "TestDataSets",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "ApplicationId",
                table: "TestDataSets",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "TestDataSets",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true)
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<string>(
                name: "UserPersona",
                table: "TestCases",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Tags",
                table: "TestCases",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Selected",
                table: "TestCases",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TestCases",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "FeatureId",
                table: "TestCases",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "ExecutionFrequency",
                table: "TestCases",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "EstimatedMinutes",
                table: "TestCases",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TestCases",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CriticalityLevel",
                table: "TestCases",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "TestCases",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<bool>(
                name: "Active",
                table: "TestCases",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "TestCases",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true)
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "TotalSteps",
                table: "TestCaseExecutions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "TestRunId",
                table: "TestCaseExecutions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "TestCaseName",
                table: "TestCaseExecutions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<int>(
                name: "TestCaseId",
                table: "TestCaseExecutions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "TestCaseExecutions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartedAt",
                table: "TestCaseExecutions",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "PassedSteps",
                table: "TestCaseExecutions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "FailedSteps",
                table: "TestCaseExecutions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "ErrorStackTrace",
                table: "TestCaseExecutions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "ErrorMessage",
                table: "TestCaseExecutions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "DurationMs",
                table: "TestCaseExecutions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CompletedAt",
                table: "TestCaseExecutions",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "TestCaseExecutions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true)
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "SystemSettings",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "SystemSettings",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "SystemSettings",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "SystemSettings",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "IsEncrypted",
                table: "SystemSettings",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SystemSettings",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DataType",
                table: "SystemSettings",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "SystemSettings",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "SystemSettings",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true)
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Screenshots",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "TestStepExecutionId",
                table: "Screenshots",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TestRunId",
                table: "Screenshots",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "TestCaseExecutionId",
                table: "Screenshots",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "FileSizeBytes",
                table: "Screenshots",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "Screenshots",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Screenshots",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CapturedAt",
                table: "Screenshots",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Screenshots",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true)
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<double>(
                name: "Value",
                table: "PerformanceMetrics",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AlterColumn<string>(
                name: "Unit",
                table: "PerformanceMetrics",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "TestRunId",
                table: "PerformanceMetrics",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "TestCaseExecutionId",
                table: "PerformanceMetrics",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RecordedAt",
                table: "PerformanceMetrics",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "MetricName",
                table: "PerformanceMetrics",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Context",
                table: "PerformanceMetrics",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "PerformanceMetrics",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true)
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "Jobs",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "TestCaseId",
                table: "Jobs",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NextExecutionTimeUtc",
                table: "Jobs",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Jobs",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastExecutionTimeUtc",
                table: "Jobs",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastExecutionStatus",
                table: "Jobs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsRunning",
                table: "Jobs",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<bool>(
                name: "IsEnabled",
                table: "Jobs",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "FrequencyInMinutes",
                table: "Jobs",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EnvironmentId",
                table: "Jobs",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Jobs",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Jobs",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "ConsecutiveFailures",
                table: "Jobs",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Jobs",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true)
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Features",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Features",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "Features",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Features",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Features",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Complexity",
                table: "Features",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "BusinessPriority",
                table: "Features",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "ApplicationId",
                table: "Features",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<bool>(
                name: "Active",
                table: "Features",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Features",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true)
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "ExecutionLogs",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "TestRunId",
                table: "ExecutionLogs",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "TestCaseExecutionId",
                table: "ExecutionLogs",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StackTrace",
                table: "ExecutionLogs",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "ExecutionLogs",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Level",
                table: "ExecutionLogs",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Details",
                table: "ExecutionLogs",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ExecutionLogs",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true)
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Environments",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<bool>(
                name: "RequiresAuth",
                table: "Environments",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Environments",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Environments",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Environments",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "BaseUrl",
                table: "Environments",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "ApplicationId",
                table: "Environments",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<bool>(
                name: "Active",
                table: "Environments",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "AccessInfo",
                table: "Environments",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Environments",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true)
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Applications",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<bool>(
                name: "Selected",
                table: "Applications",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Applications",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "MainUrl",
                table: "Applications",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Applications",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Applications",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "AppType",
                table: "Applications",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<bool>(
                name: "Active",
                table: "Applications",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Applications",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true)
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ActionTemplates",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "ActionTemplates",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Example",
                table: "ActionTemplates",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ActionTemplates",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "ActionTemplates",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ActionTemplates",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true)
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2340));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2340));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2350));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2350));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2350));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2350));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2350));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 10,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2350));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 11,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2360));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 12,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2360));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 13,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2360));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 14,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2360));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 15,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2360));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 20,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2360));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 21,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2360));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 22,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2360));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 23,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2370));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 24,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2370));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 25,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2370));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 26,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2370));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 27,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2370));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 28,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2370));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 30,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2370));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 31,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2380));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 32,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2380));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 33,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2380));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 34,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2380));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 35,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2380));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 36,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2380));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 37,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2380));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 40,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2380));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 41,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2380));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 42,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2390));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 50,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2390));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 51,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2390));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 52,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2390));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 53,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2390));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 60,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2390));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 61,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2390));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 62,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2390));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 63,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2400));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 64,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 20, 7, 29, 54, 852, DateTimeKind.Utc).AddTicks(2400));
        }
    }
}
