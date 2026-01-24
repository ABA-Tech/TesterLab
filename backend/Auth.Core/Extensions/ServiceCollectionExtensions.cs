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
            // Récupérer la politique de mot de passe depuis la configuration
            var passwordPolicySection = configuration.GetSection("PasswordPolicy");
            var passwordPolicy = new PasswordPolicy();

            // Mapper manuellement les valeurs
            if (int.TryParse(passwordPolicySection["MinimumLength"], out var minLength))
                passwordPolicy.MinimumLength = minLength;

            if (bool.TryParse(passwordPolicySection["RequireUppercase"], out var requireUpper))
                passwordPolicy.RequireUppercase = requireUpper;

            if (bool.TryParse(passwordPolicySection["RequireLowercase"], out var requireLower))
                passwordPolicy.RequireLowercase = requireLower;

            if (bool.TryParse(passwordPolicySection["RequireDigit"], out var requireDigit))
                passwordPolicy.RequireDigit = requireDigit;

            if (bool.TryParse(passwordPolicySection["RequireSpecialCharacter"], out var requireSpecial))
                passwordPolicy.RequireSpecialCharacter = requireSpecial;

            if (int.TryParse(passwordPolicySection["MaxFailedAttempts"], out var maxAttempts))
                passwordPolicy.MaxFailedAttempts = maxAttempts;

            if (int.TryParse(passwordPolicySection["LockoutDurationMinutes"], out var lockoutDuration))
                passwordPolicy.LockoutDurationMinutes = lockoutDuration;

            // Enregistrement des services de base
            services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
            services.AddSingleton(passwordPolicy); // Enregistrer comme singleton
            services.AddScoped<PasswordValidator>(provider =>
                new PasswordValidator(provider.GetRequiredService<PasswordPolicy>()));

            // Enregistrement des services
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            //services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
            /*services.AddScoped(_ => new PasswordValidator(passwordPolicy));*/

            return services;
        }
    }

}
