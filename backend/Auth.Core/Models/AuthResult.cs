using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Core.Models
{
    /// <summary>
    /// Résultat d'une opération d'authentification.
    /// Contient les tokens en cas de succès ou un message d'erreur.
    /// </summary>
    public class AuthResult
    {
        /// <summary>
        /// Indique si l'authentification a réussi.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Access token JWT (courte durée).
        /// </summary>
        public string? AccessToken { get; set; }

        /// <summary>
        /// Refresh token (longue durée, révocable).
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Durée de validité de l'access token en secondes.
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Message d'erreur si Success = false.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Crée un résultat de succès.
        /// </summary>
        public static AuthResult Successful(string accessToken, string refreshToken, int expiresIn)
            => new()
            {
                Success = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = expiresIn
            };

        /// <summary>
        /// Crée un résultat d'échec.
        /// </summary>
        public static AuthResult Failed(string errorMessage)
            => new()
            {
                Success = false,
                ErrorMessage = errorMessage
            };
    }

}
