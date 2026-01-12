using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System;
using TesterLab.Domain.Models;
using TesterLab.Infrastructure.Data;
using TesterLab.JobScheduler.Services;
using TesterLab.Domain.interfaces.Services;

namespace TesterLab.JobScheduler.BackgroundServices
{

    public class JobSchedulerService : BackgroundService
    {
        private readonly ILogger<JobSchedulerService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private const int CheckIntervalSeconds = 30; // Vérifier toutes les 30 secondes

        public JobSchedulerService(
            ILogger<JobSchedulerService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Job Scheduler Service démarré");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessPendingJobsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur dans le Job Scheduler");
                }

                // Attendre avant la prochaine vérification
                await Task.Delay(TimeSpan.FromSeconds(CheckIntervalSeconds), stoppingToken);
            }

            _logger.LogInformation("Job Scheduler Service arrêté");
        }

        private async Task ProcessPendingJobsAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TesterLabDbContext>();

            var now = DateTime.Now;

            // Récupérer les jobs prêts à être exécutés
            var jobsToExecute = await dbContext.Jobs
                .Where(j => j.IsEnabled &&
                           !j.IsRunning &&
                           j.NextExecutionTimeUtc <= now)
                .OrderBy(j => j.NextExecutionTimeUtc)
                .ToListAsync(cancellationToken);

            _logger.LogInformation($"{jobsToExecute.Count} job(s) à exécuter");

            foreach (var job in jobsToExecute)
            {
                // Exécuter chaque job de manière asynchrone sans bloquer
                _ = Task.Run(async () => await ExecuteJobAsync(job.Id), cancellationToken);
            }
        }

        private async Task<TestRun> prepareExecution(TesterLabDbContext dbContext, Job job)
        {
            var testCase = dbContext.TestCases.Include(x=>x.Feature).FirstOrDefault(t=>t.Id == job.TestCaseId);
            if (testCase == null)
                return null;

            var testRun = new TestRun
            {
                ApplicationId = testCase.Feature.ApplicationId, // ID de l'application
                Name = $"Job execution - TestCase #{job.TestCaseId}",
                ExecutionType = "testcase", // Type important !
                TargetIds = JsonSerializer.Serialize(new[] { job.TestCaseId }), // Un seul ID
                EnvironmentId = job.EnvironmentId,
                TestDataId = null, // Optionnel
                Browser = "Chrome",
                Headless = true,
                Trigger = "Auto",
                Status = "Created",
                ProgressPercentage = 0,
                CreatedAt = DateTime.Now
            };

            dbContext.TestRuns.Add(testRun);
            var res = await dbContext.SaveChangesAsync();

            return testRun;
        }

        private async Task ExecuteJobAsync(int jobId)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TesterLabDbContext>();
            var jobService = scope.ServiceProvider.GetRequiredService<ITestSchedulerService>();

            // Utiliser une transaction pour garantir la cohérence
            using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                // Recharger le job avec un verrou pessimiste pour éviter la concurrence
                var job = await dbContext.Jobs.Where(x=>x.Id == jobId).FirstOrDefaultAsync();

                if (job == null || !job.IsEnabled || job.IsRunning)
                {
                    _logger.LogWarning($"Job {jobId} ignoré (désactivé ou déjà en cours)");
                    return;
                }

                // Marquer comme en cours d'exécution
                job.IsRunning = true;
                job.LastExecutionTimeUtc = DateTime.Now;
                job.UpdatedAtUtc = DateTime.Now;
                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"Début de l'exécution du job {jobId} - {job.Name}");

                var run = await prepareExecution(dbContext, job);
                // Exécuter la tâche métier
                await jobService.ExecuteAsync(jobId, run.Id);

                // Mettre à jour le statut après succès
                using var successScope = _serviceProvider.CreateScope();
                var successDbContext = successScope.ServiceProvider.GetRequiredService<TesterLabDbContext>();
                var successJob = await successDbContext.Jobs.FindAsync(jobId);

                if (successJob != null)
                {
                    successJob.IsRunning = false;
                    successJob.LastExecutionStatus = "Success";
                    successJob.ConsecutiveFailures = 0;

                    // Calculer la prochaine exécution
                    if (successJob.FrequencyInMinutes.HasValue)
                    {
                        successJob.NextExecutionTimeUtc = DateTime.Now.AddMinutes(successJob.FrequencyInMinutes.Value);
                    }
                    else
                    {
                        // Job unique : désactiver après exécution
                        successJob.IsEnabled = false;
                    }

                    successJob.UpdatedAtUtc = DateTime.Now;
                    await successDbContext.SaveChangesAsync();

                    _logger.LogInformation($"Job {jobId} terminé avec succès. Prochaine exécution: {successJob.NextExecutionTimeUtc}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de l'exécution du job {jobId}");

                // Mettre à jour le statut après échec
                using var failScope = _serviceProvider.CreateScope();
                var failDbContext = failScope.ServiceProvider.GetRequiredService<TesterLabDbContext>();
                var failJob = await failDbContext.Jobs.FindAsync(jobId);

                if (failJob != null)
                {
                    failJob.IsRunning = false;
                    failJob.LastExecutionStatus = $"Failed: {ex.Message}";
                    failJob.ConsecutiveFailures++;

                    // Stratégie de retry avec backoff exponentiel
                    if (failJob.FrequencyInMinutes.HasValue)
                    {
                        var backoffMinutes = Math.Min(failJob.ConsecutiveFailures * 5, 60);
                        failJob.NextExecutionTimeUtc = DateTime.Now.AddMinutes(backoffMinutes);
                    }

                    // Désactiver après 5 échecs consécutifs
                    if (failJob.ConsecutiveFailures >= 5)
                    {
                        failJob.IsEnabled = false;
                        _logger.LogWarning($"Job {jobId} désactivé après 5 échecs consécutifs");
                    }

                    failJob.UpdatedAtUtc = DateTime.Now;
                    await failDbContext.SaveChangesAsync();
                }
            }
        }
    }
}
