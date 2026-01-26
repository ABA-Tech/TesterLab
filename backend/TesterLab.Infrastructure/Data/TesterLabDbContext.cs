using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesterLab.Domain.Models;
using Environment = TesterLab.Domain.Models.Environment;

namespace TesterLab.Infrastructure.Data
{
    public class TesterLabDbContext : DbContext
    {
        public TesterLabDbContext(DbContextOptions<TesterLabDbContext> options) : base(options) { }

        public DbSet<Application> Applications { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<TestCase> TestCases { get; set; }
        public DbSet<TestStep> TestSteps { get; set; }
        public DbSet<TestData> TestDataSets { get; set; }
        public DbSet<Environment> Environments { get; set; }
        public DbSet<TestRun> TestRuns { get; set; }
        public DbSet<ActionTemplate> ActionTemplates { get; set; }
        public DbSet<TestCaseExecution> TestCaseExecutions { get; set; }
        public DbSet<TestStepExecution> TestStepExecutions { get; set; }
        public DbSet<ExecutionLog> ExecutionLogs { get; set; }
        public DbSet<PerformanceMetric> PerformanceMetrics { get; set; }
        public DbSet<Screenshot> Screenshots { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration des relations
            modelBuilder.Entity<Feature>()
                .HasOne(f => f.Application)
                .WithMany(a => a.Features)
                .HasForeignKey(f => f.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TestCase>()
                .HasOne(tc => tc.Feature)
                .WithMany(f => f.TestCases)
                .HasForeignKey(tc => tc.FeatureId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TestStep>()
                .HasOne(ts => ts.TestCase)
                .WithMany(tc => tc.TestSteps)
                .HasForeignKey(ts => ts.TestCaseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index pour les performances
            modelBuilder.Entity<TestCase>()
                .HasIndex(tc => new { tc.FeatureId, tc.Active });

            modelBuilder.Entity<TestRun>()
                .HasIndex(tr => new { tr.ApplicationId, tr.Status });

            modelBuilder.Entity<Environment>()
                .HasIndex(e => new { e.ApplicationId, e.Active })
                .HasDatabaseName("IX_Environment_ApplicationId_Active");
            // Seed des ActionTemplates
            modelBuilder.Entity<ActionTemplate>().HasData(
                      ActionTemplate.GetDefaultTemplates().Select((t, i) => new ActionTemplate
                      {
                          Id = i + 1,
                          Name = t.Name,
                          Category = t.Category,
                          Icon = t.Icon,
                          Description = t.Description,
                          Example = t.Example
                      })
                  );

            // Configuration SystemSetting
            modelBuilder.Entity<SystemSetting>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Key).IsUnique();
                entity.HasIndex(e => e.Category);
            });
            // Seed des paramètres par défaut
            SeedDefaultSettings(modelBuilder);
        }

        private void SeedDefaultSettings(ModelBuilder modelBuilder)
        {
            var settings = new List<SystemSetting>
            {
                // General
                new SystemSetting { Id = 1, Key = "General.ApplicationName", Value = "TesterLab", Category = "General", DataType = "String", Description = "Nom de l'application" },
                new SystemSetting { Id = 2, Key = "General.BaseUrl", Value = "http://localhost", Category = "General", DataType = "String" },
                new SystemSetting { Id = 3, Key = "General.TimeZone", Value = "UTC", Category = "General", DataType = "String" },
                new SystemSetting { Id = 4, Key = "General.DefaultLanguage", Value = "fr-FR", Category = "General", DataType = "String" },
                new SystemSetting { Id = 5, Key = "General.MaintenanceMode", Value = "false", Category = "General", DataType = "Boolean" },
                new SystemSetting { Id = 6, Key = "General.AllowRegistration", Value = "true", Category = "General", DataType = "Boolean" },
                new SystemSetting { Id = 7, Key = "General.RequireEmailConfirmation", Value = "true", Category = "General", DataType = "Boolean" },

                // Email
                new SystemSetting { Id = 10, Key = "Email.SmtpHost", Value = "smtp.gmail.com", Category = "Email", DataType = "String" },
                new SystemSetting { Id = 11, Key = "Email.SmtpPort", Value = "587", Category = "Email", DataType = "Integer" },
                new SystemSetting { Id = 12, Key = "Email.EnableSsl", Value = "true", Category = "Email", DataType = "Boolean" },
                new SystemSetting { Id = 13, Key = "Email.FromEmail", Value = "noreply@testerlab.com", Category = "Email", DataType = "String" },
                new SystemSetting { Id = 14, Key = "Email.FromName", Value = "TesterLab", Category = "Email", DataType = "String" },
                new SystemSetting { Id = 15, Key = "Email.EmailEnabled", Value = "true", Category = "Email", DataType = "Boolean" },

                // Testing
                new SystemSetting { Id = 20, Key = "Testing.DefaultTimeoutSeconds", Value = "30", Category = "Testing", DataType = "Integer" },
                new SystemSetting { Id = 21, Key = "Testing.MaxRetries", Value = "3", Category = "Testing", DataType = "Integer" },
                new SystemSetting { Id = 22, Key = "Testing.DefaultBrowser", Value = "Chrome", Category = "Testing", DataType = "String" },
                new SystemSetting { Id = 23, Key = "Testing.DefaultHeadlessMode", Value = "true", Category = "Testing", DataType = "Boolean" },
                new SystemSetting { Id = 24, Key = "Testing.CaptureScreenshotOnFailure", Value = "true", Category = "Testing", DataType = "Boolean" },
                new SystemSetting { Id = 25, Key = "Testing.CaptureScreenshotOnSuccess", Value = "false", Category = "Testing", DataType = "Boolean" },
                new SystemSetting { Id = 26, Key = "Testing.RetentionDays", Value = "30", Category = "Testing", DataType = "Integer" },
                new SystemSetting { Id = 27, Key = "Testing.MaxParallelExecutions", Value = "5", Category = "Testing", DataType = "Integer" },
                new SystemSetting { Id = 28, Key = "Testing.AutoRetryOnFailure", Value = "true", Category = "Testing", DataType = "Boolean" },

                // Security
                new SystemSetting { Id = 30, Key = "Security.MinPasswordLength", Value = "12", Category = "Security", DataType = "Integer" },
                new SystemSetting { Id = 31, Key = "Security.RequireUppercase", Value = "true", Category = "Security", DataType = "Boolean" },
                new SystemSetting { Id = 32, Key = "Security.RequireLowercase", Value = "true", Category = "Security", DataType = "Boolean" },
                new SystemSetting { Id = 33, Key = "Security.RequireDigit", Value = "true", Category = "Security", DataType = "Boolean" },
                new SystemSetting { Id = 34, Key = "Security.RequireNonAlphanumeric", Value = "true", Category = "Security", DataType = "Boolean" },
                new SystemSetting { Id = 35, Key = "Security.MaxFailedLoginAttempts", Value = "5", Category = "Security", DataType = "Integer" },
                new SystemSetting { Id = 36, Key = "Security.LockoutDurationMinutes", Value = "15", Category = "Security", DataType = "Integer" },
                new SystemSetting { Id = 37, Key = "Security.SessionTimeoutMinutes", Value = "720", Category = "Security", DataType = "Integer" },

                // Branding
                new SystemSetting { Id = 40, Key = "Branding.PrimaryColor", Value = "#007bff", Category = "Branding", DataType = "String" },
                new SystemSetting { Id = 41, Key = "Branding.SecondaryColor", Value = "#6c757d", Category = "Branding", DataType = "String" },
                new SystemSetting { Id = 42, Key = "Branding.ShowCompanyName", Value = "true", Category = "Branding", DataType = "Boolean" },

                // Notifications
                new SystemSetting { Id = 50, Key = "Notifications.NotifyOnTestFailure", Value = "true", Category = "Notifications", DataType = "Boolean" },
                new SystemSetting { Id = 51, Key = "Notifications.NotifyOnTestSuccess", Value = "false", Category = "Notifications", DataType = "Boolean" },
                new SystemSetting { Id = 52, Key = "Notifications.NotifyOnScheduledRuns", Value = "true", Category = "Notifications", DataType = "Boolean" },
                new SystemSetting { Id = 53, Key = "Notifications.MinSuccessRateForNotification", Value = "90", Category = "Notifications", DataType = "Integer" },

                // Storage
                new SystemSetting { Id = 60, Key = "Storage.ScreenshotPath", Value = "wwwroot/screenshots", Category = "Storage", DataType = "String" },
                new SystemSetting { Id = 61, Key = "Storage.ReportsPath", Value = "wwwroot/reports", Category = "Storage", DataType = "String" },
                new SystemSetting { Id = 62, Key = "Storage.MaxFileSizeMB", Value = "10", Category = "Storage", DataType = "Integer" },
                new SystemSetting { Id = 63, Key = "Storage.AutoCleanupEnabled", Value = "true", Category = "Storage", DataType = "Boolean" },
                new SystemSetting { Id = 64, Key = "Storage.FileRetentionDays", Value = "30", Category = "Storage", DataType = "Integer" },
            };

            modelBuilder.Entity<SystemSetting>().HasData(settings);
        }
    }
}
