using Microsoft.EntityFrameworkCore;
using TesterLab.Domain.Models;
using TesterLab.Infrastructure.Data;

namespace TesterLab.JobScheduler.Services
{
    public class JobRepository
    {
        protected TesterLabDbContext _context { get; set; }

        public JobRepository(TesterLabDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Job>> GetJobs()
        {
            return await _context.Jobs.OrderBy(j => j.NextExecutionTimeUtc).ToListAsync();
        }

        public async Task<Job> GetJob(int id)
        {
            var job = await _context.Jobs.FindAsync(id);

            if(job == null)
            {
                throw new Exception("Job inexistant");
            }

            return job;
        }

        public async Task<Job> AddJob(Job job)
        {
            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();
            return job;
        }

        public async Task<Job> UpdateJob(Job job)
        {
            _context.Jobs.Update(job);
            await _context.SaveChangesAsync();
            return job;
        }

        public async Task DeleteJob(Job job)
        {
            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
        }
    }
}
