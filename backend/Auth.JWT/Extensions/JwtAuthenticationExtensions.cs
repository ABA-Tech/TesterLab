using Auth.Core.Abstractions;
using Auth.JWT.Models;
using Auth.JWT.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Auth.Core.Extensions;

namespace Auth.JWT.Extensions
{
    /// <summary>
    /// Extensions pour configurer l'authentification JWT.
    /// Point d'entrée principal pour les applications consommatrices.
    /// </summary>
    public static class JwtAuthenticationExtensions
    {
        /// <summary>
        /// Configure l'authentification complète avec JWT.
        /// Enregistre tous les services nécessaires et configure ASP.NET Core Authentication.
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">Configuration (appsettings.json)</param>
        /// <returns>Service collection pour chaînage</returns>
        public static IServiceCollection AddMyCompanyAuthWithJwt(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // 1. Ajouter les services de base (AuthenticationService, PasswordHasher, etc.)
            services.AddMyCompanyAuthCore(configuration);

            // 2. SÉCURITÉ: Récupérer la clé depuis les variables d'environnement
            var jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                ?? configuration["JwtSettings:SecretKey"];

            if (string.IsNullOrEmpty(jwtSecretKey))
            {
                throw new InvalidOperationException(
                    "JWT_SECRET_KEY doit être définie dans les variables d'environnement ou appsettings.json");
            }

            if (jwtSecretKey.Length < 32)
            {
                throw new InvalidOperationException(
                    "JWT_SECRET_KEY doit contenir au moins 32 caractères pour une sécurité adéquate");
            }

            // 3. Configurer les paramètres JWT
            var jwtSettings = new JwtSettings
            {
                SecretKey = jwtSecretKey,
                Issuer = configuration["JwtSettings:Issuer"] ?? "MyCompany.Api",
                Audience = configuration["JwtSettings:Audience"] ?? "MyCompany.Clients",
                ExpirationMinutes = int.Parse(configuration["JwtSettings:ExpirationMinutes"] ?? "15"),
                RefreshTokenExpirationDays = int.Parse(configuration["JwtSettings:RefreshTokenExpirationDays"] ?? "7")
            };

            services.Configure<JwtSettings>(options =>
            {
                options.SecretKey = jwtSettings.SecretKey;
                options.Issuer = jwtSettings.Issuer;
                options.Audience = jwtSettings.Audience;
                options.ExpirationMinutes = jwtSettings.ExpirationMinutes;
                options.RefreshTokenExpirationDays = jwtSettings.RefreshTokenExpirationDays;
            });

            // 4. Enregistrer l'implémentation JWT de ITokenService
            services.AddScoped<ITokenService, JwtTokenService>();

            // 5. Configuration de l'authentification ASP.NET Core
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // SÉCURITÉ: HTTPS obligatoire
                options.RequireHttpsMetadata = true;

                // SÉCURITÉ: Ne pas sauvegarder le token dans les propriétés d'authentification
                options.SaveToken = false;

                // Paramètres de validation
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero, // Pas de tolérance
                    RequireExpirationTime = true,
                    RequireSignedTokens = true
                };

                // Événements pour logging et diagnostics
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILogger<JwtBearerEvents>>();

                        logger.LogWarning(
                            "Échec d'authentification JWT: {Error}",
                            context.Exception.Message);

                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILogger<JwtBearerEvents>>();

                        var username = context.Principal?.Identity?.Name;
                        logger.LogInformation(
                            "Token validé avec succès pour {Username}",
                            username);

                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILogger<JwtBearerEvents>>();

                        logger.LogWarning(
                            "Challenge JWT déclenché: {Error}",
                            context.Error);

                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }
    }

}
