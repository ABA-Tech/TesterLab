using TesterLab.Domain.interfaces.Repositories;
using TesterLab.Domain.interfaces.Services;
using TesterLab.Domain.Models;

namespace TesterLab.Applications.Services
{
    public class FeatureService : IFeatureService
    {
        private readonly IFeatureRepository _featureRepository;
        public FeatureService(IFeatureRepository featureRepository)
        {
            _featureRepository = featureRepository;
        }
        public Task<bool> CanDeleteFeatureAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Feature> CreateFeatureAsync(Feature feature)
        {
            var featureCreated = await _featureRepository.CreateAsync(feature);
            return featureCreated;
        }

        public async Task<bool> DeleteFeatureAsync(int id)
        {
            return await _featureRepository.DeleteAsync(id);
        }

        public async Task<Feature?> GetFeatureByIdAsync(int id)
        {
            return await _featureRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Feature>> GetFeaturesByApplicationAsync(int applicationId)
        {
            return await _featureRepository.GetByApplicationIdAsync(applicationId);
        }

        public Task<Feature> UpdateFeatureAsync(Feature feature)
        {
            return _featureRepository.UpdateAsync(feature);
        }
    }
}
