using System.ComponentModel.DataAnnotations;
using TesterLab.Domain.Models;
using Environment = TesterLab.Domain.Models.Environment;

namespace TesterLab.Models
{

  public class TestDataDto
  {
    public int Id { get; set; }

    [Required, Display(Name = "Application")]
    public int ApplicationId { get; set; }

    [Required, Display(Name = "Nom du jeu de données")]
    public string Name { get; set; }

    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Display(Name = "Type de données")]
    public string DataType { get; set; } = "UserAccount"; // UserAccount, Product, Payment, etc.

    [Display(Name = "Données (format simple)")]
    public string DataJson { get; set; } = "{}";

    [Display(Name = "Modèle/Template")]
    public bool IsTemplate { get; set; } = false;

    [Display(Name = "Environnement spécifique")]
    public int? SpecificEnvironmentId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relations
    public ApplicationDto? Application { get; set; }
    public Environment? SpecificEnvironment { get; set; }
  }
}
