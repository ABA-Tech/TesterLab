using Auth.Core.Abstractions;
using Auth.JWT.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Auth.JWT.Services
{
    /// <summary>
    /// Implémentation JWT de ITokenService.
    /// Génère et valide des JSON Web Tokens (JWT).
    /// </summary>
    public class JwtTokenService : ITokenService
    {
        private readonly JwtSettings _settings;
        private readonly TokenValidationParameters _validationParameters;

        public JwtTokenService(IOptions<JwtSettings> settings)
        {
            _settings = settings.Value;
            _validationParameters = CreateValidationParameters();
        }

        /// <summary>
        /// Génère un JWT access token.
        /// Format: Header.Payload.Signature (base64url)
        /// </summary>
        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_settings.ExpirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Génère un refresh token aléatoire sécurisé.
        /// Utilise RNGCryptoServiceProvider pour la génération cryptographique.
        /// </summary>
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64]; // 64 bytes = 512 bits
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        /// <summary>
        /// Valide un JWT et extrait les claims.
        /// </summary>
        public ClaimsPrincipal? ValidateToken(string token, bool validateLifetime = true)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var validationParameters = _validationParameters.Clone();
                validationParameters.ValidateLifetime = validateLifetime;

                var principal = tokenHandler.ValidateToken(
                    token,
                    validationParameters,
                    out SecurityToken validatedToken);

                // Vérification supplémentaire de l'algorithme
                if (validatedToken is not JwtSecurityToken jwtToken ||
                    !jwtToken.Header.Alg.Equals(
                        SecurityAlgorithms.HmacSha256,
                        StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                return principal;
            }
            catch (SecurityTokenExpiredException)
            {
                // Token expiré
                return null;
            }
            catch (Exception)
            {
                // Token invalide (signature, format, etc.)
                return null;
            }
        }

        /// <summary>
        /// Crée les paramètres de validation de tokens.
        /// </summary>
        private TokenValidationParameters CreateValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_settings.SecretKey)),
                ValidateIssuer = true,
                ValidIssuer = _settings.Issuer,
                ValidateAudience = true,
                ValidAudience = _settings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero, // Pas de tolérance sur l'expiration
                RequireExpirationTime = true,
                RequireSignedTokens = true
            };
        }
    }

}
