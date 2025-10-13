using Microsoft.EntityFrameworkCore;
using TesterLab.Domain.interfaces.Repositories;
using TesterLab.Domain.Models;

namespace TesterLab.Infrastructure.Data.Repositories
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly TesterLabDbContext _context;

        public ApplicationRepository(TesterLabDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Application>> GetAllAsync()
        {
            return await _context.Applications
                .Include(a => a.Features)
                .Include(a => a.Environments)
                .OrderBy(a => a.Name)
                .ToListAsync();
        }

        public async Task<Application?> GetByIdAsync(int id)
        {
            return await _context.Applications
                .Include(a => a.Features)
                .Include(a => a.Environments)
                .Include(a => a.TestDataSets)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Application?> GetByNameAsync(string name)
        {
            return await _context.Applications
                .FirstOrDefaultAsync(a => a.Name.ToLower() == name.ToLower());
        }

        public async Task<Application> CreateAsync(Application application)
        {
            _context.Applications.Add(application);
            await _context.SaveChangesAsync();
            return application;
        }

        public async Task<Application> UpdateAsync(Application application)
        {
            application.UpdatedAt = DateTime.UtcNow;
            _context.Applications.Update(application);
            await _context.SaveChangesAsync();
            return application;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application == null) return false;

            _context.Applications.Remove(application);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Applications.AnyAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Application>> GetByUserAsync(string userId)
        {
            // À implémenter selon votre système d'authentification
            return await GetAllAsync();
        }

        public async Task<Application?> GetSelectedAsync()
        {
            return await _context.Applications.FirstOrDefaultAsync(x=>x.Selected);
        }

        public async Task<Application?> SetSelectedAsync(int applicationId)
        {
            var res = _context.Applications.Where(x => x.Selected).ToList();
            res.ForEach(x => { x.Selected = false; x.UpdatedAt = DateTime.Now; });

            var current = await GetByIdAsync(applicationId);
            if (current == null)
            {
                return null;
            }

            current.UpdatedAt = DateTime.Now;
            current.Selected = true;
            res.Add(current);

            _context.Applications.UpdateRange(res);
            await _context.SaveChangesAsync();
            return current;
        }
    }
}
