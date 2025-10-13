using Microsoft.EntityFrameworkCore;
using TesterLab.Domain.interfaces.Repositories;
using TesterLab.Domain.Models;


namespace TesterLab.Infrastructure.Data.Repositories
{
    public class TestCaseRepository : ITestCaseRepository
    {
        private readonly TesterLabDbContext _context;

        public TestCaseRepository(TesterLabDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TestCase>> GetAllAsync()
        {
            return await _context.TestCases
                .Include(tc => tc.Feature)
                .OrderBy(tc => tc.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<TestCase>> GetByFeatureIdAsync(int featureId)
        {
            return await _context.TestCases
                .Where(tc => tc.FeatureId == featureId && tc.Active)
                .Include(tc => tc.TestSteps.OrderBy(ts => ts.Order))
                .OrderBy(tc => tc.CriticalityLevel)
                .ThenBy(tc => tc.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<TestCase>> GetByApplicationIdAsync(int applicationId)
        {
            return await _context.TestCases
                .Include(tc => tc.Feature)
                .Where(tc => tc.Feature.ApplicationId == applicationId && tc.Active)
                .OrderBy(tc => tc.CriticalityLevel)
                .ThenBy(tc => tc.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<TestCase>> GetByTagsAsync(string[] tags)
        {
            return await _context.TestCases
                //.Where(tc => tc.Tags == null || tc.Tags != null && tags.Any(tag => tc.Tags.Contains(tag)))
                .Include(tc => tc.Feature)
                .Include(tc => tc.TestSteps)
                .ToListAsync();
        }

        public async Task<TestCase?> GetByIdAsync(int id)
        {
            return await _context.TestCases
                .Include(tc => tc.Feature)
                .FirstOrDefaultAsync(tc => tc.Id == id);
        }

        public async Task<TestCase?> GetByIdWithStepsAsync(int id)
        {
            return await _context.TestCases
                .Include(tc => tc.Feature)
                .Include(tc => tc.TestSteps.OrderBy(ts => ts.Order))
                .FirstOrDefaultAsync(tc => tc.Id == id);
        }

        public async Task<TestCase> CreateAsync(TestCase testCase)
        {
            _context.TestCases.Add(testCase);
            await _context.SaveChangesAsync();
            return testCase;
        }

        public async Task<TestCase> UpdateAsync(TestCase testCase)
        {
            _context.TestCases.Update(testCase);
            await _context.SaveChangesAsync();
            return testCase;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var testCase = await _context.TestCases.FindAsync(id);
            if (testCase == null) return false;

            _context.TestCases.Remove(testCase);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TestCase>> GetByCriticalityAsync(int minLevel)
        {
            return await _context.TestCases
                .Where(tc => tc.CriticalityLevel >= minLevel && tc.Active)
                .Include(tc => tc.Feature)
                .OrderByDescending(tc => tc.CriticalityLevel)
                .ToListAsync();
        }
    }
}
