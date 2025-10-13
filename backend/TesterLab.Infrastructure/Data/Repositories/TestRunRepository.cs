using Microsoft.EntityFrameworkCore;
using TesterLab.Domain.interfaces.Repositories;
using TesterLab.Domain.Models;

namespace TesterLab.Infrastructure.Data.Repositories
{
    public class TestRunRepository : ITestRunRepository
    {
        private readonly TesterLabDbContext _context;

        public TestRunRepository(TesterLabDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TestRun>> GetAllAsync()
        {
            return await _context.TestRuns
                .Include(tr => tr.Application)
                .Include(tr => tr.Environment)
                .Include(tr => tr.TestData)
                .OrderByDescending(tr => tr.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<TestRun>> GetByApplicationIdAsync(int applicationId)
        {
            return await _context.TestRuns
                .Where(tr => tr.ApplicationId == applicationId)
                .Include(tr => tr.Environment)
                .Include(tr => tr.TestData)
                .OrderByDescending(tr => tr.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<TestRun>> GetRecentAsync(int applicationId, int count = 10)
        {
            return await _context.TestRuns
                .Where(tr => tr.ApplicationId == applicationId)
                .Include(tr => tr.Environment)
                .Include(tr=>tr.Application)
                .OrderByDescending(tr => tr.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<TestRun?> GetByIdAsync(int id)
        {
            return await _context.TestRuns
                .Include(tr => tr.Application)
                .Include(tr => tr.Environment)
                .Include(tr => tr.TestData)
                .FirstOrDefaultAsync(tr => tr.Id == id);
        }

        public async Task<TestRun> CreateAsync(TestRun testRun)
        {
            _context.TestRuns.Add(testRun);
            await _context.SaveChangesAsync();
            return testRun;
        }

        public async Task<TestRun> UpdateAsync(TestRun testRun)
        {
            _context.TestRuns.Update(testRun);
            await _context.SaveChangesAsync();
            return testRun;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var testRun = await _context.TestRuns.FindAsync(id);
            if (testRun == null) return false;

            _context.TestRuns.Remove(testRun);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TestRun>> GetRunningAsync()
        {
            return await _context.TestRuns
                .Where(tr => tr.Status == "Running" || tr.Status == "Created")
                .Include(tr => tr.Application)
                .Include(tr => tr.Environment)
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetStatusCountsAsync(int applicationId)
        {
            return await _context.TestRuns
                .Where(tr => tr.ApplicationId == applicationId)
                .GroupBy(tr => tr.Status)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }
    }
}
