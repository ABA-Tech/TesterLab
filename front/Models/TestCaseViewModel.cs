using System.ComponentModel.DataAnnotations;
using TesterLab.Domain.Models;

namespace TesterLab.Models
{
  public class TestCaseViewModel
  {
    public int Id { get; set; }

    [Required, Display(Name = "Nom du scénario")]
    public string Name { get; set; }

    [Display(Name = "Description (en langage naturel)")]
    public string? Description { get; set; }

    [Required, Display(Name = "Fonctionnalité")]
    public int FeatureId { get; set; }

    [Display(Name = "Niveau de criticité")]
    [Range(1, 5)]
    public int CriticalityLevel { get; set; } = 3;

    [Display(Name = "Fréquence d'exécution")]
    public string ExecutionFrequency { get; set; } = "Manual"; // Manual, Daily, Weekly, OnDemand

    [Display(Name = "Tags")]
    public string? Tags { get; set; } // "smoke,regression,critical" séparés par virgules

    [Display(Name = "Durée estimée (minutes)")]
    public int EstimatedMinutes { get; set; } = 2;

    [Display(Name = "Persona utilisateur")]
    public string? UserPersona { get; set; } // "Admin", "Client", "Guest"

    [Display(Name = "Actif")]
    public bool Active { get; set; } = true;

    public bool Selected { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relations
    public FeatureDto? Feature { get; set; }
    public List<TestStepDto> TestSteps { get; set; } = new();
  }
}
