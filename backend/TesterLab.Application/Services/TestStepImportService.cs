
namespace TesterLab.Applications.Services
{
    using System.Text.Json;
    using TesterLab.Domain.DTOs;
    using TesterLab.Domain.interfaces.Repositories;
    using TesterLab.Domain.interfaces.Services;
    using TesterLab.Domain.Models;

    public class TestStepImportService : ITestStepImportService
    {
        private readonly ITestStepRepository _testStepRepository;

        public TestStepImportService(ITestStepRepository testStepRepository)
        {
            _testStepRepository = testStepRepository;
        }

        public async Task<TestStepImportResultDto> ImportFromJsonAsync(
            int testCaseId, 
            string jsonContent, 
            bool replaceExisting = false)
        {
            var result = new TestStepImportResultDto();

            try
            {
                // Vérifier que le test case existe
                if (!await _testStepRepository.TestCaseExistsAsync(testCaseId))
                {
                    result.Success = false;
                    result.Message = "Le test case spécifié n'existe pas.";
                    return result;
                }

                // Parser le JSON
                var recordedActions = JsonSerializer.Deserialize<List<RecordedActionDto>>(
                    jsonContent, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (recordedActions == null || !recordedActions.Any())
                {
                    result.Success = false;
                    result.Message = "Aucune action trouvée dans le JSON.";
                    return result;
                }

                // Convertir et importer
                return await ImportFromRecordedActionsAsync(testCaseId, recordedActions, replaceExisting);
            }
            catch (JsonException ex)
            {
                result.Success = false;
                result.Message = $"Erreur de parsing JSON : {ex.Message}";
                result.Errors.Add(ex.Message);
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Erreur lors de l'importation : {ex.Message}";
                result.Errors.Add(ex.Message);
                return result;
            }
        }

        public async Task<TestStepImportResultDto> ImportFromRecordedActionsAsync(
            int testCaseId, 
            List<RecordedActionDto> recordedActions, 
            bool replaceExisting = false)
        {
            var result = new TestStepImportResultDto();

            try
            {
                // Vérifier que le test case existe
                if (!await _testStepRepository.TestCaseExistsAsync(testCaseId))
                {
                    result.Success = false;
                    result.Message = "Le test case spécifié n'existe pas.";
                    return result;
                }

                // Supprimer les étapes existantes si demandé
                if (replaceExisting)
                {
                    var existingSteps = await _testStepRepository.GetByTestCaseIdAsync(testCaseId);
                    if (existingSteps.Any())
                    {
                        await _testStepRepository.DeleteRangeAsync(existingSteps);
                    }
                }

                // Déterminer l'ordre de départ
                int startOrder = replaceExisting ? 1 : 
                    await _testStepRepository.GetMaxOrderByTestCaseIdAsync(testCaseId) + 1;

                // Convertir les actions enregistrées en test steps
                var testStepDtos = ConvertRecordedActionsToTestSteps(recordedActions);

                // Créer les entités TestStep
                var testSteps = new List<TestStep>();
                foreach (var dto in testStepDtos)
                {
                    var testStep = new TestStep
                    {
                        TestCaseId = testCaseId,
                        Order = startOrder + dto.Order - 1,
                        Action = dto.Action,
                        Target = dto.Target,
                        Value = dto.Value,
                        Description = dto.Description,
                        TimeoutSeconds = dto.TimeoutSeconds,
                        IsOptional = dto.IsOptional,
                        Selector = dto.Xpath
                    };
                    testSteps.Add(testStep);
                }

                // Enregistrer en base
                await _testStepRepository.AddRangeAsync(testSteps);

                result.Success = true;
                result.ImportedCount = testSteps.Count;
                result.Message = $"{testSteps.Count} étape(s) importée(s) avec succès.";

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Erreur lors de l'importation : {ex.Message}";
                result.Errors.Add(ex.Message);
                return result;
            }
        }

        public List<TestStepImportDto> ConvertRecordedActionsToTestSteps(
            List<RecordedActionDto> recordedActions)
        {
            var testSteps = new List<TestStepImportDto>();
            int order = 1;

            foreach (var action in recordedActions)
            {
                var testStep = new TestStepImportDto
                {
                    Order = order++,
                    Action = MapActionType(action.Type),
                    Target = action.Selector ?? "",
                    Value = action.Value ?? action.ExpectedValue ?? action.ActualValue ?? "",
                    Xpath = action.Xpath ?? action.Selector ?? "",
                    Description = action.Description,
                    Selector = action.Selector,
                    TimeoutSeconds = 30,
                    IsOptional = false
                };

                // Générer une description
                //testStep.Description = GenerateDescription(testStep.Action, action);

                testSteps.Add(testStep);
            }

            return testSteps;
        }

        private string MapActionType(string type)
        {
            return type?.ToLower() switch
            {
                "click" => "click",
                "change" => "type",
                "input" => "type",
                "hover" => "hover",
                "type" => "type",
                "keypress" => "type",
                "navigate" => "navigate",
                "submit" => "click",
                "wait" => "wait",
                "verify_text" => "assert",
                "verify_enabled" => "assert_enabled",
                "assert_enabled" => "assert_enabled",
                _ => "Click"
            };
        }

        private string GenerateDescription(string action, RecordedActionDto recordedAction)
        {
            return action switch
            {
                "Click" => !string.IsNullOrEmpty(recordedAction.Text) 
                    ? $"Cliquer sur \"{recordedAction.Text}\""
                    : "Cliquer sur l'élément",
                
                "Hover" => !string.IsNullOrEmpty(recordedAction.Text) 
                    ? $"Survoler \"{recordedAction.Text}\""
                    : "Passer la souris sur l'élément",
                
                "Type" => !string.IsNullOrEmpty(recordedAction.Value)
                    ? $"Saisir \"{recordedAction.Value}\" dans le champ"
                    : "Saisir du texte",
                
                "Navigate" => !string.IsNullOrEmpty(recordedAction.Value)
                    ? $"Naviguer vers {recordedAction.Value}"
                    : "Naviguer vers la page",
                
                "Wait" => "Attendre",
                
                _ => $"Exécuter l'action {action}"
            };
        }
    }
}
