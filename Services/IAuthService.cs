using App_Proyecto.DTOS;

namespace App_Proyecto.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string? ErrorMessage)> RegisterAsync(RegisterRequestDto request);
        Task<(bool Success, LoginResponseDto? result,string? ErrorMessage)> LoginAsync(LoginRequestDto dto);
        Task<bool> ConfirmEmialAsync(string email, string token);
        Task<bool> SendResetPasswordLinkAsync(string email);
        Task<bool> ResetPasswordAsync(ResetPasswordRequestDto dto);
        Task<bool> ResendConfirmationEmailAsync(string email);
        Task<bool> ChangePasswordAsync(string email, ChangePasswordDto dto);
        Task RevokeRefreshTokenAsync(string refreshToken);
        Task<LoginResponseDto?> RefreshAccessTokenAsync(string refreshToken);
    }
}
