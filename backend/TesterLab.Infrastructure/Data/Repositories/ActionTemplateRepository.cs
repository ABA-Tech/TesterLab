using Microsoft.EntityFrameworkCore;
using TesterLab.Domain.interfaces.Repositories;
using TesterLab.Domain.Models;

namespace TesterLab.Infrastructure.Data.Repositories
{
    public class ActionTemplateRepository : GenericRepository<ActionTemplate>, IActionTemplateRepository
    {
        public ActionTemplateRepository(TesterLabDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<ActionTemplate>> GetAllAsync()
        {
            return await _context.ActionTemplates
                .OrderBy(at => at.Category)
                .ThenBy(at => at.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<ActionTemplate>> GetByCategoryAsync(string category)
        {
            return await _context.ActionTemplates
                .Where(at => at.Category.ToLower() == category.ToLower())
                .OrderBy(at => at.Name)
                .ToListAsync();
        }

        public override async Task<ActionTemplate?> GetByIdAsync(int id)
        {
            return await _context.ActionTemplates.FindAsync(id);
        }

        public async Task<ActionTemplate?> GetByNameAsync(string name)
        {
            return await _context.ActionTemplates
                .FirstOrDefaultAsync(at => at.Name.ToLower() == name.ToLower());
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            return await _context.ActionTemplates
                .Select(at => at.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }

        public async Task<IEnumerable<ActionTemplate>> SearchTemplatesAsync(string searchTerm)
        {
            return await _context.ActionTemplates
                .Where(at => at.Name.Contains(searchTerm) ||
                            at.Description.Contains(searchTerm) ||
                            at.Category.Contains(searchTerm))
                .OrderBy(at => at.Category)
                .ThenBy(at => at.Name)
                .ToListAsync();
        }
    }
}
