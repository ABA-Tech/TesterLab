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
    public class PerformanceMetricRepository : IPerformanceMetricRepository
    {
        private readonly TesterLabDbContext _context;

        public PerformanceMetricRepository(TesterLabDbContext context)
        {
            _context = context;
        }

        public async Task<PerformanceMetric> CreateAsync(PerformanceMetric metric)
        {
            _context.PerformanceMetrics.Add(metric);
            await _context.SaveChangesAsync();
            return metric;
        }

        public async Task<IEnumerable<PerformanceMetric>> GetByTestRunIdAsync(int testRunId)
        {
            return await _context.PerformanceMetrics
                .Where(pm => pm.TestRunId == testRunId)
                .Include(pm => pm.TestCaseExecution)
                .OrderBy(pm => pm.RecordedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PerformanceMetric>> GetByMetricNameAsync(string metricName, DateTime from, DateTime to)
        {
            return await _context.PerformanceMetrics
                .Where(pm => pm.MetricName == metricName
                    && pm.RecordedAt >= from
                    && pm.RecordedAt <= to)
                .Include(pm => pm.TestRun)
                .Include(pm => pm.TestCaseExecution)
                .OrderBy(pm => pm.RecordedAt)
                .ToListAsync();
        }
    }
}
