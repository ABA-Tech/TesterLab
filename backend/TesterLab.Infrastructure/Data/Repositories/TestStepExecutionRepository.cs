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
    public class TestStepExecutionRepository : ITestStepExecutionRepository
    {
        private readonly TesterLabDbContext _context;

        public TestStepExecutionRepository(TesterLabDbContext context)
        {
            _context = context;
        }

        public async Task<TestStepExecution> CreateAsync(TestStepExecution execution)
        {
            _context.TestStepExecutions.Add(execution);
            await _context.SaveChangesAsync();
            return execution;
        }

        public async Task<TestStepExecution> UpdateAsync(TestStepExecution execution)
        {
            _context.TestStepExecutions.Update(execution);
            await _context.SaveChangesAsync();
            return execution;
        }

        public async Task<IEnumerable<TestStepExecution>> GetByTestCaseExecutionIdAsync(int testCaseExecutionId)
        {
            return await _context.TestStepExecutions
                .Where(tse => tse.TestCaseExecutionId == testCaseExecutionId)
                .Include(tse => tse.TestStep)
                .OrderBy(tse => tse.StepOrder)
                .ToListAsync();
        }
    }
}
