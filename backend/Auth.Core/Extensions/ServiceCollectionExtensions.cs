using Auth.Core.Abstractions;
using Auth.Core.Models;
using Auth.Core.Services;
using Auth.Core.Validators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Core.Extensions
{
    /// <summary>
    /// Extensions pour configurer les services d'authentification de base.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Ajoute les services de base (sans implémentation de tokens spécifique).
        /// Utilisé par les projets d'implémentation (JWT, OAuth, etc.).
        /// </summary>
        public static IServiceCollection AddMyCompanyAuthCore(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Configuration de la politique de mots de passe
            /*services.Configure<PasswordPolicy>(
                configuration.GetSection("PasswordPolicy"));

            var passwordPolicy = configuration
                .GetSection("PasswordPolicy")
                .Get<PasswordPolicy>() ?? new PasswordPolicy();
            */
            // Enregistrement des services
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
            /*services.AddScoped(_ => new PasswordValidator(passwordPolicy));*/

            return services;
        }
    }

}
