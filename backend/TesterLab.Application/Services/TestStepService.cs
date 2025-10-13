using TesterLab.Domain.interfaces.Repositories;
using TesterLab.Domain.interfaces.Services;
using TesterLab.Domain.Models;

namespace TesterLab.Applications.Services
{
    public class TestStepService : GenericService<TestStep>, ITestStepService
    {
        private readonly ITestStepRepository _testStepRepository;
        private readonly ITestCaseRepository _testCaseRepository;

        public TestStepService(
            IGenericRepository<TestStep> genericRepository,
            ITestStepRepository testStepRepository,
            ITestCaseRepository testCaseRepository): base(genericRepository)
        {
            _testStepRepository = testStepRepository;
            _testCaseRepository = testCaseRepository;
        }

        public async Task<IEnumerable<TestStep>> GetStepsByTestCaseAsync(int testCaseId)
        {
            return await _testStepRepository.GetByTestCaseIdAsync(testCaseId);
        }

        public async Task<TestStep> CreateStepAsync(TestStep testStep)
        {
            // Validation métier
            if (string.IsNullOrWhiteSpace(testStep.Action))
                throw new ArgumentException("L'action est requise");

            if (testStep.TimeoutSeconds <= 0)
                testStep.TimeoutSeconds = 10; // Valeur par défaut

            // Vérifier que le test case existe
            var testCase = await _testCaseRepository.GetByIdAsync(testStep.TestCaseId);
            if (testCase == null)
                throw new ArgumentException("Scénario de test non trouvé");

            // Si l'ordre n'est pas spécifié, mettre à la fin
            if (testStep.Order <= 0)
            {
                var existingSteps = await _testStepRepository.GetByTestCaseIdAsync(testStep.TestCaseId);
                testStep.Order = existingSteps.Any() ? existingSteps.Max(s => s.Order) + 1 : 1;
            }

            return await _testStepRepository.CreateAsync(testStep);
        }

        public async Task<TestStep> UpdateStepAsync(TestStep testStep)
        {
            var existing = await _testStepRepository.GetByIdAsync(testStep.Id);
            if (existing == null)
                throw new ArgumentException("Étape non trouvée");

            // Validation similaire à Create
            if (string.IsNullOrWhiteSpace(testStep.Action))
                throw new ArgumentException("L'action est requise");

            if (testStep.TimeoutSeconds <= 0)
                testStep.TimeoutSeconds = 10;

            return await _testStepRepository.UpdateAsync(testStep);
        }

        public async Task<bool> DeleteStepAsync(int id)
        {
            var step = await _testStepRepository.GetByIdAsync(id);
            if (step == null)
                throw new ArgumentException("Étape non trouvée");

            var deleted = await _testStepRepository.DeleteAsync(id);

            if (deleted)
            {
                // Réorganiser les ordres des étapes restantes
                await ReorderStepsAfterDeleteAsync(step.TestCaseId, step.Order);
            }

            return deleted;
        }

        public async Task<bool> ReorderStepsAsync(int testCaseId, List<int> stepIds)
        {
            var steps = await _testStepRepository.GetByTestCaseIdAsync(testCaseId);
            var stepDict = steps.ToDictionary(s => s.Id);

            var newOrders = new Dictionary<int, int>();

            for (int i = 0; i < stepIds.Count; i++)
            {
                if (stepDict.ContainsKey(stepIds[i]))
                {
                    newOrders[stepIds[i]] = i + 1;
                }
            }

            return await _testStepRepository.ReorderStepsAsync(testCaseId, newOrders);
        }

        public async Task<TestStep> InsertStepAsync(TestStep testStep, int afterStepId)
        {
            var afterStep = await _testStepRepository.GetByIdAsync(afterStepId);
            if (afterStep == null)
                throw new ArgumentException("Étape de référence non trouvée");

            testStep.TestCaseId = afterStep.TestCaseId;

            // Décaler les étapes suivantes
            var existingSteps = await _testStepRepository.GetByTestCaseIdAsync(testStep.TestCaseId);
            var stepsToUpdate = existingSteps.Where(s => s.Order > afterStep.Order).ToList();

            foreach (var step in stepsToUpdate)
            {
                step.Order++;
                await _testStepRepository.UpdateAsync(step);
            }

            // Insérer la nouvelle étape
            testStep.Order = afterStep.Order + 1;
            return await _testStepRepository.CreateAsync(testStep);
        }

        public async Task<TestStep> MoveStepAsync(int stepId, int newOrder)
        {
            var step = await _testStepRepository.GetByIdAsync(stepId);
            if (step == null)
                throw new ArgumentException("Étape non trouvée");

            var allSteps = await _testStepRepository.GetByTestCaseIdAsync(step.TestCaseId);
            var maxOrder = allSteps.Max(s => s.Order);

            if (newOrder < 1) newOrder = 1;
            if (newOrder > maxOrder) newOrder = maxOrder;

            var oldOrder = step.Order;

            if (oldOrder == newOrder) return step;

            // Réorganiser les autres étapes
            var stepsToUpdate = allSteps.Where(s => s.Id != stepId).ToList();

            if (newOrder < oldOrder)
            {
                // Déplacer vers le haut - décaler les étapes entre newOrder et oldOrder
                foreach (var s in stepsToUpdate.Where(s => s.Order >= newOrder && s.Order < oldOrder))
                {
                    s.Order++;
                    await _testStepRepository.UpdateAsync(s);
                }
            }
            else
            {
                // Déplacer vers le bas - décaler les étapes entre oldOrder et newOrder
                foreach (var s in stepsToUpdate.Where(s => s.Order > oldOrder && s.Order <= newOrder))
                {
                    s.Order--;
                    await _testStepRepository.UpdateAsync(s);
                }
            }

            // Mettre à jour l'étape déplacée
            step.Order = newOrder;
            return await _testStepRepository.UpdateAsync(step);
        }

        private async Task ReorderStepsAfterDeleteAsync(int testCaseId, int deletedOrder)
        {
            var remainingSteps = await _testStepRepository.GetByTestCaseIdAsync(testCaseId);
            var stepsToUpdate = remainingSteps.Where(s => s.Order > deletedOrder).ToList();

            foreach (var step in stepsToUpdate)
            {
                step.Order--;
                await _testStepRepository.UpdateAsync(step);
            }
        }

        public async Task<IEnumerable<TestStep>> ValidateStepsAsync(int testCaseId)
        {
            var steps = await _testStepRepository.GetByTestCaseIdAsync(testCaseId);
            var invalidSteps = new List<TestStep>();

            foreach (var step in steps)
            {
                // Vérifications de validité
                bool isValid = true;

                // Actions nécessitant un sélecteur
                var actionsNeedingSelector = new[] { "click", "type", "select", "check", "hover" };
                if (actionsNeedingSelector.Contains(step.Action.ToLower()) &&
                    string.IsNullOrWhiteSpace(step.Selector))
                {
                    isValid = false;
                }

                // Actions nécessitant une valeur
                var actionsNeedingValue = new[] { "type", "select", "assert" };
                if (actionsNeedingValue.Contains(step.Action.ToLower()) &&
                    string.IsNullOrWhiteSpace(step.Value))
                {
                    isValid = false;
                }

                if (!isValid)
                {
                    invalidSteps.Add(step);
                }
            }

            return invalidSteps;
        }
    }
}
