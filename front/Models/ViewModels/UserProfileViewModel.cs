namespace TesterLab.Models.ViewModels
{
  public class UserProfileViewModel
  {
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool EmailConfirmed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public List<string> Roles { get; set; } = new();

    public string FullName => $"{FirstName} {LastName}".Trim();
    public string Initials
    {
      get
      {
        if (!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName))
          return $"{FirstName[0]}{LastName[0]}".ToUpper();

        return Username.Length >= 2
            ? Username.Substring(0, 2).ToUpper()
            : Username[0].ToString().ToUpper();
      }
    }
  }
}
