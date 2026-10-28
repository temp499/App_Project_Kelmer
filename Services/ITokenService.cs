using App_Proyecto.Models;

namespace App_Proyecto.Services
{
    public interface ITokenService
    {

        string GenerateAccessToken(Usuario usuario);
        string GenerateRefreshToken();
        string GeneratePasswordResetToken();
    }
}
