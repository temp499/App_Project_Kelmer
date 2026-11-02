using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using App_Proyecto.Extensions;
using App_Proyecto.Models;
using App_Proyecto.Models.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace App_Proyecto.Services
{
    public class TokenService(IOptions<JwtSettings> jwtSettings) : ITokenService
    {
        private readonly JwtSettings _settings = jwtSettings.Value;

        public string GenerateAccessToken(Usuario usuario)
        {
            if (string.IsNullOrWhiteSpace(_settings.Key) || Encoding.UTF8.GetByteCount(_settings.Key) < 32)
                throw new InvalidOperationException("Configure una clave JWT de al menos 32 bytes mediante configuración segura.");
            if (string.IsNullOrWhiteSpace(_settings.Issuer) || string.IsNullOrWhiteSpace(_settings.Audience))
                throw new InvalidOperationException("Configure el emisor y la audiencia JWT antes de emitir tokens.");

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new(ClaimTypes.Name, usuario.UserName),
                new(ClaimTypes.Email, usuario.Email)
            };

            if (!string.IsNullOrWhiteSpace(usuario.Rol?.Name))
                claims.Add(new Claim(ClaimTypes.Role, usuario.Rol.Name));

            claims.AddRange(EffectivePermission.FromUser(usuario)
                .Select(permission => new Claim(ClaimsPrincipalExtension.PermissionClaimType, permission)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(Math.Max(1, _settings.TokenValidateInMinutes)),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(64));
        }

        public string GeneratePasswordResetToken()
        {
            return Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(48));
        }
    }
}
