using App_Proyecto.DTOS;
using App_Proyecto.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace App_Proyecto.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetUsuarioByUserName(string userName);
        Task<Usuario?> GetUsuarioByEmail(string email);
        Task<Usuario?> GetUsuarioByIdWithPermissionAsync(int userId);
        Task<Usuario> CreateAsync(Usuario usuario);
        bool ValidatePassword(Usuario usuario, string password);

        Task SaveAsync();

        Task<Usuario?> GetUsuarioByRefreshTokenAsync(string refreshToken);
        Task<bool> AssingPermissiontoUsuarioAsync(int usuarioId, int permissionId, bool isGranted);
        Task<bool> RemovePermissionFromUsuarioAsync(int usuarioId, int permissionId);
        Task<bool> UpdateUsuarioRolAsync(int usuarioId, int roleId);
        Task<IReadOnlyList<Usuario>> GetAllUsuariosAsync();
    }
}
