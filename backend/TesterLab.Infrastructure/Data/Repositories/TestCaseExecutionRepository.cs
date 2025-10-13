using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesterLab.Domain.interfaces.Repositories;
using TesterLab.Domain.Models;

namespace TesterLab.Infrastructure.Data.Repositories
{
    public class TestCaseExecutionRepository : ITestCaseExecutionRepository
    {
        private readonly TesterLabDbContext _context;

        public TestCaseExecutionRepository(TesterLabDbContext context)
        {
            _context = context;
        }

        public async Task<TestCaseExecution> CreateAsync(TestCaseExecution execution)
        {
            _context.TestCaseExecutions.Add(execution);
            await _context.SaveChangesAsync();
            return execution;
        }

        public async Task<TestCaseExecution> UpdateAsync(TestCaseExecution execution)
        {
            _context.TestCaseExecutions.Update(execution);
            await _context.SaveChangesAsync();
            return execution;
        }

        public async Task<TestCaseExecution> GetByIdAsync(int id)
        {
            return await _context.TestCaseExecutions
                .Include(tce => tce.StepExecutions)
                .Include(tce => tce.TestCase)
                .Include(tce => tce.TestRun)
                .FirstOrDefaultAsync(tce => tce.Id == id);
        }

        public async Task<IEnumerable<TestCaseExecution>> GetByTestRunIdAsync(int testRunId)
        {
            return await _context.TestCaseExecutions
                .Where(tce => tce.TestRunId == testRunId)
                .Include(tce => tce.StepExecutions)
                .Include(tce => tce.TestCase)
                .OrderBy(tce => tce.StartedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<TestCaseExecution>> GetByTestCaseIdAsync(int testCaseId, int limit = 10)
        {
            return await _context.TestCaseExecutions
                .Where(tce => tce.TestCaseId == testCaseId)
                .OrderByDescending(tce => tce.StartedAt)
                .Take(limit)
                .Include(tce => tce.TestRun)
                .Include(tce => tce.StepExecutions)
                .ToListAsync();
        }

        public async Task<IEnumerable<TestCaseExecution>> GetByTestApplicationIdAsync(int applicatonId)
        {
            return await _context.TestCaseExecutions
                .Include(tce => tce.StepExecutions)
                .Include(tce => tce.TestCase).ThenInclude(c=>c.Feature)
                .Where(tce => tce.TestCase.Feature.ApplicationId == applicatonId)
                .OrderBy(tce => tce.StartedAt)
                .ToListAsync();
        }
    }
}
