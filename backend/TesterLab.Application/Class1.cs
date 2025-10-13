using Microsoft.Extensions.DependencyInjection;
using TesterLab.Applications.Services;
using TesterLab.Domain.interfaces.Services;

namespace TesterLab.Applications
{
  public static class Class1
  {
    public static IServiceCollection AddServiceDpendencyInjection(this IServiceCollection services)
    {
      services.AddScoped<IApplicationService, ApplicationService>();
      services.AddScoped<ITestExecutionService, TestExecutionService>();
      services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));
      return services;
    }
  }
}
