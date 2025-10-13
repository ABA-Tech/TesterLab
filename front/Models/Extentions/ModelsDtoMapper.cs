using TesterLab.Domain.Models;

namespace TesterLab.Models.Extentions
{
  public static class ModelsDtoMapper
  {
    public static TestStepDto ToDto(this TestStep model)
    {
      return new TestStepDto
      {
        Value = model.Value,
        TimeoutSeconds = model.TimeoutSeconds,
        Target = model.Target,
        Selector = model.Selector,
        Action = model.Action,
        Description = model.Description,
        Id = model.Id,
        IsOptional = model.IsOptional,
        Order = model.Order,
        TestCaseId = model.TestCaseId,
      };
    }

    public static TestStep ToModel(this TestStepDto model)
    {
      return new TestStep
      {
        Value = model.Value,
        TimeoutSeconds = model.TimeoutSeconds,
        Target = model.Target,
        Selector = model.Selector,
        Action = model.Action,
        Description = model.Description,
        Id = model.Id,
        IsOptional = model.IsOptional,
        Order = model.Order,
        TestCaseId = model.TestCaseId,
      };
    }

    public static IEnumerable<TestStep> ToModelCollection(this IEnumerable<TestStepDto> testSteps)
    {
      return testSteps.Select(x=>x.ToModel()).ToList();
    }
    public static IEnumerable<TestStepDto> ToDtoCollection(this IEnumerable<TestStep> testSteps)
    {
      return testSteps.Select(x=>x.ToDto()).ToList();
    }
  }
}
