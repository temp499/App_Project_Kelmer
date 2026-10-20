using App_Proyecto.Models;

namespace App_Proyecto.Repositories
{
    public interface IRoleRepository
    {
        Task<IReadOnlyList<Rol>> GetAllWithPermissionsAsync ();
        Task<Rol?> GetByIdWithPermissionAsync(int id);
        Task<Rol?> GetByNameAsync(string name);
        Task<Rol> CreateAsync(String name);
        Task<bool> AssingPermissionToRoleAsync(int RoleId, int PermissionId);
        Task<bool> RemovePermissionFromRolAsync(int RoleId, int PermissionId);





    }
}
