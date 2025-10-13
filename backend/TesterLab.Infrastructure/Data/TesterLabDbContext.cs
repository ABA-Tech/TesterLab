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
        }
    }
}
