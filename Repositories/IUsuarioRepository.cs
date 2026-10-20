using App_Proyecto.Models;

namespace App_Proyecto.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetUsuarioByUserName(string userName);
        Task<Usuario?> GetUsuarioByEmail(string email);
        Task<Usuario?> GetUsuarioByIdWithPermissionAsync(int id);
        Task<Usuario> CreateAsync(Usuario usuario);
        bool ValidatePassword(Usuario usuario, string password);
        Task SaveAsync();
        Task<Usuario?> GetUsuarioByRefreshTokenAsync(string refreshToken);
        Task<bool> AssingPermissionToUsuarioAsync(int UsuarioId, int PermissionId);

        Task<bool> RemovePermissionFromUsuarioAsync(int UsuarioId, int PermissionId);
        Task<bool> UpdateUsuarioAsync(int Usuario, int roleId);
        Task<bool> UpdateUsuarioRolAsync(int UsuarioId, string roleId);
        Task<IReadOnlyList<Usuario>> GetAllUsuarioAsync();

    }
}
