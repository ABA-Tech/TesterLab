using Auth.Core.Abstractions;
using Microsoft.Extensions.Options;
using Resend;
using System.Net;
using System.Net.Mail;
using TesterLab.Rappory.Models;

namespace TesterLab.Services
{
  /// <summary>
  /// Service d'envoi d'emails via SMTP ou Resend.
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

    // ─────────────────────────────────────────────
    // Méthodes auth
    // ─────────────────────────────────────────────

    public async Task SendEmailConfirmationAsync(string email, string username, string confirmationLink)
    {
      var subject = "Confirmez votre adresse email";
      var body = $@"
                <h2>Bonjour {username},</h2>
                <p>Merci de vous être inscrit ! Veuillez confirmer votre adresse email en cliquant sur le lien ci-dessous :</p>
                <p><a href='{confirmationLink}'>Confirmer mon email</a></p>
                <p>Ce lien expirera dans 24 heures.</p>
                <p>Si vous n'avez pas créé de compte, ignorez cet email.</p>";
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
                <p>Si vous n'avez pas demandé cette réinitialisation, ignorez cet email.</p>";
      await SendEmailAsync(email, subject, body);
    }

    public async Task SendWelcomeEmailAsync(string email, string username)
    {
      var subject = "Bienvenue !";
      var body = $@"
                <h2>Bienvenue {username} !</h2>
                <p>Votre compte a été créé avec succès.</p>
                <p>Vous pouvez maintenant vous connecter et profiter de nos services.</p>";
      await SendEmailAsync(email, subject, body);
    }

    public async Task SendPasswordChangedNotificationAsync(string email, string username)
    {
      var subject = "Votre mot de passe a été modifié";
      var body = $@"
                <h2>Bonjour {username},</h2>
                <p>Votre mot de passe a été modifié avec succès.</p>
                <p>Si vous n'êtes pas à l'origine de ce changement, contactez-nous immédiatement.</p>";
      await SendEmailAsync(email, subject, body);
    }

    // ─────────────────────────────────────────────
    // Rapport de fin d'exécution
    // ─────────────────────────────────────────────

    public async Task SendTestRunReportAsync(IEnumerable<string> recipients, TestRunReportData r)
    {
      var statusLabel = r.Summary.Status switch
      {
        "Completed" => "✅ Terminé avec succès",
        "Failed" => "❌ Terminé avec des échecs",
        "Cancelled" => "⚠️ Annulé",
        _ => r.Summary.Status
      };

      var statusColor = r.Summary.Status switch
      {
        "Completed" => "#16a34a",
        "Failed" => "#dc2626",
        "Cancelled" => "#d97706",
        _ => "#6b7280"
      };

      var badgeBg = r.Summary.Status switch
      {
        "Completed" => "#dcfce7",
        "Failed" => "#fee2e2",
        "Cancelled" => "#fef3c7",
        _ => "#f3f4f6"
      };

      var duration = r.Duration.HasValue
          ? (r.Duration.Value.TotalSeconds < 60
              ? $"{r.Duration.Value.TotalSeconds:F0} s"
              : $"{(int)r.Duration.Value.TotalMinutes} min {r.Duration.Value.Seconds:D2} s")
          : "—";

      var successRateColor = r.Summary.SuccessRate >= 80 ? "#16a34a"
                           : r.Summary.SuccessRate >= 50 ? "#d97706"
                           : "#dc2626";

      // Tableau des cas de test
      var rowsHtml = string.Join("\n", r.TestCases.Select(tc =>
      {
        var tcColor = tc.Status switch
        {
          "Passed" => "#16a34a",
          "Failed" => "#dc2626",
          "Skipped" => "#d97706",
          _ => "#6b7280"
        };
        var tcBg = tc.Status switch
        {
          "Passed" => "#f0fdf4",
          "Failed" => "#fff1f2",
          "Skipped" => "#fffbeb",
          _ => "#f9fafb"
        };
        var tcLabel = tc.Status switch
        {
          "Passed" => "✅ Passé",
          "Failed" => "❌ Échoué",
          "Skipped" => "⏭️ Ignoré",
          _ => tc.Status
        };
        var tcDuration = tc.DurationSeconds < 1
            ? $"{tc.DurationSeconds * 1000:F0} ms"
            : $"{tc.DurationSeconds:F1} s";

        var errorCell = !string.IsNullOrEmpty(tc.ErrorMessage)
            ? $"<td style='padding:10px 14px;font-size:12px;color:#dc2626;max-width:280px;word-break:break-word;'>{System.Web.HttpUtility.HtmlEncode(tc.ErrorMessage)}</td>"
            : "<td style='padding:10px 14px;color:#9ca3af;font-size:12px;'>—</td>";

        return $@"
                  <tr style='background:{tcBg};border-bottom:1px solid #e5e7eb;'>
                    <td style='padding:10px 14px;font-size:13px;color:#111827;'>{System.Web.HttpUtility.HtmlEncode(tc.TestCaseName)}</td>
                    <td style='padding:10px 14px;'>
                      <span style='display:inline-block;padding:2px 10px;border-radius:9999px;font-size:11px;font-weight:600;color:{tcColor};background:{tcBg};border:1px solid {tcColor};'>{tcLabel}</span>
                    </td>
                    <td style='padding:10px 14px;font-size:12px;color:#6b7280;text-align:right;'>{tcDuration}</td>
                    {errorCell}
                  </tr>";
      }));

      var tableSection = r.TestCases.Any() ? $@"
              <h3 style='margin:32px 0 12px;font-size:15px;color:#374151;font-weight:600;'>Détail par cas de test</h3>
              <table width='100%' cellpadding='0' cellspacing='0' style='border-collapse:collapse;border:1px solid #e5e7eb;border-radius:8px;overflow:hidden;font-family:sans-serif;'>
                <thead>
                  <tr style='background:#f9fafb;border-bottom:2px solid #e5e7eb;'>
                    <th style='padding:10px 14px;text-align:left;font-size:12px;color:#6b7280;font-weight:600;text-transform:uppercase;letter-spacing:.05em;'>Cas de test</th>
                    <th style='padding:10px 14px;text-align:left;font-size:12px;color:#6b7280;font-weight:600;text-transform:uppercase;letter-spacing:.05em;'>Statut</th>
                    <th style='padding:10px 14px;text-align:right;font-size:12px;color:#6b7280;font-weight:600;text-transform:uppercase;letter-spacing:.05em;'>Durée</th>
                    <th style='padding:10px 14px;text-align:left;font-size:12px;color:#6b7280;font-weight:600;text-transform:uppercase;letter-spacing:.05em;'>Erreur</th>
                  </tr>
                </thead>
                <tbody>{rowsHtml}</tbody>
              </table>" : "";

      var html = $@"<!DOCTYPE html>
<html lang='fr'>
<head><meta charset='UTF-8'><meta name='viewport' content='width=device-width,initial-scale=1'></head>
<body style='margin:0;padding:0;background:#f3f4f6;font-family:-apple-system,BlinkMacSystemFont,""Segoe UI"",Roboto,sans-serif;'>
  <table width='100%' cellpadding='0' cellspacing='0' style='background:#f3f4f6;padding:40px 16px;'>
    <tr><td align='center'>
      <table width='620' cellpadding='0' cellspacing='0' style='max-width:620px;width:100%;'>

        <!-- En-tête -->
        <tr>
          <td style='background:#1e293b;border-radius:12px 12px 0 0;padding:28px 32px;'>
            <table width='100%'><tr>
              <td>
                <div style='font-size:11px;color:#94a3b8;text-transform:uppercase;letter-spacing:.1em;margin-bottom:4px;'>TesterLab</div>
                <div style='font-size:20px;font-weight:700;color:#f8fafc;'>Rapport d&rsquo;exécution</div>
              </td>
              <td align='right'>
                <span style='display:inline-block;padding:6px 14px;border-radius:9999px;font-size:12px;font-weight:700;background:{badgeBg};color:{statusColor};'>
                  {statusLabel}
                </span>
              </td>
            </tr></table>
          </td>
        </tr>

        <!-- Corps -->
        <tr>
          <td style='background:#ffffff;border-radius:0 0 12px 12px;padding:32px;'>

            <h2 style='margin:0 0 4px;font-size:18px;color:#111827;'>{System.Web.HttpUtility.HtmlEncode(r.TestRunName)}</h2>
            <p style='margin:0 0 24px;font-size:13px;color:#6b7280;'>
              {System.Web.HttpUtility.HtmlEncode(r.ApplicationName)} &nbsp;·&nbsp;
              {System.Web.HttpUtility.HtmlEncode(r.EnvironmentName)} &nbsp;·&nbsp;
              {System.Web.HttpUtility.HtmlEncode(r.Browser)}
            </p>

            <!-- Métriques principales -->
            <table width='100%' cellpadding='0' cellspacing='0' style='margin-bottom:24px;'>
              <tr>
                <td width='25%' style='padding:0 8px 0 0;'>
                  <div style='background:#f8fafc;border:1px solid #e2e8f0;border-radius:8px;padding:16px;text-align:center;'>
                    <div style='font-size:26px;font-weight:700;color:#111827;'>{r.Summary.TotalTests}</div>
                    <div style='font-size:11px;color:#6b7280;margin-top:2px;text-transform:uppercase;letter-spacing:.05em;'>Total</div>
                  </div>
                </td>
                <td width='25%' style='padding:0 8px;'>
                  <div style='background:#f0fdf4;border:1px solid #bbf7d0;border-radius:8px;padding:16px;text-align:center;'>
                    <div style='font-size:26px;font-weight:700;color:#16a34a;'>{r.Summary.PassedCount}</div>
                    <div style='font-size:11px;color:#16a34a;margin-top:2px;text-transform:uppercase;letter-spacing:.05em;'>Réussis</div>
                  </div>
                </td>
                <td width='25%' style='padding:0 8px;'>
                  <div style='background:#fff1f2;border:1px solid #fecdd3;border-radius:8px;padding:16px;text-align:center;'>
                    <div style='font-size:26px;font-weight:700;color:#dc2626;'>{r.Summary.FailedCount}</div>
                    <div style='font-size:11px;color:#dc2626;margin-top:2px;text-transform:uppercase;letter-spacing:.05em;'>Échoués</div>
                  </div>
                </td>
                <td width='25%' style='padding:0 0 0 8px;'>
                  <div style='background:#fffbeb;border:1px solid #fde68a;border-radius:8px;padding:16px;text-align:center;'>
                    <div style='font-size:26px;font-weight:700;color:#d97706;'>{r.Summary.SkippedCount}</div>
                    <div style='font-size:11px;color:#d97706;margin-top:2px;text-transform:uppercase;letter-spacing:.05em;'>Ignorés</div>
                  </div>
                </td>
              </tr>
            </table>

            <!-- Taux de réussite + durée -->
            <table width='100%' cellpadding='0' cellspacing='0' style='margin-bottom:24px;'>
              <tr>
                <td width='50%' style='padding-right:8px;'>
                  <div style='background:#f8fafc;border:1px solid #e2e8f0;border-radius:8px;padding:14px 18px;'>
                    <div style='font-size:11px;color:#6b7280;text-transform:uppercase;letter-spacing:.05em;margin-bottom:4px;'>Taux de réussite</div>
                    <div style='font-size:22px;font-weight:700;color:{successRateColor};'>{r.Summary.SuccessRate:F1} %</div>
                  </div>
                </td>
                <td width='50%' style='padding-left:8px;'>
                  <div style='background:#f8fafc;border:1px solid #e2e8f0;border-radius:8px;padding:14px 18px;'>
                    <div style='font-size:11px;color:#6b7280;text-transform:uppercase;letter-spacing:.05em;margin-bottom:4px;'>Durée totale</div>
                    <div style='font-size:22px;font-weight:700;color:#111827;'>{duration}</div>
                  </div>
                </td>
              </tr>
            </table>

            <!-- Infos contextuelles -->
            <table width='100%' cellpadding='0' cellspacing='0' style='border:1px solid #e5e7eb;border-radius:8px;overflow:hidden;margin-bottom:8px;'>
              <tr style='background:#f9fafb;'>
                <td style='padding:10px 14px;font-size:12px;color:#6b7280;font-weight:600;width:40%;'>Déclencheur</td>
                <td style='padding:10px 14px;font-size:13px;color:#111827;'>{System.Web.HttpUtility.HtmlEncode(r.Trigger)}</td>
              </tr>
              <tr style='border-top:1px solid #e5e7eb;'>
                <td style='padding:10px 14px;font-size:12px;color:#6b7280;font-weight:600;'>Démarré le</td>
                <td style='padding:10px 14px;font-size:13px;color:#111827;'>{r.StartedAt?.ToLocalTime().ToString("dd/MM/yyyy à HH:mm:ss") ?? "—"}</td>
              </tr>
              <tr style='background:#f9fafb;border-top:1px solid #e5e7eb;'>
                <td style='padding:10px 14px;font-size:12px;color:#6b7280;font-weight:600;'>Terminé le</td>
                <td style='padding:10px 14px;font-size:13px;color:#111827;'>{r.CompletedAt?.ToLocalTime().ToString("dd/MM/yyyy à HH:mm:ss") ?? "—"}</td>
              </tr>
            </table>

            {tableSection}

          </td>
        </tr>

        <!-- Pied de page -->
        <tr>
          <td style='padding:20px 0 0;text-align:center;'>
            <p style='margin:0;font-size:11px;color:#9ca3af;'>
              Ce message a été généré automatiquement par <strong>TesterLab</strong>. Merci de ne pas y répondre.
            </p>
          </td>
        </tr>

      </table>
    </td></tr>
  </table>
</body>
</html>";

      var subject = $"[TesterLab] {statusLabel} — {r.TestRunName} ({r.ApplicationName})";

      foreach (var recipient in recipients)
      {
        var trimmed = recipient.Trim();
        if (string.IsNullOrEmpty(trimmed)) continue;
        await SendEmailAsync(trimmed, subject, html);
      }
    }

    // ─────────────────────────────────────────────
    // Envoi bas niveau
    // ─────────────────────────────────────────────

    private async Task SendEmailUsingResendAsync(string to, string subject, string html)
    {
      var message = new EmailMessage
      {
        From = _settings.FromEmail,
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
        if (_settings.UseExternalService)
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
