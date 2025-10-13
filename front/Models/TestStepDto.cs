using System.ComponentModel.DataAnnotations;
using TesterLab.Domain.Models;

namespace TesterLab.Models
{
  public class TestStepDto
  {
    public int Id { get; set; }

    public int TestCaseId { get; set; }

    [Required, Display(Name = "Action")]
    public string Action { get; set; } // Valeurs prédéfinies user-friendly

    [Display(Name = "Élément ciblé")]
    public string? Target { get; set; } // Description en langage naturel

    [Display(Name = "Sélecteur technique")]
    public string? Selector { get; set; }

    [Display(Name = "Valeur à saisir")]
    public string? Value { get; set; }

    [Display(Name = "Ordre")]
    public int Order { get; set; }

    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Display(Name = "Optionnel")]
    public bool IsOptional { get; set; } = false;

    [Display(Name = "Délai d'attente (secondes)")]
    public int TimeoutSeconds { get; set; } = 10;

    // Relations
    public TestCaseViewModel? TestCase { get; set; }
  }
}
