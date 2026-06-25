namespace TesterLab.Domain.interfaces.Services
{
    /// <summary>
    /// Défini dans Domain pour être accessible depuis TesterLab.Application
    /// sans créer de dépendance vers Auth.Core ou TesterLab.Rappory.
    /// L'implémentation concrète vit dans le front qui a accès à tout.
    /// </summary>
    public interface ITestRunNotificationService
    {
        /// <summary>
        /// Envoie la notification de fin d'exécution à l'auteur du run.
        /// Ne lève pas d'exception — les erreurs sont loggées silencieusement.
        /// </summary>
        Task NotifyAsync(int testRunId);
    }
}