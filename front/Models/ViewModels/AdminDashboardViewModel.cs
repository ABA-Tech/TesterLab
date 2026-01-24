namespace TesterLab.Models.ViewModels
{
  public class AdminDashboardViewModel
  {
    public AdminStatistics Statistics { get; set; } = new();
    public List<RecentUserViewModel> RecentUsers { get; set; } = new();
    public List<RoleSummaryViewModel> RoleSummaries { get; set; } = new();
  }

  public class AdminStatistics
  {
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int NewUsersThisWeek { get; set; }
    public int PendingEmailConfirmation { get; set; }
    public int LockedOutUsers { get; set; }
    public int TotalRoles { get; set; }

    public double ActiveUsersPercentage => TotalUsers > 0
        ? Math.Round((double)ActiveUsers / TotalUsers * 100, 1)
        : 0;
  }

  public class RecentUserViewModel
  {
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool EmailConfirmed { get; set; }
    public string TimeAgo
    {
      get
      {
        var diff = DateTime.UtcNow - CreatedAt;
        if (diff.TotalMinutes < 60)
          return $"Il y a {(int)diff.TotalMinutes} minute(s)";
        if (diff.TotalHours < 24)
          return $"Il y a {(int)diff.TotalHours} heure(s)";
        return $"Il y a {(int)diff.TotalDays} jour(s)";
      }
    }
  }

  public class RoleSummaryViewModel
  {
    public string RoleName { get; set; } = string.Empty;
    public int UserCount { get; set; }
    public string ColorClass { get; set; } = "primary";
  }
}
