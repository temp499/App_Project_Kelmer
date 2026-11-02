using System.Security.Cryptography;
using App_Proyecto.DTOS;
using App_Proyecto.Models;
using App_Proyecto.Models.Settings;
using App_Proyecto.Repositories;
using AutoMapper;
using Microsoft.Extensions.Options;

namespace App_Proyecto.Services
{
    public class AuthService(
        IUsuarioRepository userRepo,
        IRoleRepository roleRepo,
        ITokenService tokenService,
        IEmailService emailService,
        IMapper mapper,
        IOptions<JwtSettings> jwtSettings,
        IConfiguration configuration) : IAuthService
    {
        private readonly JwtSettings _jwtSettings = jwtSettings.Value;

        public async Task<bool> ChangePasswordAsync(string userName, ChangePasswordDto dto)
        {
            var user = await userRepo.GetUsuarioByUserName(userName);
            if (user is null || !userRepo.ValidatePassword(user, dto.CurrentPassword))
                return false;

            user.password = BCrypt.Net.BCrypt.EnhancedHashPassword(dto.NewPassword);
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await userRepo.SaveAsync();
            return true;
        }

        public async Task<bool> ConfirmEmialAsync(string email, string token)
        {
            var user = await userRepo.GetUsuarioByEmail(email);
            if (user is null || string.IsNullOrWhiteSpace(user.EmailConfirmedToken))
                return false;

            var decodedToken = Uri.UnescapeDataString(token.Trim());
            if (!string.Equals(user.EmailConfirmedToken, decodedToken, StringComparison.Ordinal))
                return false;

            user.EmailConfirmed = "true";
            user.EmailConfirmedToken = null;
            await userRepo.SaveAsync();
            return true;
        }

        public async Task<(bool Success, string? ErrorMessage)> RegisterAsync(RegisterRequestDto request)
        {
            if (await userRepo.GetUsuarioByEmail(request.Email) is not null)
                return (false, "Correo ya en uso, intente con otro correo");

            var userRole = await roleRepo.GetByNameAsync("Usuario");
            if (userRole is null)
                return (false, "El rol Usuario no está configurado");

            var user = mapper.Map<Usuario>(request);
            user.password = BCrypt.Net.BCrypt.EnhancedHashPassword(request.Password);
            user.RolId = userRole.Id;
            user.EmailConfirmed = "false";

            var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var encodedToken = Uri.EscapeDataString(rawToken);
            user.EmailConfirmedToken = rawToken;

            await userRepo.CreateAsync(user);
            await userRepo.SaveAsync();

            var baseUrl = (configuration["Application:BaseUrl"] ?? "https://localhost:7249").TrimEnd('/');
            var confirmationLink = $"{baseUrl}/Auth/ConfirmEmail?email={Uri.EscapeDataString(user.Email)}&token={encodedToken}";
            var htmlBody = $"<p>Hola {System.Net.WebUtility.HtmlEncode(user.UserName)},</p>" +
                $"<p>Confirma tu cuenta con el siguiente enlace:</p><p><a href=\"{confirmationLink}\" " +
                "style=\"background:#007BFF;color:white;padding:10px 20px;text-decoration:none;\">Confirmar cuenta</a></p>";
            emailService.SendPasswordResetEmail(user.Email, htmlBody);

            return (true, null);
        }

        public async Task<(bool Success, LoginResponseDto? result, string? ErrorMessage)> LoginAsync(LoginRequestDto dto)
        {
            var user = await userRepo.GetUsuarioByEmail(dto.Email);
            if (user is null || !userRepo.ValidatePassword(user, dto.Password))
                return (false, null, "Credenciales inválidas");

            if (!bool.TryParse(user.EmailConfirmed, out var emailConfirmed) || !emailConfirmed)
                return (false, null, "Debe confirmar su correo electrónico");

            var accessToken = tokenService.GenerateAccessToken(user);
            var refreshToken = tokenService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await userRepo.SaveAsync();

            var result = new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(Math.Max(1, _jwtSettings.TokenValidateInMinutes)),
                RefreshTokenExpiresAt = user.RefreshTokenExpiryTime
            };

            return (true, result, null);
        }

        public async Task<LoginResponseDto?> RefreshAccessTokenAsync(string refreshToken)
        {
            var user = await userRepo.GetUsuarioByRefreshTokenAsync(refreshToken);
            if (user is null || user.RefreshTokenExpiryTime is null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return null;

            var accessToken = tokenService.GenerateAccessToken(user);
            var newRefreshToken = tokenService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await userRepo.SaveAsync();

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(Math.Max(1, _jwtSettings.TokenValidateInMinutes)),
                RefreshTokenExpiresAt = user.RefreshTokenExpiryTime
            };
        }

        public async Task<bool> SendResetPasswordLinkAsync(string email)
        {
            var user = await userRepo.GetUsuarioByEmail(email);
            if (user is null)
                return false;

            var token = tokenService.GeneratePasswordResetToken();
            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiryTime = DateTime.UtcNow.AddHours(1);
            await userRepo.SaveAsync();

            var baseUrl = (configuration["Application:BaseUrl"] ?? "https://localhost:7249").TrimEnd('/');
            var resetLink = $"{baseUrl}/Auth/ResetPassword?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(token)}";
            var htmlBody = $"<p>Solicitaste restablecer tu contraseña.</p><p><a href=\"{resetLink}\">Restablecer contraseña</a></p>";
            emailService.SendPasswordResetEmail(user.Email, htmlBody);
            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequestDto dto)
        {
            var user = await userRepo.GetUsuarioByEmail(dto.Email);
            if (user is null || string.IsNullOrWhiteSpace(user.PasswordResetToken))
                return false;

            var providedToken = Uri.UnescapeDataString(dto.Token.Trim());
            if (!string.Equals(user.PasswordResetToken, providedToken, StringComparison.Ordinal) ||
                user.PasswordResetTokenExpiryTime is null ||
                user.PasswordResetTokenExpiryTime <= DateTime.UtcNow)
                return false;

            user.password = BCrypt.Net.BCrypt.EnhancedHashPassword(dto.NewPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiryTime = null;
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await userRepo.SaveAsync();
            return true;
        }

        public async Task<bool> ResendConfirmationEmailAsync(string email)
        {
            var user = await userRepo.GetUsuarioByEmail(email);
            if (user is null || bool.TryParse(user.EmailConfirmed, out var confirmed) && confirmed)
                return false;

            var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            user.EmailConfirmedToken = rawToken;
            await userRepo.SaveAsync();

            var encodedToken = Uri.EscapeDataString(rawToken);
            var baseUrl = (configuration["Application:BaseUrl"] ?? "https://localhost:7249").TrimEnd('/');
            var confirmationLink = $"{baseUrl}/Auth/ConfirmEmail?email={Uri.EscapeDataString(user.Email)}&token={encodedToken}";
            var htmlBody = $"<p>Hola {System.Net.WebUtility.HtmlEncode(user.UserName)},</p>" +
                $"<p>Haz clic para confirmar tu cuenta:</p><p><a href=\"{confirmationLink}\">Confirmar cuenta</a></p>";
            emailService.SendPasswordResetEmail(user.Email, htmlBody);
            return true;
        }
    }
}
