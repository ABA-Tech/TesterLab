using System.Text.Json;
using TesterLab.Domain.interfaces.Repositories;
using TesterLab.Domain.interfaces.Services;
using TesterLab.Domain.Models;

namespace TesterLab.Applications.Services
{
    public class TestDataService : ITestDataService
    {
        private readonly ITestDataRepository _testDataRepository;
        private readonly IApplicationRepository _applicationRepository;

        public TestDataService(
            ITestDataRepository testDataRepository,
            IApplicationRepository applicationRepository)
        {
            _testDataRepository = testDataRepository;
            _applicationRepository = applicationRepository;
        }

        public async Task<IEnumerable<TestData>> GetTestDataByApplicationAsync(int applicationId)
        {
            return await _testDataRepository.GetByApplicationIdAsync(applicationId);
        }

        public async Task<TestData?> GetTestDataByIdAsync(int id)
        {
            return await _testDataRepository.GetByIdAsync(id);
        }

        public async Task<TestData> CreateTestDataAsync(TestData testData)
        {
            // Validation métier
            if (string.IsNullOrWhiteSpace(testData.Name))
                throw new ArgumentException("Le nom du jeu de données est requis");

            if (testData.Name.Length > 100)
                throw new ArgumentException("Le nom ne peut pas dépasser 100 caractères");

            // Valider le JSON
            if (!await ValidateJsonDataAsync(testData.DataJson))
                throw new ArgumentException("Format JSON invalide");

            // Vérifier que l'application existe
            var application = await _applicationRepository.GetByIdAsync(testData.ApplicationId);
            if (application == null)
                throw new ArgumentException("Application non trouvée");

            // Vérifier l'unicité du nom dans l'application
            var existingData = await _testDataRepository.GetByApplicationIdAsync(testData.ApplicationId);
            if (existingData.Any(d => d.Name.ToLower() == testData.Name.ToLower()))
            {
                throw new InvalidOperationException($"Un jeu de données avec le nom '{testData.Name}' existe déjà dans cette application");
            }

            return await _testDataRepository.CreateAsync(testData);
        }

        public async Task<TestData> UpdateTestDataAsync(TestData testData)
        {
            var existing = await _testDataRepository.GetByIdAsync(testData.Id);
            if (existing == null)
                throw new ArgumentException("Jeu de données non trouvé");

            // Validation similaire à Create
            if (string.IsNullOrWhiteSpace(testData.Name))
                throw new ArgumentException("Le nom du jeu de données est requis");

            if (!await ValidateJsonDataAsync(testData.DataJson))
                throw new ArgumentException("Format JSON invalide");

            return await _testDataRepository.UpdateAsync(testData);
        }

        public async Task<bool> DeleteTestDataAsync(int id)
        {
            var testData = await _testDataRepository.GetByIdAsync(id);
            if (testData == null)
                throw new ArgumentException("Jeu de données non trouvé");

            return await _testDataRepository.DeleteAsync(id);
        }

        public async Task<bool> ValidateJsonDataAsync(string jsonData)
        {
            if (string.IsNullOrWhiteSpace(jsonData))
                return true; // JSON vide est valide

            try
            {
                JsonDocument.Parse(jsonData);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }

        public async Task<TestData> CreateFromTemplateAsync(int templateId, string name, Dictionary<string, string> values)
        {
            var template = await _testDataRepository.GetByIdAsync(templateId);
            if (template == null || !template.IsTemplate)
                throw new ArgumentException("Template non trouvé");

            // Parser le template JSON
            var templateData = JsonDocument.Parse(template.DataJson);
            var newData = new Dictionary<string, object>();

            // Remplacer les valeurs du template
            foreach (var element in templateData.RootElement.EnumerateObject())
            {
                if (values.ContainsKey(element.Name))
                {
                    newData[element.Name] = values[element.Name];
                }
                else
                {
                    newData[element.Name] = element.Value.ToString();
                }
            }

            var newTestData = new TestData
            {
                ApplicationId = template.ApplicationId,
                Name = name,
                Description = $"Créé à partir du template: {template.Name}",
                DataType = template.DataType,
                DataJson = JsonSerializer.Serialize(newData),
                IsTemplate = false
            };

            return await CreateTestDataAsync(newTestData);
        }

        public async Task<Dictionary<string, object>> ParseTestDataAsync(int id)
        {
            var testData = await _testDataRepository.GetByIdAsync(id);
            if (testData == null)
                throw new ArgumentException("Jeu de données non trouvé");

            try
            {
                var jsonDocument = JsonDocument.Parse(testData.DataJson);
                var result = new Dictionary<string, object>();

                foreach (var element in jsonDocument.RootElement.EnumerateObject())
                {
                    result[element.Name] = element.Value.ToString();
                }

                return result;
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Erreur lors du parsing JSON: {ex.Message}");
            }
        }

        public async Task<IEnumerable<TestData>> GetTemplatesAsync()
        {
            return await _testDataRepository.GetTemplatesAsync();
        }

        public async Task<TestData> ConvertToTemplateAsync(int id)
        {
            var testData = await _testDataRepository.GetByIdAsync(id);
            if (testData == null)
                throw new ArgumentException("Jeu de données non trouvé");

            testData.IsTemplate = true;
            testData.Name += " (Template)";
            testData.Description = $"Template créé à partir de: {testData.Description}";

            return await _testDataRepository.UpdateAsync(testData);
        }
    }
}
