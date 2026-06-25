using Auth.Core.Abstractions;
using Microsoft.Extensions.Logging;
using TesterLab.Domain.interfaces.Repositories;
using TesterLab.Domain.interfaces.Services;
using TesterLab.Rappory.Services;

namespace TesterLab.Services
{
    public class TestRunNotificationService : ITestRunNotificationService
    {
        private readonly ITestRunRepository _testRunRepository;
        private readonly IUserRepository _userRepository;
        private readonly IReportDataService _reportDataService;
        private readonly IEmailService _emailService;
        private readonly ILogger<TestRunNotificationService> _logger;

        public TestRunNotificationService(
            ITestRunRepository testRunRepository,
            IUserRepository userRepository,
            IReportDataService reportDataService,
            IEmailService emailService,
            ILogger<TestRunNotificationService> logger)
        {
            _testRunRepository = testRunRepository;
            _userRepository = userRepository;
            _reportDataService = reportDataService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task NotifyAsync(int testRunId)
        {
            try
            {
                var testRun = await _testRunRepository.GetByIdAsync(testRunId);
                if (testRun == null)
                {
                    _logger.LogWarning("Notification : TestRun {RunId} introuvable", testRunId);
                    return;
                }

                if (string.IsNullOrWhiteSpace(testRun.CreatedByUserId))
                {
                    _logger.LogInformation(
                        "Aucun utilisateur associé au run {RunId}, notification ignorée", testRunId);
                    return;
                }

                var user = await _userRepository.GetByIdAsync(testRun.CreatedByUserId);
                if (user == null || string.IsNullOrWhiteSpace(user.Email))
                {
                    _logger.LogWarning(
                        "Utilisateur {UserId} introuvable ou sans email pour le run {RunId}",
                        testRun.CreatedByUserId, testRunId);
                    return;
                }

                var reportData = await _reportDataService.CollectTestRunDataAsync(testRunId, false);
                await _emailService.SendTestRunReportAsync(new[] { user.Email }, reportData);

                _logger.LogInformation(
                    "Notification envoyée à {Email} pour le run {RunId}", user.Email, testRunId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Erreur lors de l'envoi de la notification pour le run {RunId}", testRunId);
            }
        }
    }
}