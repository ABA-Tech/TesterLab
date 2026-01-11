using Microsoft.Extensions.Logging;

namespace TesterLab.JobScheduler.Services
{
    public interface ITestSchedulerService
    {
        Task ExecuteAsync(int jobId);
    }

    public class TestSchedulerService
    {
        private readonly ILogger<TestSchedulerService> _logger;

        public TestSchedulerService(ILogger<TestSchedulerService> logger)
        {
            _logger = logger;
        }

        public async Task ExecuteAsync(int jobId)
        {
            _logger.LogInformation($"Début de l'exécution du job {jobId}");

            try
            {
                // Simuler une tâche métier (remplacer par votre logique réelle)
                await Task.Delay(2000); // Simule un traitement de 2 secondes

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
