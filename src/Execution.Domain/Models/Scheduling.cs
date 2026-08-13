using System.ComponentModel.DataAnnotations;
using Execution.Domain.Common;
using Execution.Domain.Enums;

namespace Execution.Domain.Models;

public class Job : TenantAuditableEntity
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public DateTime NextExecutionTimeUtc { get; set; }

    public int? FrequencyInMinutes { get; set; } // null = exécution unique

    public bool IsRunning { get; set; }
    public bool IsEnabled { get; set; } = true;

    public DateTime? LastExecutionTimeUtc { get; set; }
    public RunStatus? LastExecutionStatus { get; set; }

    public int ConsecutiveFailures { get; set; }

    // Désactivation automatique après N échecs consécutifs —
    // évite qu'un job cassé spamme des emails d'échec indéfiniment
    public int MaxConsecutiveFailuresBeforeAutoDisable { get; set; } = 5;

    [Required]
    public int TestCaseId { get; set; }
    public TestCase? TestCase { get; set; }

    [Required]
    public int EnvironmentId { get; set; }
    public TestEnvironment? Environment { get; set; }

    // ── Historique — la vraie amélioration pro ──
    public List<JobExecutionHistory> History { get; set; } = new();
}

// Chaque déclenchement du job crée une ligne ici,
// reliée au TestRun réellement produit
public class JobExecutionHistory
{
    public long Id { get; set; }

    [Required]
    public int JobId { get; set; }
    public Job? Job { get; set; }

    [Required]
    public DateTime TriggeredAtUtc { get; set; } = DateTime.UtcNow;

    public int? TestRunId { get; set; } // le run réellement créé
    public TestRun? TestRun { get; set; }

    public RunStatus Status { get; set; }

    [MaxLength(1000)]
    public string? ErrorMessage { get; set; }
}