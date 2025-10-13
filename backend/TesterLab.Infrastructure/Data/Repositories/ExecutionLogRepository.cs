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
    public class ExecutionLogRepository : IExecutionLogRepository
    {
        private readonly TesterLabDbContext _context;

        public ExecutionLogRepository(TesterLabDbContext context)
        {
            _context = context;
        }

        public async Task<ExecutionLog> CreateAsync(ExecutionLog log)
        {
            _context.ExecutionLogs.Add(log);
            await _context.SaveChangesAsync();
            return log;
        }

        public async Task<IEnumerable<ExecutionLog>> GetByTestRunIdAsync(int testRunId)
        {
            return await _context.ExecutionLogs
                .Where(el => el.TestRunId == testRunId)
                .Include(el => el.TestCaseExecution)
                .OrderBy(el => el.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExecutionLog>> GetByLevelAsync(int testRunId, string level)
        {
            return await _context.ExecutionLogs
                .Where(el => el.TestRunId == testRunId && el.Level == level)
                .Include(el => el.TestCaseExecution)
                .OrderBy(el => el.Timestamp)
                .ToListAsync();
        }
    }
}
