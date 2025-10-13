using Microsoft.EntityFrameworkCore;
using TesterLab.Domain.interfaces.Repositories;
using TesterLab.Domain.Models;

namespace TesterLab.Infrastructure.Data.Repositories
{
    public class FeatureRepository : IFeatureRepository
    {
        private readonly TesterLabDbContext _context;

        public FeatureRepository(TesterLabDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Feature>> GetAllAsync()
        {
            return await _context.Features
                .Include(f => f.Application)
                .OrderBy(f => f.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Feature>> GetByApplicationIdAsync(int applicationId)
        {
            return await _context.Features
                .Where(f => f.ApplicationId == applicationId && f.Active)
                .Include(f => f.TestCases.Where(tc => tc.Active))
                .OrderBy(f => f.BusinessPriority)
                .ThenBy(f => f.Name)
                .ToListAsync();
        }

        public async Task<Feature?> GetByIdAsync(int id)
        {
            return await _context.Features
                .Include(f => f.Application)
                .Include(f => f.TestCases)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<Feature> CreateAsync(Feature feature)
        {
            _context.Features.Add(feature);
            await _context.SaveChangesAsync();
            return feature;
        }

        public async Task<Feature> UpdateAsync(Feature feature)
        {
            feature.UpdatedAt = DateTime.Now;
            _context.Features.Update(feature);
            await _context.SaveChangesAsync();
            return feature;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var feature = await _context.Features.FindAsync(id);
            if (feature == null) return false;

            _context.Features.Remove(feature);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetTestCaseCountAsync(int featureId)
        {
            return await _context.TestCases
                .CountAsync(tc => tc.FeatureId == featureId && tc.Active);
        }
    }

}
