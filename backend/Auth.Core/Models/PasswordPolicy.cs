using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Core.Models
{
    /// <summary>
    /// Politique de complexité des mots de passe.
    /// Configurable via appsettings.json.
    /// </summary>
    public class PasswordPolicy
    {
        /// <summary>
        /// Longueur minimale du mot de passe.
        /// Recommandé: 12+ caractères.
        /// </summary>
        public int MinimumLength { get; set; } = 12;

        /// <summary>
        /// Nécessite au moins une majuscule.
        /// </summary>
        public bool RequireUppercase { get; set; } = true;

        /// <summary>
        /// Nécessite au moins une minuscule.
        /// </summary>
        public bool RequireLowercase { get; set; } = true;

        /// <summary>
        /// Nécessite au moins un chiffre.
        /// </summary>
        public bool RequireDigit { get; set; } = true;

        /// <summary>
        /// Nécessite au moins un caractère spécial.
        /// </summary>
        public bool RequireSpecialCharacter { get; set; } = true;

        /// <summary>
        /// Nombre maximum de tentatives avant verrouillage.
        /// </summary>
        public int MaxFailedAttempts { get; set; } = 5;

        /// <summary>
        /// Durée de verrouillage du compte en minutes.
        /// </summary>
        public int LockoutDurationMinutes { get; set; } = 15;
    }

}
