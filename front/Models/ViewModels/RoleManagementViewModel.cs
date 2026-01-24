using System.ComponentModel.DataAnnotations;

namespace TesterLab.Models.ViewModels
{
  public class RoleManagementViewModel
  {
    public string? Id { get; set; }

    [Required(ErrorMessage = "Le nom du rôle est requis")]
    [StringLength(50, ErrorMessage = "Le nom ne peut pas dépasser 50 caractères")]
    [Display(Name = "Nom du rôle")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères")]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    public int UserCount { get; set; }
    public DateTime CreatedAt { get; set; }
  }
}
