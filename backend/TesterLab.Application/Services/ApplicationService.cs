using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesterLab.Domain.Models;
using TesterLab.Domain.interfaces.Repositories;
using TesterLab.Domain.interfaces.Services;

namespace TesterLab.Applications.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;

        public ApplicationService(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task<IEnumerable<Application>> GetAllApplicationsAsync()
        {
            return await _applicationRepository.GetAllAsync();
        }

        public async Task<Application?> GetApplicationByIdAsync(int id)
        {
            return await _applicationRepository.GetByIdAsync(id);
        }

        public async Task<Application> CreateApplicationAsync(Application application)
        {
            var existingApp = await _applicationRepository.GetByNameAsync(application.Name);
            if (existingApp != null)
            {
                throw new InvalidOperationException($"Une application avec le nom '{application.Name}' existe déjà.");
            }

            return await _applicationRepository.CreateAsync(application);
        }

        public async Task<Application> UpdateApplicationAsync(Application application)
        {
            var existingApp = await _applicationRepository.GetByNameAsync(application.Name);
            if (existingApp != null && existingApp.Id != application.Id)
            {
                throw new InvalidOperationException($"Une application avec le nom '{application.Name}' existe déjà.");
            }

            return await _applicationRepository.UpdateAsync(application);
        }

        public async Task<bool> DeleteApplicationAsync(int id)
        {
            return await _applicationRepository.DeleteAsync(id);
        }

        public async Task<bool> ValidateApplicationNameAsync(string name, int? excludeId = null)
        {
            var existing = await _applicationRepository.GetByNameAsync(name);
            return existing == null || (excludeId.HasValue && existing.Id == excludeId.Value);
        }

        public async Task<Application?> GetSelectedAsync()
        {
            var selected = await _applicationRepository.GetSelectedAsync();
            return selected;
        }

        public async Task<Application?> SetSelectedAsync(int appId)
        {
            var selected = await _applicationRepository.SetSelectedAsync(appId);
            return selected;
        }
    }
}
