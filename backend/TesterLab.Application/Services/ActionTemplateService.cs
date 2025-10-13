using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesterLab.Domain.interfaces.Repositories;
using TesterLab.Domain.interfaces.Services;
using TesterLab.Domain.Models;

namespace TesterLab.Applications.Services
{
    public class ActionTemplateService : IActionTemplateService
    {
        private readonly IActionTemplateRepository _actionTemplateRepository;

        public ActionTemplateService(IActionTemplateRepository actionTemplateRepository)
        {
            _actionTemplateRepository = actionTemplateRepository;
        }

        public async Task<IEnumerable<ActionTemplate>> GetAllTemplatesAsync()
        {
            return await _actionTemplateRepository.GetAllAsync();
        }

        public async Task<IEnumerable<ActionTemplate>> GetTemplatesByCategoryAsync(string category)
        {
            return await _actionTemplateRepository.GetByCategoryAsync(category);
        }

        public async Task<ActionTemplate> CreateCustomTemplateAsync(ActionTemplate template)
        {
            // Validation métier
            if (string.IsNullOrWhiteSpace(template.Name))
                throw new ArgumentException("Le nom de l'action est requis");

            if (string.IsNullOrWhiteSpace(template.Category))
                throw new ArgumentException("La catégorie est requise");

            if (string.IsNullOrWhiteSpace(template.Description))
                throw new ArgumentException("La description est requise");

            // Vérifier l'unicité du nom
            var existing = await _actionTemplateRepository.GetAllAsync();
            if (existing.Any(t => t.Name.ToLower() == template.Name.ToLower()))
            {
                throw new InvalidOperationException($"Une action avec le nom '{template.Name}' existe déjà");
            }

            return await _actionTemplateRepository.CreateAsync(template);
        }

        public async Task<Dictionary<string, int>> GetCategoryStatisticsAsync()
        {
            var templates = await _actionTemplateRepository.GetAllAsync();

            return templates
                .GroupBy(t => t.Category)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public async Task<IEnumerable<ActionTemplate>> SearchTemplatesAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return await GetAllTemplatesAsync();

            var allTemplates = await _actionTemplateRepository.GetAllAsync();

            return allTemplates.Where(t =>
                t.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                t.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                t.Category.Contains(query, StringComparison.OrdinalIgnoreCase)
            );
        }
    }
}
