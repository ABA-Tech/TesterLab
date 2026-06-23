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
                if (!await _testStepRepository.TestCaseExistsAsync(testCaseId))
                {
                    result.Success = false;
                    result.Message = "Le test case spécifié n'existe pas.";
                    return result;
                }

                var recordedActions = JsonSerializer.Deserialize<List<RecordedActionDto>>(
                    jsonContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (recordedActions == null || !recordedActions.Any())
                {
                    result.Success = false;
                    result.Message = "Aucune action trouvée dans le JSON.";
                    return result;
                }

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
                if (!await _testStepRepository.TestCaseExistsAsync(testCaseId))
                {
                    result.Success = false;
                    result.Message = "Le test case spécifié n'existe pas.";
                    return result;
                }

                if (replaceExisting)
                {
                    var existingSteps = await _testStepRepository.GetByTestCaseIdAsync(testCaseId);
                    if (existingSteps.Any())
                        await _testStepRepository.DeleteRangeAsync(existingSteps);
                }

                int startOrder = replaceExisting ? 1 :
                    await _testStepRepository.GetMaxOrderByTestCaseIdAsync(testCaseId) + 1;

                var testStepDtos = ConvertRecordedActionsToTestSteps(recordedActions);

                var testSteps = testStepDtos.Select((dto, i) => new TestStep
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
                }).ToList();

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

        /// <summary>
        /// Convertit les actions enregistrées en étapes de test.
        /// Règles appliquées :
        ///  - Un "hover" immédiatement suivi d'un "click" sur le MÊME sélecteur est redondant
        ///    (le clic Selenium déclenche déjà le survol) et est supprimé.
        ///  - Tout autre "hover" est conservé et génère une étape "hover" (ex: ouverture
        ///    d'un menu/dropdown qui doit rester ouvert pour l'action suivante).
        ///  - Pour les saisies successives sur le même sélecteur (type "value"),
        ///    seul le dernier événement est conservé (valeur finale saisie).
        ///  - "value: unchecked" sur une checkbox génère une action "uncheck".
        ///  - "change" sur un SELECT génère une action "select".
        /// </summary>
        public List<TestStepImportDto> ConvertRecordedActionsToTestSteps(
            List<RecordedActionDto> recordedActions)
        {
            // 1. Trier par sequenceNumber (ou timestamp en fallback)
            var sorted = recordedActions
                .OrderBy(a => a.SequenceNumber > 0 ? a.SequenceNumber : (int)(a.Timestamp & int.MaxValue))
                .ToList();

            // 2. Supprimer uniquement les hovers redondants :
            //    un "hover" immédiatement suivi d'un "click" sur le même sélecteur.
            var withoutRedundantHovers = new List<RecordedActionDto>();
            for (int i = 0; i < sorted.Count; i++)
            {
                var current = sorted[i];
                bool isHover = string.Equals(current.Type, "hover", StringComparison.OrdinalIgnoreCase);

                if (isHover)
                {
                    var next = i + 1 < sorted.Count ? sorted[i + 1] : null;
                    bool followedByClickOnSameElement = next != null
                        && string.Equals(next.Type, "click", StringComparison.OrdinalIgnoreCase)
                        && next.Selector == current.Selector;

                    if (followedByClickOnSameElement)
                        continue; // hover redondant, le click suffit
                }

                withoutRedundantHovers.Add(current);
            }
            sorted = withoutRedundantHovers;

            // 3. Dédupliquer les saisies progressives :
            //    Pour des événements "value" consécutifs sur le même sélecteur, ne garder que le dernier.
            var deduped = new List<RecordedActionDto>();
            for (int i = 0; i < sorted.Count; i++)
            {
                var current = sorted[i];
                bool isTypingEvent = string.Equals(current.Type, "value", StringComparison.OrdinalIgnoreCase);

                if (isTypingEvent)
                {
                    // Regarder en avant : s'il existe un autre "value" sur le même sélecteur juste après, on saute celui-ci
                    bool hasNextSame = i + 1 < sorted.Count
                        && string.Equals(sorted[i + 1].Type, "value", StringComparison.OrdinalIgnoreCase)
                        && sorted[i + 1].Selector == current.Selector;

                    if (hasNextSame)
                        continue; // ignorer cet événement intermédiaire
                }

                deduped.Add(current);
            }

            // 4. Construire les TestStepImportDto
            var testSteps = new List<TestStepImportDto>();
            int order = 1;

            foreach (var action in deduped)
            {
                var mappedAction = MapActionType(action.Type, action.TagName, action.Value);

                // Ignorer les clics "parasites" sur une checkbox qui a déjà un "value" (doublon click+value)
                if (string.Equals(action.Type, "click", StringComparison.OrdinalIgnoreCase)
                    && string.Equals(action.TagName, "INPUT", StringComparison.OrdinalIgnoreCase))
                {
                    // Si l'event précédent ou suivant est un "value" sur le même sélecteur, ignorer ce click
                    bool hasPairedValue = deduped.Any(a =>
                        a != action
                        && string.Equals(a.Type, "value", StringComparison.OrdinalIgnoreCase)
                        && a.Selector == action.Selector);
                    if (hasPairedValue)
                        continue;
                }

                if (string.IsNullOrWhiteSpace(action.Type) || action.Type.ToLower() == "hover")
                {
                    continue;
                }

                var selector = action.Selector ?? action.Xpath ?? "";
                var xpath = action.Xpath ?? action.Selector ?? "";
                var value = action.Value ?? "";

                var step = new TestStepImportDto
                {
                    Order = order++,
                    Action = mappedAction,
                    Target = selector,
                    Selector = selector,
                    Xpath = xpath,
                    Value = value,
                    Text = action.Text ?? "",
                    TimeoutSeconds = 30,
                    IsOptional = false,
                    Description = GenerateDescription(mappedAction, action)
                };

                testSteps.Add(step);
            }

            return testSteps;
        }

        private string MapActionType(string? type, string? tagName, string? value)
        {
            return type?.ToLower() switch
            {
                "click"   => "click",
                "change"  => "select",
                "input"   => "type",
                "type"    => "type",
                "keypress" => "type",
                "value"   => string.Equals(value, "unchecked", StringComparison.OrdinalIgnoreCase) ? "uncheck"
                           : string.Equals(value, "checked",   StringComparison.OrdinalIgnoreCase) ? "check"
                           : "type",
                "navigate" => "navigate",
                "submit"   => "click",
                "wait"     => "wait",
                "verify_text"    => "assert",
                "verify_value"    => "assert",
                "verify_enabled" => "assert_enabled",
                "assert_enabled" => "assert_enabled",
                // "hover" ne doit pas arriver ici (filtré en amont), mais sécurité :
                "hover"   => "hover",
                _         => "click"
            };
        }

        private string GenerateDescription(string action, RecordedActionDto recordedAction)
        {
            var label = string.IsNullOrEmpty(recordedAction.Text)
                ? (recordedAction.Locators?.Text ?? "")
                : recordedAction.Text;
            label = label.Length > 60 ? label.Substring(0, 60) + "…" : label;

            return action.ToLower() switch
            {
                "click"   => !string.IsNullOrEmpty(label) ? $"Cliquer sur \"{label}\"" : "Cliquer sur l'élément",
                "type"    => !string.IsNullOrEmpty(recordedAction.Value) ? $"Saisir \"{recordedAction.Value}\"" : "Saisir du texte",
                "select"  => !string.IsNullOrEmpty(recordedAction.Value) ? $"Sélectionner la valeur \"{recordedAction.Value}\"" : "Sélectionner une option",
                "check"   => $"Cocher la case",
                "uncheck" => $"Décocher la case",
                "navigate"=> !string.IsNullOrEmpty(recordedAction.Value) ? $"Naviguer vers {recordedAction.Value}" : "Naviguer vers la page",
                "wait"    => "Attendre",
                "assert"  => !string.IsNullOrEmpty(label) ? $"Vérifier que \"{label}\" est présent" : "Vérifier le texte",
                "assert_enabled" => "Vérifier que l'élément est actif",
                "hover"   => !string.IsNullOrEmpty(label) ? $"Survoler \"{label}\"" : "Survoler l'élément",
                _ => $"Exécuter l'action {action}"
            };
        }
    }
}