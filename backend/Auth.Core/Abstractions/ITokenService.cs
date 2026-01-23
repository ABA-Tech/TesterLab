using System.Security.Claims;

namespace Auth.Core.Abstractions
{
    /// <summary>
    /// Service de gestion des tokens d'authentification.
    /// Abstraction permettant différentes implémentations (JWT, OAuth, etc.)
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Génère un access token avec les claims fournis.
        /// </summary>
        /// <param name="claims">Informations utilisateur à inclure dans le token</param>
        /// <returns>Token d'accès encodé</returns>
        string GenerateAccessToken(IEnumerable<Claim> claims);

        /// <summary>
        /// Génère un refresh token aléatoire sécurisé.
        /// </summary>
        /// <returns>Refresh token encodé</returns>
        string GenerateRefreshToken();

        /// <summary>
        /// Valide un token et extrait les claims.
        /// </summary>
        /// <param name="token">Token à valider</param>
        /// <param name="validateLifetime">Vérifier l'expiration ou non</param>
        /// <returns>ClaimsPrincipal si valide, null sinon</returns>
        ClaimsPrincipal? ValidateToken(string token, bool validateLifetime = true);
    }

}
