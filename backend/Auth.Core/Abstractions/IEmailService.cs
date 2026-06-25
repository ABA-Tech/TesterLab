using TesterLab.Rappory.Models;

namespace Auth.Core.Abstractions
{
    /// <summary>
    /// Service d'envoi d'emails.
    /// </summary>
    public interface IEmailService
    {
        Task SendEmailConfirmationAsync(string email, string username, string confirmationLink);
        Task SendPasswordResetAsync(string email, string username, string resetLink);
        Task SendWelcomeEmailAsync(string email, string username);
        Task SendPasswordChangedNotificationAsync(string email, string username);
        Task SendTestRunReportAsync(IEnumerable<string> recipients, TestRunReportData r);
    }
}
