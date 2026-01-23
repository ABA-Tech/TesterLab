using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.JWT.Models
{
    /// <summary>
    /// Configuration pour la génération et validation de JWT.
    /// SÉCURITÉ: SecretKey ne doit JAMAIS être en clair dans le code source.
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// Clé secrète pour signer les tokens.
        /// MINIMUM 32 caractères. Stockée dans variables d'environnement.
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// Émetteur du token (votre API).
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// Audience du token (votre application cliente).
        /// </summary>
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// Durée de validité de l'access token en minutes.
        /// Recommandé: 15-60 minutes.
        /// </summary>
        public int ExpirationMinutes { get; set; } = 15;

        /// <summary>
        /// Durée de validité du refresh token en jours.
        /// Recommandé: 7-30 jours.
        /// </summary>
        public int RefreshTokenExpirationDays { get; set; } = 7;
    }

}
