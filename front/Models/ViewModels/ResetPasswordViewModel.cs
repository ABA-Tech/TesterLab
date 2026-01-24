using System.ComponentModel.DataAnnotations;

namespace TesterLab.Models.ViewModels
{
  public class ResetPasswordViewModel
  {
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le mot de passe est requis")]
    [StringLength(100, MinimumLength = 12, ErrorMessage = "Le mot de passe doit contenir au moins 12 caractères")]
    [DataType(DataType.Password)]
    [Display(Name = "Nouveau mot de passe")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "La confirmation est requise")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas")]
    [Display(Name = "Confirmer le mot de passe")]
    public string ConfirmPassword { get; set; } = string.Empty;
  }
}
