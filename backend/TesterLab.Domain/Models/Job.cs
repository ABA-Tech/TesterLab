using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesterLab.Domain.Models
{

    public class Job
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        // Heure de la prochaine exécution (en UTC pour éviter les problèmes de fuseaux horaires)
        [Required]
        public DateTime NextExecutionTimeUtc { get; set; }

        // Fréquence en minutes (null = exécution unique)
        public int? FrequencyInMinutes { get; set; }

        // Indique si le job est actuellement en cours d'exécution
        public bool IsRunning { get; set; }

        // Indique si le job est activé
        public bool IsEnabled { get; set; } = true;

        // Dernière exécution
        public DateTime? LastExecutionTimeUtc { get; set; }

        // Statut de la dernière exécution
        public string? LastExecutionStatus { get; set; }

        // Nombre d'échecs consécutifs
        public int ConsecutiveFailures { get; set; }

        // Date de création
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        // Date de dernière modification
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        public int TestCaseId { get; set; }
        public int EnvironmentId { get; set; }
        public string? CreatedByUserId { get; set; }
    }
}
