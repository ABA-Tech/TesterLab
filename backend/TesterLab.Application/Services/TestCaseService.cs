using TesterLab.Domain.interfaces.Repositories;
using TesterLab.Domain.interfaces.Services;
using TesterLab.Domain.Models;

namespace TesterLab.Applications.Services
{
    public class TestCaseService : ITestCaseService
    {
        private readonly ITestCaseRepository _testCaseRepository;
        private readonly ITestStepRepository _testStepRepository;
        private readonly IFeatureRepository _featureRepository;

        public TestCaseService(
            ITestCaseRepository testCaseRepository,
            ITestStepRepository testStepRepository,
            IFeatureRepository featureRepository)
        {
            _testCaseRepository = testCaseRepository;
            _testStepRepository = testStepRepository;
            _featureRepository = featureRepository;
        }

        public async Task<IEnumerable<TestCase>> GetTestCasesByFeatureAsync(int featureId)
        {
            return await _testCaseRepository.GetByFeatureIdAsync(featureId);
        }

        public async Task<IEnumerable<TestCase>> GetTestCasesByApplicationAsync(int applicationId)
        {
            return await _testCaseRepository.GetByApplicationIdAsync(applicationId);
        }

        public async Task<TestCase?> GetTestCaseWithStepsAsync(int id)
        {
            return await _testCaseRepository.GetByIdWithStepsAsync(id);
        }

        public async Task<TestCase> CreateTestCaseAsync(TestCase testCase)
        {
            // Validation métier
            if (string.IsNullOrWhiteSpace(testCase.Name))
                throw new ArgumentException("Le nom du scénario est requis");

            if (testCase.Name.Length > 100)
                throw new ArgumentException("Le nom ne peut pas dépasser 100 caractères");

            if (testCase.CriticalityLevel < 1 || testCase.CriticalityLevel > 5)
                throw new ArgumentException("Le niveau de criticité doit être entre 1 et 5");

            if (testCase.EstimatedMinutes <= 0)
                throw new ArgumentException("La durée estimée doit être positive");

            // Vérifier que la feature existe
            var feature = await _featureRepository.GetByIdAsync(testCase.FeatureId);
            if (feature == null)
                throw new ArgumentException("Fonctionnalité non trouvée");

            // Vérifier l'unicité du nom dans la feature
            var existingTests = await _testCaseRepository.GetByFeatureIdAsync(testCase.FeatureId);
            if (existingTests.Any(t => t.Name.ToLower() == testCase.Name.ToLower()))
            {
                throw new InvalidOperationException($"Un scénario avec le nom '{testCase.Name}' existe déjà dans cette fonctionnalité");
            }

            return await _testCaseRepository.CreateAsync(testCase);
        }

        public async Task<TestCase> UpdateTestCaseAsync(TestCase testCase)
        {
            var existing = await _testCaseRepository.GetByIdAsync(testCase.Id);
            if (existing == null)
                throw new ArgumentException("Scénario de test non trouvé");

            // Validation similaire à Create
            if (string.IsNullOrWhiteSpace(testCase.Name))
                throw new ArgumentException("Le nom du scénario est requis");

            if (testCase.CriticalityLevel < 1 || testCase.CriticalityLevel > 5)
                throw new ArgumentException("Le niveau de criticité doit être entre 1 et 5");

            return await _testCaseRepository.UpdateAsync(testCase);
        }

        public async Task<bool> DeleteTestCaseAsync(int id)
        {
            var testCase = await _testCaseRepository.GetByIdAsync(id);
            if (testCase == null)
                throw new ArgumentException("Scénario de test non trouvé");

            return await _testCaseRepository.DeleteAsync(id);
        }

        public async Task<TestCase> DuplicateTestCaseAsync(int id, string newName)
        {
            var original = await _testCaseRepository.GetByIdWithStepsAsync(id);
            if (original == null)
                throw new ArgumentException("Scénario de test non trouvé");

            var duplicate = new TestCase
            {
                Name = newName,
                Description = original.Description + " (Copie)",
                FeatureId = original.FeatureId,
                CriticalityLevel = original.CriticalityLevel,
                ExecutionFrequency = original.ExecutionFrequency,
                Tags = original.Tags,
                EstimatedMinutes = original.EstimatedMinutes,
                UserPersona = original.UserPersona,
                Active = original.Active
            };

            var createdTestCase = await _testCaseRepository.CreateAsync(duplicate);

            // Dupliquer les étapes
            if (original.TestSteps != null && original.TestSteps.Any())
            {
                foreach (var step in original.TestSteps.OrderBy(s => s.Order))
                {
                    var duplicatedStep = new TestStep
                    {
                        TestCaseId = createdTestCase.Id,
                        Action = step.Action,
                        Target = step.Target,
                        Selector = step.Selector,
                        Value = step.Value,
                        Order = step.Order,
                        Description = step.Description,
                        IsOptional = step.IsOptional,
                        TimeoutSeconds = step.TimeoutSeconds
                    };

                    await _testStepRepository.CreateAsync(duplicatedStep);
                }
            }

            return createdTestCase;
        }

        public async Task<IEnumerable<TestCase>> SearchTestCasesAsync(int applicationId, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return await _testCaseRepository.GetByApplicationIdAsync(applicationId);

            var allTests = await _testCaseRepository.GetByApplicationIdAsync(applicationId);

            return allTests.Where(t =>
                t.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                t.Description?.Contains(query, StringComparison.OrdinalIgnoreCase) == true ||
                t.Tags?.Contains(query, StringComparison.OrdinalIgnoreCase) == true ||
                t.UserPersona?.Contains(query, StringComparison.OrdinalIgnoreCase) == true
            );
        }

        public async Task<IEnumerable<TestCase>> GetTestCasesByTagsAsync(int applicationId, string[] tags)
        {
            if (tags == null || !tags.Any())
                return await _testCaseRepository.GetByApplicationIdAsync(applicationId);

            return await _testCaseRepository.GetByTagsAsync(tags);
        }

        public async Task<IEnumerable<TestCase>> GetTestCasesByCriticalityAsync(int applicationId, int minLevel)
        {
            return await _testCaseRepository.GetByCriticalityAsync(minLevel);
        }
    }
}
