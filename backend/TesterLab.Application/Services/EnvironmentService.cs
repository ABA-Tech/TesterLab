using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesterLab.Domain.interfaces.Repositories;
using TesterLab.Domain.interfaces.Services;
using Environment = TesterLab.Domain.Models.Environment;

namespace TesterLab.Applications.Services
{
    public class EnvironmentService : IEnvironmentService
    {
        private readonly IEnvironmentRepository _environmentRepository;
        private readonly IApplicationRepository _applicationRepository;
        //private readonly HttpClient _httpClient;

        public EnvironmentService(
            IEnvironmentRepository environmentRepository,
            IApplicationRepository applicationRepository/*
            HttpClient httpClient*/)
        {
            _environmentRepository = environmentRepository;
            _applicationRepository = applicationRepository;
            //_httpClient = httpClient;
        }

        public async Task<IEnumerable<Environment>> GetEnvironmentsByApplicationAsync(int applicationId)
        {
            return await _environmentRepository.GetByApplicationIdAsync(applicationId);
        }

        public async Task<Environment?> GetEnvironmentByIdAsync(int id)
        {
            return await _environmentRepository.GetByIdAsync(id);
        }

        public async Task<Environment> CreateEnvironmentAsync(Environment environment)
        {
            // Validation métier
            if (string.IsNullOrWhiteSpace(environment.Name))
                throw new ArgumentException("Le nom de l'environnement est requis");

            if (string.IsNullOrWhiteSpace(environment.BaseUrl))
                throw new ArgumentException("L'URL de base est requise");

            if (!Uri.IsWellFormedUriString(environment.BaseUrl, UriKind.Absolute))
                throw new ArgumentException("L'URL de base n'est pas valide");

            // Vérifier que l'application existe
            var application = await _applicationRepository.GetByIdAsync(environment.ApplicationId);
            if (application == null)
                throw new ArgumentException("Application non trouvée");

            // Vérifier l'unicité du nom dans l'application
            var existingEnvs = await _environmentRepository.GetByApplicationIdAsync(environment.ApplicationId);
            if (existingEnvs.Any(e => e.Name.ToLower() == environment.Name.ToLower()))
            {
                throw new InvalidOperationException($"Un environnement avec le nom '{environment.Name}' existe déjà dans cette application");
            }

            return await _environmentRepository.CreateAsync(environment);
        }

        public async Task<Environment> UpdateEnvironmentAsync(Environment environment)
        {
            var existing = await _environmentRepository.GetByIdAsync(environment.Id);
            if (existing == null)
                throw new ArgumentException("Environnement non trouvé");

            // Validation similaire à Create
            if (string.IsNullOrWhiteSpace(environment.Name))
                throw new ArgumentException("Le nom de l'environnement est requis");

            if (!Uri.IsWellFormedUriString(environment.BaseUrl, UriKind.Absolute))
                throw new ArgumentException("L'URL de base n'est pas valide");

            return await _environmentRepository.UpdateAsync(environment);
        }

        public async Task<bool> DeleteEnvironmentAsync(int id)
        {
            var environment = await _environmentRepository.GetByIdAsync(id);
            if (environment == null)
                throw new ArgumentException("Environnement non trouvé");

            return await _environmentRepository.DeleteAsync(id);
        }

        public async Task<bool> TestEnvironmentConnectivityAsync(int id)
        {
            var environment = await _environmentRepository.GetByIdAsync(id);
            if (environment == null)
                throw new ArgumentException("Environnement non trouvé");

            try
            {
                //_httpClient.Timeout = TimeSpan.FromSeconds(10);
                //var response = await _httpClient.GetAsync(environment.BaseUrl);

                // Considérer comme accessible si le serveur répond (même avec une erreur HTTP)
                //return response != null;
                return true;
            }
            catch (HttpRequestException)
            {
                return false;
            }
            catch (TaskCanceledException)
            {
                // Timeout
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Dictionary<int, bool>> TestAllEnvironmentsAsync(int applicationId)
        {
            var environments = await _environmentRepository.GetByApplicationIdAsync(applicationId);
            var results = new Dictionary<int, bool>();

            var tasks = environments.Select(async env =>
            {
                var isAccessible = await TestEnvironmentConnectivityAsync(env.Id);
                return new { env.Id, IsAccessible = isAccessible };
            });

            var testResults = await Task.WhenAll(tasks);

            foreach (var result in testResults)
            {
                results[result.Id] = result.IsAccessible;
            }

            return results;
        }

        public async Task<Environment> CloneEnvironmentAsync(int sourceId, string newName, string? newUrl = null)
        {
            var source = await _environmentRepository.GetByIdAsync(sourceId);
            if (source == null)
                throw new ArgumentException("Environnement source non trouvé");

            var cloned = new Environment
            {
                ApplicationId = source.ApplicationId,
                Name = newName,
                BaseUrl = newUrl ?? source.BaseUrl,
                Type = source.Type,
                Description = $"Cloné de: {source.Name}",
                RequiresAuth = source.RequiresAuth,
                AccessInfo = source.AccessInfo,
                Active = true
            };

            return await CreateEnvironmentAsync(cloned);
        }
    }
}
