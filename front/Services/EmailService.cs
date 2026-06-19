using Auth.Core.Abstractions;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using Resend;

namespace TesterLab.Services
{
  /// <summary>
  /// Service d'envoi d'emails via SMTP.
  /// </summary>
  public class EmailService : IEmailService
  {
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;
    private readonly IResend _resend;


    public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger, IResend resend)
    {
      _settings = settings.Value;
      _logger = logger;
      _resend = resend;
    }

    public async Task SendEmailConfirmationAsync(string email, string username, string confirmationLink)
    {
      var subject = "Confirmez votre adresse email";
      var body = $@"
                <h2>Bonjour {username},</h2>
                <p>Merci de vous être inscrit ! Veuillez confirmer votre adresse email en cliquant sur le lien ci-dessous :</p>
                <p><a href='{confirmationLink}'>Confirmer mon email</a></p>
                <p>Ce lien expirera dans 24 heures.</p>
                <p>Si vous n'avez pas créé de compte, ignorez cet email.</p>
            ";

      await SendEmailAsync(email, subject, body);
    }

    public async Task SendPasswordResetAsync(string email, string username, string resetLink)
    {
      var subject = "Réinitialisation de votre mot de passe";
      var body = $@"
                <h2>Bonjour {username},</h2>
                <p>Vous avez demandé la réinitialisation de votre mot de passe.</p>
                <p><a href='{resetLink}'>Réinitialiser mon mot de passe</a></p>
                <p>Ce lien expirera dans 1 heure.</p>
                <p>Si vous n'avez pas demandé cette réinitialisation, ignorez cet email.</p>
            ";

      await SendEmailAsync(email, subject, body);
    }

    public async Task SendWelcomeEmailAsync(string email, string username)
    {
      var subject = "Bienvenue !";
      var body = $@"
                <h2>Bienvenue {username} !</h2>
                <p>Votre compte a été créé avec succès.</p>
                <p>Vous pouvez maintenant vous connecter et profiter de nos services.</p>
            ";

      await SendEmailAsync(email, subject, body);
    }

    public async Task SendPasswordChangedNotificationAsync(string email, string username)
    {
      var subject = "Votre mot de passe a été modifié";
      var body = $@"
                <h2>Bonjour {username},</h2>
                <p>Votre mot de passe a été modifié avec succès.</p>
                <p>Si vous n'êtes pas à l'origine de ce changement, contactez-nous immédiatement.</p>
            ";

      await SendEmailAsync(email, subject, body);
    }


    public async Task SendEmailUsingResendAsync(string to, string subject, string html)
    {
      var message = new EmailMessage
      {
        From = "magaliperlin237@blandine-mafeu.fr",
        Subject = subject,
        HtmlBody = html
      };

      message.To.Add(to);
      await _resend.EmailSendAsync(message);
    }

    private async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
      try
      {
        if (_settings.UseExternalService) // si d'autres services, on utilisera une chaine de caractere
        {
          await SendEmailUsingResendAsync(to, subject, htmlBody);
          return;
        }
        using var message = new MailMessage
        {
          From = new MailAddress(_settings.FromEmail, _settings.FromName),
          Subject = subject,
          Body = htmlBody,
          IsBodyHtml = true
        };

        message.To.Add(to);

        using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
        {
          Credentials = new NetworkCredential(_settings.SmtpUsername, _settings.SmtpPassword),
          EnableSsl = true
        };

        await client.SendMailAsync(message);
        _logger.LogInformation("Email envoyé à {Email}: {Subject}", to, subject);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erreur lors de l'envoi de l'email à {Email}", to);
        throw;
      }
    }
  }

  public class EmailSettings
  {
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string SmtpUsername { get; set; } = string.Empty;
    public string SmtpPassword { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public bool UseExternalService { get; set; } = false;
  }
}
