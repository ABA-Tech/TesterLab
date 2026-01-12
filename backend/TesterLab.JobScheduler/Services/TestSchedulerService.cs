using Microsoft.Extensions.Logging;
using System.Text.Json;
using TesterLab.Domain.interfaces.Services;
using TesterLab.Domain.Models;

namespace TesterLab.JobScheduler.Services
{
    public interface ITestSchedulerService
    {
        Task ExecuteAsync(int jobId, int runId = 0);
    }

    public class TestSchedulerService : ITestSchedulerService
    {
        private readonly ILogger<TestSchedulerService> _logger;
        private readonly ITestExecutionService3 _testExecutionService;

        public TestSchedulerService(ILogger<TestSchedulerService> logger, ITestExecutionService3 testExecution)
        {
            _logger = logger;
            _testExecutionService = testExecution;
        }

        public async Task ExecuteAsync(int jobId, int runId = 0)
        {
            _logger.LogInformation($"Début de l'exécution du job {jobId}");

            try
            {
                // Lancer l'exécution
                await _testExecutionService.StartTestRunAsync(runId);

                // Exemple : envoyer un email, générer un rapport, nettoyer des données, etc.
                _logger.LogInformation($"Job {jobId} exécuté avec succès");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de l'exécution du job {jobId}");
                throw; // Propager l'exception pour que le scheduler puisse la gérer
            }
        }
    }
}
