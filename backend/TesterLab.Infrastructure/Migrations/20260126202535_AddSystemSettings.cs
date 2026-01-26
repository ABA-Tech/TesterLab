using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TesterLab.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Category = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DataType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    IsEncrypted = table.Column<bool>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "SystemSettings",
                columns: new[] { "Id", "Category", "DataType", "Description", "IsEncrypted", "Key", "UpdatedAt", "UpdatedBy", "Value" },
                values: new object[,]
                {
                    { 1, "General", "String", "Nom de l'application", false, "General.ApplicationName", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6656), null, "TesterLab" },
                    { 2, "General", "String", null, false, "General.BaseUrl", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6664), null, "http://localhost" },
                    { 3, "General", "String", null, false, "General.TimeZone", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6668), null, "UTC" },
                    { 4, "General", "String", null, false, "General.DefaultLanguage", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6670), null, "fr-FR" },
                    { 5, "General", "Boolean", null, false, "General.MaintenanceMode", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6671), null, "false" },
                    { 6, "General", "Boolean", null, false, "General.AllowRegistration", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6677), null, "true" },
                    { 7, "General", "Boolean", null, false, "General.RequireEmailConfirmation", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6679), null, "true" },
                    { 10, "Email", "String", null, false, "Email.SmtpHost", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6680), null, "smtp.gmail.com" },
                    { 11, "Email", "Integer", null, false, "Email.SmtpPort", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6683), null, "587" },
                    { 12, "Email", "Boolean", null, false, "Email.EnableSsl", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6686), null, "true" },
                    { 13, "Email", "String", null, false, "Email.FromEmail", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6688), null, "noreply@testerlab.com" },
                    { 14, "Email", "String", null, false, "Email.FromName", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6689), null, "TesterLab" },
                    { 15, "Email", "Boolean", null, false, "Email.EmailEnabled", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6690), null, "true" },
                    { 20, "Testing", "Integer", null, false, "Testing.DefaultTimeoutSeconds", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6691), null, "30" },
                    { 21, "Testing", "Integer", null, false, "Testing.MaxRetries", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6692), null, "3" },
                    { 22, "Testing", "String", null, false, "Testing.DefaultBrowser", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6694), null, "Chrome" },
                    { 23, "Testing", "Boolean", null, false, "Testing.DefaultHeadlessMode", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6696), null, "true" },
                    { 24, "Testing", "Boolean", null, false, "Testing.CaptureScreenshotOnFailure", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6699), null, "true" },
                    { 25, "Testing", "Boolean", null, false, "Testing.CaptureScreenshotOnSuccess", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6701), null, "false" },
                    { 26, "Testing", "Integer", null, false, "Testing.RetentionDays", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6703), null, "30" },
                    { 27, "Testing", "Integer", null, false, "Testing.MaxParallelExecutions", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6704), null, "5" },
                    { 28, "Testing", "Boolean", null, false, "Testing.AutoRetryOnFailure", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6706), null, "true" },
                    { 30, "Security", "Integer", null, false, "Security.MinPasswordLength", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6707), null, "12" },
                    { 31, "Security", "Boolean", null, false, "Security.RequireUppercase", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6708), null, "true" },
                    { 32, "Security", "Boolean", null, false, "Security.RequireLowercase", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6710), null, "true" },
                    { 33, "Security", "Boolean", null, false, "Security.RequireDigit", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6711), null, "true" },
                    { 34, "Security", "Boolean", null, false, "Security.RequireNonAlphanumeric", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6712), null, "true" },
                    { 35, "Security", "Integer", null, false, "Security.MaxFailedLoginAttempts", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6714), null, "5" },
                    { 36, "Security", "Integer", null, false, "Security.LockoutDurationMinutes", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6716), null, "15" },
                    { 37, "Security", "Integer", null, false, "Security.SessionTimeoutMinutes", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6718), null, "720" },
                    { 40, "Branding", "String", null, false, "Branding.PrimaryColor", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6719), null, "#007bff" },
                    { 41, "Branding", "String", null, false, "Branding.SecondaryColor", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6720), null, "#6c757d" },
                    { 42, "Branding", "Boolean", null, false, "Branding.ShowCompanyName", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6722), null, "true" },
                    { 50, "Notifications", "Boolean", null, false, "Notifications.NotifyOnTestFailure", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6725), null, "true" },
                    { 51, "Notifications", "Boolean", null, false, "Notifications.NotifyOnTestSuccess", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6726), null, "false" },
                    { 52, "Notifications", "Boolean", null, false, "Notifications.NotifyOnScheduledRuns", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6728), null, "true" },
                    { 53, "Notifications", "Integer", null, false, "Notifications.MinSuccessRateForNotification", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6729), null, "90" },
                    { 60, "Storage", "String", null, false, "Storage.ScreenshotPath", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6730), null, "wwwroot/screenshots" },
                    { 61, "Storage", "String", null, false, "Storage.ReportsPath", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6732), null, "wwwroot/reports" },
                    { 62, "Storage", "Integer", null, false, "Storage.MaxFileSizeMB", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6835), null, "10" },
                    { 63, "Storage", "Boolean", null, false, "Storage.AutoCleanupEnabled", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6838), null, "true" },
                    { 64, "Storage", "Integer", null, false, "Storage.FileRetentionDays", new DateTime(2026, 1, 26, 20, 25, 34, 839, DateTimeKind.Utc).AddTicks(6839), null, "30" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_Category",
                table: "SystemSettings",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_Key",
                table: "SystemSettings",
                column: "Key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SystemSettings");
        }
    }
}
