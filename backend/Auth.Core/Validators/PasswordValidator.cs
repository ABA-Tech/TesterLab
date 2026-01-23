using Auth.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Auth.Core.Validators
{
    /// <summary>
    /// Validateur de mots de passe selon une politique de sécurité.
    /// </summary>
    public class PasswordValidator
    {
        private readonly PasswordPolicy _policy;

        public PasswordValidator(PasswordPolicy policy)
        {
            _policy = policy;
        }

        /// <summary>
        /// Valide un mot de passe selon la politique configurée.
        /// </summary>
        /// <param name="password">Mot de passe à valider</param>
        /// <returns>Tuple (IsValid, ErrorMessage)</returns>
        public (bool IsValid, string? ErrorMessage) Validate(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return (false, "Le mot de passe ne peut pas être vide");

            if (password.Length < _policy.MinimumLength)
                return (false, $"Le mot de passe doit contenir au moins {_policy.MinimumLength} caractères");

            if (_policy.RequireUppercase && !Regex.IsMatch(password, @"[A-Z]"))
                return (false, "Le mot de passe doit contenir au moins une majuscule");

            if (_policy.RequireLowercase && !Regex.IsMatch(password, @"[a-z]"))
                return (false, "Le mot de passe doit contenir au moins une minuscule");

            if (_policy.RequireDigit && !Regex.IsMatch(password, @"\d"))
                return (false, "Le mot de passe doit contenir au moins un chiffre");

            if (_policy.RequireSpecialCharacter && !Regex.IsMatch(password, @"[^a-zA-Z0-9]"))
                return (false, "Le mot de passe doit contenir au moins un caractère spécial");

            return (true, null);
        }
    }

}
