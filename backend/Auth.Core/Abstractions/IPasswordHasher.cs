using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Core.Abstractions
{
    /// <summary>
    /// Service de hachage sécurisé de mots de passe.
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Hache un mot de passe en clair.
        /// </summary>
        /// <param name="password">Mot de passe en clair</param>
        /// <returns>Hash sécurisé</returns>
        string Hash(string password);

        /// <summary>
        /// Vérifie qu'un mot de passe correspond à un hash.
        /// </summary>
        /// <param name="password">Mot de passe en clair</param>
        /// <param name="hash">Hash stocké</param>
        /// <returns>True si le mot de passe correspond</returns>
        bool Verify(string password, string hash);
    }
}
