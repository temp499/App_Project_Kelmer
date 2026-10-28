using App_Proyecto.Models;
using App_Proyecto.DTOS;

namespace App_Proyecto.Repositories
{
    public interface IPermissionRepository
    { 
        Task <DataTableResponseDto<PermissionDto>> GetPagedAsync(DataTablesRequestDto request);

        Task <IReadOnlyList<Permission>> GetAllAsync();
        Task<Permission?> GetByIdAsync(int id);
        Task<Permission?> GetByNameAsync(string name);

        Task<Permission> CreateAsync(string name, string? description);
        Task<bool> UpdateAsync(Permission permission);
        Task<bool> DeleteAsync(int id); 
        Task<bool> IsAssignedToUsuarioAsync(int permissionId);

    }
}
