using System.ComponentModel.DataAnnotations;

namespace TesterLab.Models.ViewModels
{
  public class ForgotPasswordViewModel
  {
    [Required(ErrorMessage = "L'email est requis")]
    [EmailAddress(ErrorMessage = "Email invalide")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;
  }
}
