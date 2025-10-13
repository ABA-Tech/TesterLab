using System.ComponentModel.DataAnnotations;
using TesterLab.Domain.Models;

namespace TesterLab.Models
{
  public class FeatureDto
  {
    public int Id { get; set; }

    [Required, Display(Name = "Application")]
    public int ApplicationId { get; set; }

    [Required, Display(Name = "Nom de la fonctionnalité")]
    public string Name { get; set; }

    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Display(Name = "Icône")]
    public string? Icon { get; set; } = "🔧"; // Emoji par défaut

    [Display(Name = "Priorité métier")]
    [Range(1, 5)]
    public int BusinessPriority { get; set; } = 3;

    [Display(Name = "Complexité")]
    public string Complexity { get; set; } = "Medium"; // Low, Medium, High

    [Display(Name = "Actif")]
    public bool Active { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relations
    public Application? Application { get; set; }
    public List<TestCase> TestCases { get; set; } = new();
    public DateTime UpdatedAt { get; set; }
  }

}
