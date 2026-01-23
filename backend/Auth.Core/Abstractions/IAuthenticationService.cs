using Auth.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Core.Abstractions
{
    /// <summary>
    /// Service principal d'authentification.
    /// Orchestre la validation des credentials et la génération de tokens.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Authentifie un utilisateur avec username/password.
        /// </summary>
        /// <param name="username">Nom d'utilisateur</param>
        /// <param name="password">Mot de passe</param>
        /// <param name="ipAddress">Adresse IP (audit)</param>
        /// <param name="userAgent">User-Agent (audit)</param>
        /// <returns>Résultat contenant les tokens si succès</returns>
        Task<AuthResult> AuthenticateAsync(
            string username,
            string password,
            string? ipAddress = null,
            string? userAgent = null);

        /// <summary>
        /// Renouvelle un access token via un refresh token.
        /// </summary>
        /// <param name="refreshToken">Refresh token valide</param>
        /// <returns>Nouveaux tokens si succès</returns>
        Task<AuthResult> RefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Révoque un refresh token (logout, sécurité).
        /// </summary>
        /// <param name="refreshToken">Token à révoquer</param>
        /// <param name="reason">Raison de la révocation</param>
        Task RevokeTokenAsync(string refreshToken, string reason);

        /// <summary>
        /// Révoque tous les tokens d'un utilisateur.
        /// </summary>
        /// <param name="userId">ID de l'utilisateur</param>
        /// <param name="reason">Raison (ex: changement mot de passe)</param>
        Task RevokeAllUserTokensAsync(string userId, string reason);
    }

}
