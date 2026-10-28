using Microsoft.EntityFrameworkCore;
using App_Proyecto.Data;
using App_Proyecto.DTOS;
using App_Proyecto.Models;

namespace App_Proyecto.Repositories
{
    public class PermissionRepository(AppDBContext context) : IPermissionRepository
    {
        public async Task<Permission> CreateAsync(string name, string? description)
        {
            var permission = new Permission
            {
                Nombre = name.Trim().ToLowerInvariant(),
                Descripcion = description?.Trim()
            };
            var entry = await context.AddAsync(permission);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var permission = await context.Permissions.FindAsync(id);
            if (permission == null) return false;
            context.Permissions.Remove(permission);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<IReadOnlyList<Permission>> GetAllAsync()
        {
            return await context.Permissions
                .OrderBy(x => x.Nombre)
                .ToListAsync();
        }

        public async Task<Permission?> GetByIdAsync(int Id)
        {
            return await context.Permissions.FindAsync(Id);
        }

        public async Task<Permission?> GetByNameAsync(string name)
        {
            return await context.Permissions
                .FirstOrDefaultAsync(x => x.Nombre == name);
        }

        public async Task<DataTableResponseDto<PermissionDto>> GetPagedAsync(DataTablesRequestDto request)
        {
            var query = context.Permissions.AsQueryable();
            var recordsTotal = await query.CountAsync();
            var search = request.Search?.Value?.Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c => c.Nombre.Contains(search)
                || (c.Descripcion != null && c.Descripcion.Contains(search)
                ));
            }
            var recordsFiltered = await query.CountAsync();
            var orderColumn = request.Order.FirstOrDefault()?.Column ?? 0;
            var orderDir = request.Order.FirstOrDefault()?.Dir ?? "asc";
            var ColumnName = request.Columns.ElementAtOrDefault(orderColumn)?.Data ?? "nombre";


            query = ColumnName.ToLowerInvariant() switch
            {
                "descripcion" => orderDir == "desc" ? query.OrderByDescending(c => c.Descripcion) : query.OrderBy(c => c.Descripcion),
                "id" => orderDir == "desc" ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id),
                _ => orderDir == "desc" ? query.OrderByDescending(c => c.Nombre) : query.OrderBy(c => c.Nombre)
            };

            var data = await query
                .Skip(request.Start)
                .Take(request.Length > 0 ? request.Length : 10)
                .Select(c => new PermissionDto
                {
                    Id = c.Id,
                    Name = c.Nombre,
                    Description = c.Descripcion,

                }).ToListAsync();
            return new DataTableResponseDto<PermissionDto>
            {
                Draw = request.Draw,
                RecordsTotal = recordsTotal,
                RecordsTotalFiltered = recordsFiltered,
                Data = data
            };
        }

        public Task<bool> IsAssignedToUsuarioAsync(int permissionId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> isAssingnedToUsuarioAsync(int permissionId)
        {
            return await context.UsuarioPermissions.AnyAsync(up => up.PermissionId == permissionId);
        }

        public async Task<bool> UpdateAsync(Permission permission)
        {
            context.Permissions.Update(permission);
            await context.SaveChangesAsync();
            return true;
        }

    }
}
