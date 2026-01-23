using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Core.Models
{
    /// <summary>
    /// Entité représentant un refresh token stocké en base.
    /// Permet la révocation et l'audit des sessions utilisateur.
    /// </summary>
    public class RefreshToken
    {
        /// <summary>
        /// ID unique du refresh token.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Valeur du token (hashé en production).
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// ID de l'utilisateur propriétaire.
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Date d'expiration UTC.
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Date de création UTC.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Indique si le token a été révoqué.
        /// </summary>
        public bool IsRevoked { get; set; }

        /// <summary>
        /// Date de révocation UTC (si révoqué).
        /// </summary>
        public DateTime? RevokedAt { get; set; }

        /// <summary>
        /// Raison de la révocation (ex: "User logout", "Password changed").
        /// </summary>
        public string? RevokedReason { get; set; }

        /// <summary>
        /// Adresse IP lors de la création (audit).
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// User-Agent lors de la création (audit).
        /// </summary>
        public string? UserAgent { get; set; }

        /// <summary>
        /// Token de remplacement (si rotation activée).
        /// </summary>
        public string? ReplacedByToken { get; set; }

        /// <summary>
        /// Vérifie si le token est actif (non révoqué et non expiré).
        /// </summary>
        public bool IsActive => !IsRevoked && DateTime.UtcNow < ExpiresAt;
    }

}
