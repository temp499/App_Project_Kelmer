using App_Proyecto.DTOS;
using App_Proyecto.Extensions;
using App_Proyecto.Models;
using App_Proyecto.Repositories;

namespace App_Proyecto.Services
{
    public class RolePermissionService(
        IRoleRepository roleRepo,
        IPermissionRepository permissionRepo,
        IUsuarioRepository userRepo) : IRolePermissionService
    {
        public async Task<(bool Success, string? ErrorMessage)> AssignPermissionToRoleAsync(int roleId, int permissionId)
        {
            var role = await roleRepo.GetByIdWithPermissionsAsync(roleId);
            if (role is null)
                return (false, "Rol no encontrado");

            var permission = await permissionRepo.GetByIdAsync(permissionId);
            if (permission is null)
                return (false, "Permiso no encontrado");

            var assigned = await roleRepo.AssingPermissionToRoleAsync(roleId, permissionId);
            return assigned
                ? (true, null)
                : (false, "El rol ya tiene asignado ese permiso");
        }

        public async Task<(bool Success, string? ErrorMessage)> AssignPermissionToUserAsync(int userId, AssignUserPermission permission)
        {
            var user = await userRepo.GetUsuarioByIdWithPermissionAsync(userId);
            if (user is null)
                return (false, "Usuario no encontrado");

            var existingPermission = await permissionRepo.GetByIdAsync(permission.PermissionId);
            if (existingPermission is null)
                return (false, "Permiso no encontrado");

            await userRepo.AssingPermissiontoUsuarioAsync(userId, permission.PermissionId, permission.IsGranted);
            await userRepo.SaveAsync();
            return (true, null);
        }

        public async Task<(bool Success, PermissionDto? Result, string? ErrorMessage)> CreatePermissionAsync(SavePermissionDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return (false, null, "El nombre del permiso es requerido");

            var name = dto.Name.Trim().ToLowerInvariant();
            var existing = await permissionRepo.GetByNameAsync(name);
            if (existing is not null)
                return (false, null, "Ya existe un permiso con ese nombre");

            var permission = await permissionRepo.CreateAsync(name, dto.Descripcion);
            return (true, MapPermission(permission), null);
        }

        public async Task<(bool Success, RoleWithPermissionDto? Result, string? ErrorMessage)> CreateRoleAsync(CreateRolDto dto)
        {
            var name = dto.Name.Trim();
            if (string.IsNullOrWhiteSpace(name))
                return (false, null, "El nombre del rol es requerido");

            var existing = await roleRepo.GetByNameAsync(name);
            if (existing is not null)
                return (false, null, "Ya existe un rol con ese nombre");

            var role = await roleRepo.CreateAsync(name);
            return (true, MapRole(role), null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeletePermissionAsync(int id)
        {
            var permission = await permissionRepo.GetByIdAsync(id);
            if (permission is null)
                return (false, "Permiso no encontrado");

            if (await permissionRepo.IsAssignedToUsuarioAsync(id))
                return (false, "No se puede eliminar el permiso asignado a uno o más usuarios");

            var deleted = await permissionRepo.DeleteAsync(id);
            return deleted ? (true, null) : (false, "No se pudo eliminar el permiso");
        }

        public async Task<IReadOnlyList<PermissionDto>> GetAllPermissionsAsync()
        {
            var permissions = await permissionRepo.GetAllAsync();
            return permissions.Select(MapPermission).ToList();
        }

        public async Task<IReadOnlyList<RoleWithPermissionDto>> GetAllRolesAsync()
        {
            var roles = await roleRepo.GetAllWithPermissionsAsync();
            return roles.Select(MapRole).ToList();
        }

        public async Task<IReadOnlyList<UserListItemDto>> GetAllUsersAsync()
        {
            var users = await userRepo.GetAllUsuariosAsync();
            return users.Select(user => new UserListItemDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                RoleName = user.Rol?.Name ?? string.Empty
            }).ToList();
        }

        public async Task<PermissionDto?> GetPermissionByIdAsync(int id)
        {
            var permission = await permissionRepo.GetByIdAsync(id);
            return permission is null ? null : MapPermission(permission);
        }

        public Task<DataTableResponseDto<PermissionDto>> GetPermissionsPagedAsync(DataTablesRequestDto request)
        {
            return permissionRepo.GetPagedAsync(request);
        }

        public async Task<RoleWithPermissionDto?> GetRoleByIdAsync(int roleId)
        {
            var role = await roleRepo.GetByIdWithPermissionsAsync(roleId);
            return role is null ? null : MapRole(role);
        }

        public async Task<UserPermissionDto?> GetUserPermissionsAsync(int userId)
        {
            var user = await userRepo.GetUsuarioByIdWithPermissionAsync(userId);
            if (user is null)
                return null;

            return new UserPermissionDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                RoleId = user.RolId ?? 0,
                RoleName = user.Rol?.Name ?? string.Empty,
                EffectivePermission = EffectivePermission.FromUser(user),
                DirectPermission = user.UsuarioPermissions
                    .Where(up => up.Permission is not null)
                    .Select(up => new UserPermissionOverride
                    {
                        PermissionId = up.PermissionId,
                        PermissionName = up.Permission.Nombre,
                        IsGranted = up.IsGranted
                    })
                    .OrderBy(up => up.PermissionName)
                    .ToList()
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> RemovePermissionFromRoleAsync(int roleId, int permissionId)
        {
            var removed = await roleRepo.RemovePermissionFromRoleAsync(roleId, permissionId);
            return removed ? (true, null) : (false, "El permiso no está asignado al rol");
        }

        public async Task<(bool Success, string? ErrorMessage)> RemoveUserPermissionAsync(int userId, int permissionId)
        {
            var removed = await userRepo.RemovePermissionFromUsuarioAsync(userId, permissionId);
            return removed ? (true, null) : (false, "El usuario no tiene una asignación directa para este permiso");
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdatePermissionAsync(SavePermissionDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return (false, "El nombre del permiso es requerido");

            if (!dto.Id.HasValue)
                return (false, "Id de permiso inválido");

            var permission = await permissionRepo.GetByIdAsync(dto.Id.Value);
            if (permission is null)
                return (false, "Permiso no encontrado");

            var name = dto.Name.Trim().ToLowerInvariant();
            var duplicate = await permissionRepo.GetByNameAsync(name);
            if (duplicate is not null && duplicate.Id != permission.Id)
                return (false, "Ya existe un permiso con ese nombre");

            permission.Nombre = name;
            permission.Descripcion = dto.Descripcion?.Trim() ?? string.Empty;
            await permissionRepo.UpdateAsync(permission);
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateUserRoleAsync(int userId, int roleId)
        {
            if (await roleRepo.GetByIdWithPermissionsAsync(roleId) is null)
                return (false, "Rol no encontrado");

            var updated = await userRepo.UpdateUsuarioRolAsync(userId, roleId);
            if (!updated)
                return (false, "Usuario no encontrado o rol inválido");

            await userRepo.SaveAsync();
            return (true, null);
        }

        private static PermissionDto MapPermission(Permission permission) => new()
        {
            Id = permission.Id,
            Name = permission.Nombre,
            Description = permission.Descripcion
        };

        private static RoleWithPermissionDto MapRole(Role role) => new()
        {
            Id = role.Id,
            Name = role.Name,
            Permissions = role.RolePermissions
                .Where(rp => rp.Permission is not null)
                .Select(rp => MapPermission(rp.Permission))
                .OrderBy(permission => permission.Name)
                .ToList()
        };
    }
}
