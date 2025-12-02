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
    public class TestStepRepository : GenericRepository<TestStep>, ITestStepRepository
    {
        public TestStepRepository(TesterLabDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TestStep>> GetByTestCaseIdAsync(int testCaseId)
        {
            return await _context.TestSteps
                .Where(ts => ts.TestCaseId == testCaseId)
                .OrderBy(ts => ts.Order)
                .ToListAsync();
        }

        public override async Task<TestStep?> GetByIdAsync(int id)
        {
            return await _context.TestSteps
                .Include(ts => ts.TestCase)
                .FirstOrDefaultAsync(ts => ts.Id == id);
        }

        public async Task<bool> ReorderStepsAsync(int testCaseId, Dictionary<int, int> newOrders)
        {
            foreach (var kvp in newOrders)
            {
                var step = await _context.TestSteps.FindAsync(kvp.Key);
                if (step != null && step.TestCaseId == testCaseId)
                {
                    step.Order = kvp.Value;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TestStep>> GetByActionAsync(string action)
        {
            return await _context.TestSteps
                .Where(ts => ts.Action.ToLower() == action.ToLower())
                .Include(ts => ts.TestCase)
                .OrderBy(ts => ts.TestCaseId)
                .ThenBy(ts => ts.Order)
                .ToListAsync();
        }

        public async Task<IEnumerable<TestStep>> GetOptionalStepsAsync(int testCaseId)
        {
            return await _context.TestSteps
                .Where(ts => ts.TestCaseId == testCaseId && ts.IsOptional)
                .OrderBy(ts => ts.Order)
                .ToListAsync();
        }

        public async Task<TestStep?> GetStepByOrderAsync(int testCaseId, int order)
        {
            return await _context.TestSteps
                .FirstOrDefaultAsync(ts => ts.TestCaseId == testCaseId && ts.Order == order);
        }

        public async Task<int> GetMaxOrderAsync(int testCaseId)
        {
            var maxOrder = await _context.TestSteps
                .Where(ts => ts.TestCaseId == testCaseId)
                .MaxAsync(ts => (int?)ts.Order);

            return maxOrder ?? 0;
        }


        public async Task AddRangeAsync(IEnumerable<TestStep> testSteps)
        {
            await _context.TestSteps.AddRangeAsync(testSteps);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<TestStep> testSteps)
        {
            _context.TestSteps.RemoveRange(testSteps);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetMaxOrderByTestCaseIdAsync(int testCaseId)
        {
            var maxOrder = await _context.TestSteps
                .Where(s => s.TestCaseId == testCaseId)
                .MaxAsync(s => (int?)s.Order);
            
            return maxOrder ?? 0;
        }

        public async Task<bool> TestCaseExistsAsync(int testCaseId)
        {
            return await _context.TestCases.AnyAsync(tc => tc.Id == testCaseId);
        }
    }
}
