using System.ComponentModel.DataAnnotations;
using TesterLab.Domain.Models;

namespace TesterLab.Models
{

  public class ApplicationDto
  {
    public int Id { get; set; }

    [Required, Display(Name = "Nom de l'application")]
    public string Name { get; set; }

    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Display(Name = "URL principale")]
    [Url]
    public string? MainUrl { get; set; }

    [Display(Name = "Type d'application")]
    public string AppType { get; set; } = "Web"; // Web, Mobile, API, Desktop

    //[Display(Name = "Secteur d'activité")]
    //public string? Industry { get; set; } // E-commerce, Banking, Education, etc.

    [Display(Name = "Actif")]
    public bool Active { get; set; } = true;

    //[Display(Name = "Couleur du thème")]
    //public string? ThemeColor { get; set; } = "#007bff";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relations
    public List<Feature> Features { get; set; } = new();
    public List<Domain.Models.Environment> Environments { get; set; } = new();
    public List<TestData> TestDataSets { get; set; } = new();
    public DateTime UpdatedAt { get; set; }
  }
}
