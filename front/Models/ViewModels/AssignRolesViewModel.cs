using System.ComponentModel.DataAnnotations;

namespace TesterLab.Models.ViewModels
{
  public class AssignRolesViewModel
  {
    [Required]
    public string UserId { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public List<RoleSelectionItem> AvailableRoles { get; set; } = new();
  }

  public class RoleSelectionItem
  {
    public string RoleId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSelected { get; set; }
  }
}
