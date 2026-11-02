using App_Proyecto.Data;
using App_Proyecto.Models;
using Microsoft.EntityFrameworkCore;

namespace App_Proyecto.Repositories
{
    public class RolRepository(AppDBContext context) : IRoleRepository
    {
        public async Task<bool> AssingPermissionToRoleAsync(int roleId, int permissionId)
        {
            var exists = await context.RolePermissions
                .AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);

            if (exists) return false;
            context.RolePermissions.Add(new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId
            });
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<Role> CreateAsync(string name)
        {
            var role = new Role { Name = name.Trim() };
            var entry = await context.Roles.AddAsync(role);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<IReadOnlyList<Role>> GetAllWithPermissionsAsync()
        {
            return await context.Roles
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .ToListAsync();
        }

        public async  Task<Role?> GetByIdWithPermissionsAsync(int id)
        {
            return await context.Roles
              .Include(r => r.RolePermissions)
              .ThenInclude(rp => rp.Permission)
              .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Role?> GetByNameAsync(string name)
        {
            return await context.Roles.FirstOrDefaultAsync(r => r.Name == name);
        }

        public async Task<bool> RemovePermissionFromRoleAsync(int roleId, int permissionId)
        {
            var rolePermission = await context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId
                && rp.PermissionId == permissionId);

            if (rolePermission == null)
                return false;
            context.RolePermissions.Remove(rolePermission);
            await context.SaveChangesAsync();
            return true;
        }

    }
}
