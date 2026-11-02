using App_Proyecto.DTOS;

namespace App_Proyecto.Services
{
    public interface IRolePermissionService
    {
        Task<(bool Success, string? ErrorMessage)> AssignPermissionToRoleAsync(int roleId, int permissionId);
        Task<(bool Success, string? ErrorMessage)> AssignPermissionToUserAsync(int userId, AssignUserPermission permission);
        Task<(bool Success, PermissionDto? Result, string? ErrorMessage)> CreatePermissionAsync(SavePermissionDto dto);
        Task<(bool Success, RoleWithPermissionDto? Result, string? ErrorMessage)> CreateRoleAsync(CreateRolDto dto);
        Task<(bool Success, string? ErrorMessage)> DeletePermissionAsync(int id);
        Task<IReadOnlyList<PermissionDto>> GetAllPermissionsAsync();
        Task<IReadOnlyList<RoleWithPermissionDto>> GetAllRolesAsync();
        Task<IReadOnlyList<UserListItemDto>> GetAllUsersAsync();
        Task<PermissionDto?> GetPermissionByIdAsync(int id);
        Task<DataTableResponseDto<PermissionDto>> GetPermissionsPagedAsync(DataTablesRequestDto request);
        Task<RoleWithPermissionDto?> GetRoleByIdAsync(int roleId);
        Task<UserPermissionDto?> GetUserPermissionsAsync(int userId);
        Task<(bool Success, string? ErrorMessage)> RemovePermissionFromRoleAsync(int roleId, int permissionId);
        Task<(bool Success, string? ErrorMessage)> RemoveUserPermissionAsync(int userId, int permissionId);
        Task<(bool Success, string? ErrorMessage)> UpdatePermissionAsync(SavePermissionDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateUserRoleAsync(int userId, int roleId);
    }
}
