using Microsoft.EntityFrameworkCore;
using TesterLab.Domain.interfaces.Repositories;
using TesterLab.Domain.Models;

namespace TesterLab.Infrastructure.Data.Repositories
{
    public class ScreenshotRepository : IScreenshotRepository
    {
        private readonly TesterLabDbContext _context;

        public ScreenshotRepository(TesterLabDbContext context)
        {
            _context = context;
        }

        public async Task<Screenshot> CreateAsync(Screenshot screenshot)
        {
            _context.Screenshots.Add(screenshot);
            await _context.SaveChangesAsync();
            return screenshot;
        }

        public async Task<IEnumerable<Screenshot>> GetByTestRunIdAsync(int testRunId)
        {
            return await _context.Screenshots
                .Where(s => s.TestRunId == testRunId)
                .Include(s => s.TestCaseExecution)
                .Include(s => s.TestStepExecution)
                .OrderBy(s => s.CapturedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Screenshot>> GetByTestCaseExecutionIdAsync(int testCaseExecutionId)
        {
            return await _context.Screenshots
                .Where(s => s.TestCaseExecutionId == testCaseExecutionId)
                .Include(s => s.TestStepExecution)
                .OrderBy(s => s.CapturedAt)
                .ToListAsync();
        }
    }
}
