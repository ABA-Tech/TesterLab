using Microsoft.EntityFrameworkCore;
using TesterLab.Domain.interfaces.Repositories;
using TesterLab.Domain.Models;

namespace TesterLab.Infrastructure.Data.Repositories
{
    public class JobRepository2 : IJobRepository2
    {
        private readonly TesterLabDbContext _context;

        public JobRepository2(TesterLabDbContext context)
        {
            _context = context;
        }

        public async Task<int> CountActiveAsync()
        {
            return await _context.Jobs
                .Where(j => j.IsEnabled)
                .CountAsync();
        }

        public async Task<List<Job>> GetUpcomingAsync(int count)
        {
            var now = DateTime.UtcNow;
            
            return await _context.Jobs
                .Where(j => j.IsEnabled && j.NextExecutionTimeUtc >= now)
                .OrderBy(j => j.NextExecutionTimeUtc)
                .Take(count)
                .ToListAsync();
        }
    }
}