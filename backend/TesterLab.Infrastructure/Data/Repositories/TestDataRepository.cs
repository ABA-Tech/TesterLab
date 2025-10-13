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
    public class TestDataRepository : GenericRepository<TestData>, ITestDataRepository
    {
        public TestDataRepository(TesterLabDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<TestData>> GetAllAsync()
        {
            return await _context.TestDataSets
                .Include(td => td.Application)
                .Include(td => td.SpecificEnvironment)
                .OrderBy(td => td.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<TestData>> GetByApplicationIdAsync(int applicationId)
        {
            return await _context.TestDataSets
                .Where(td => td.ApplicationId == applicationId)
                .Include(td => td.SpecificEnvironment)
                .OrderBy(td => td.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<TestData>> GetTemplatesAsync()
        {
            return await _context.TestDataSets
                .Where(td => td.IsTemplate)
                .Include(td => td.Application)
                .OrderBy(td => td.Name)
                .ToListAsync();
        }

        public override async Task<TestData?> GetByIdAsync(int id)
        {
            return await _context.TestDataSets
                .Include(td => td.Application)
                .Include(td => td.SpecificEnvironment)
                .FirstOrDefaultAsync(td => td.Id == id);
        }

        public async Task<IEnumerable<TestData>> GetByDataTypeAsync(string dataType)
        {
            return await _context.TestDataSets
                .Where(td => td.DataType.ToLower() == dataType.ToLower())
                .Include(td => td.Application)
                .OrderBy(td => td.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<TestData>> GetByEnvironmentAsync(int environmentId)
        {
            return await _context.TestDataSets
                .Where(td => td.SpecificEnvironmentId == environmentId)
                .Include(td => td.Application)
                .OrderBy(td => td.Name)
                .ToListAsync();
        }

        public async Task<TestData?> GetByNameAndApplicationAsync(string name, int applicationId)
        {
            return await _context.TestDataSets
                .FirstOrDefaultAsync(td => td.Name.ToLower() == name.ToLower() &&
                                          td.ApplicationId == applicationId);
        }

    }
}
