using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesterLab.Domain.interfaces.Repositories;
using Environment = TesterLab.Domain.Models.Environment;

namespace TesterLab.Infrastructure.Data.Repositories
{
    public class EnvironmentRepository : IEnvironmentRepository
    {
        private readonly TesterLabDbContext _context;
        public EnvironmentRepository(TesterLabDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Environment>> GetAllAsync()
        {
            return await _context.Environments
                .Include(e => e.Application)
                .OrderBy(e => e.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Environment>> GetByApplicationIdAsync(int applicationId)
        {
            return await _context.Environments
                .Where(e => e.ApplicationId == applicationId && e.Active)
                .OrderBy(e => e.Type)
                .ThenBy(e => e.Name)
                .ToListAsync();
        }

        public async Task<Environment?> GetByIdAsync(int id)
        {
            return await _context.Environments
                .Include(e => e.Application)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Environment>> GetByTypeAsync(string type)
        {
            return await _context.Environments
                .Where(e => e.Type.ToLower() == type.ToLower())
                .Include(e => e.Application)
                .OrderBy(e => e.Name)
                .ToListAsync();
        }

        public async Task<Environment?> GetByNameAndApplicationAsync(string name, int applicationId)
        {
            return await _context.Environments
                .FirstOrDefaultAsync(e => e.Name.ToLower() == name.ToLower() &&
                                         e.ApplicationId == applicationId);
        }

        public async Task<IEnumerable<Environment>> GetActiveEnvironmentsAsync(int applicationId)
        {
            return await _context.Environments
                .Where(e => e.ApplicationId == applicationId && e.Active)
                .OrderBy(e => e.Type)
                .ToListAsync();
        }

        public async Task<Environment> CreateAsync(Environment environment)
        {
            environment.CreatedAt = DateTime.Now;
            var env = _context.Environments.Add(environment);

            await _context.SaveChangesAsync();

            return environment;
        }

        public async Task<Environment> UpdateAsync(Environment environment)
        {
            var env = _context.Environments.Update(environment);
            await _context.SaveChangesAsync();
            return environment;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var env = await _context.Environments.FirstAsync(e => e.Id == id);
            if (env == null) return false;

            _context.Environments.Remove(env);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
