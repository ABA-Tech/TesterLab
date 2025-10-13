namespace TesterLab.Models
{
  public class EnvironmentDto
  {
    public int Id { get; set; }
    public int ApplicationId { get; set; }
    public string Name { get; set; }
    public string BaseUrl { get; set; }
    public string Type { get; set; } = "Staging";
    public string? Description { get; set; }
    public bool RequiresAuth { get; set; } = false;
    public string? AccessInfo { get; set; }
    public bool Active { get; set; } = true;
  }
}
