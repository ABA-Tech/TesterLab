
namespace TesterLab.Domain.interfaces.Services
{
    using TesterLab.Domain.DTOs;

    public interface ITestStepImportService
    {
        Task<TestStepImportResultDto> ImportFromRecordedActionsAsync(
            int testCaseId, 
            List<RecordedActionDto> recordedActions, 
            bool replaceExisting = false);
        
        Task<TestStepImportResultDto> ImportFromJsonAsync(
            int testCaseId, 
            string jsonContent, 
            bool replaceExisting = false);
        
        List<TestStepImportDto> ConvertRecordedActionsToTestSteps(
            List<RecordedActionDto> recordedActions);
    }
}
