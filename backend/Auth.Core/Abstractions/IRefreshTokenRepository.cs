using Auth.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Core.Abstractions
{
    /// <summary>
    /// Repository pour la gestion des refresh tokens en base de données.
    /// Permet la révocation et l'audit des tokens.
    /// </summary>
    public interface IRefreshTokenRepository
    {
        /// <summary>
        /// Récupère un refresh token par sa valeur.
        /// </summary>
        Task<RefreshToken?> GetByTokenAsync(string token);

        /// <summary>
        /// Ajoute un nouveau refresh token.
        /// </summary>
        Task AddAsync(RefreshToken refreshToken);

        /// <summary>
        /// Révoque un token spécifique.
        /// </summary>
        Task RevokeAsync(string token, string reason);

        /// <summary>
        /// Révoque tous les tokens d'un utilisateur.
        /// </summary>
        Task RevokeAllForUserAsync(string userId, string reason);

        /// <summary>
        /// Vérifie si un token est actif (non révoqué, non expiré).
        /// </summary>
        Task<bool> IsActiveAsync(string token);

        /// <summary>
        /// Nettoie les tokens expirés (maintenance).
        /// </summary>
        Task CleanupExpiredTokensAsync();
    }

}
