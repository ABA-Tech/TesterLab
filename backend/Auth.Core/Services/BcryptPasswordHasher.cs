using Auth.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Core.Services
{
    /// <summary>
    /// Implémentation du hachage de mots de passe avec BCrypt.
    /// BCrypt est lent volontairement pour résister aux attaques par force brute.
    /// </summary>
    public class BcryptPasswordHasher : IPasswordHasher
    {
        private const int WorkFactor = 12; // Plus c'est élevé, plus c'est lent (mais sécurisé)

        /// <summary>
        /// Hache un mot de passe avec BCrypt.
        /// </summary>
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
        }

        /// <summary>
        /// Vérifie un mot de passe contre son hash.
        /// Temps constant pour éviter les timing attacks.
        /// </summary>
        public bool Verify(string password, string hash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                // Si le hash est invalide, retourner false au lieu de throw
                return false;
            }
        }
    }

}
