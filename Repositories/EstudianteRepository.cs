using App_Proyecto.Data;
using App_Proyecto.DTOS;
using App_Proyecto.Models;
using Microsoft.EntityFrameworkCore;

namespace App_Proyecto.Repositories
{
    public class EstudianteRepository(AppDBContext context) : IEstudianteRepository
    {
        public async Task<Estudiante> CreateAsync(Estudiante estudiante)
        {
            var entry = await context.Estudiantes.AddAsync(estudiante);
            await context.SaveChangesAsync();
            return entry.Entity;

        }

        public async Task<bool> DeleteAsync(int Id)
        {
            var estudiante = await context.Estudiantes.FindAsync(Id);
            if (estudiante == null) return false;

            context.Estudiantes.Remove(estudiante);
            await context.SaveChangesAsync();
            return true;

        }

        public async Task<bool> EmailExistsAsync(string email, int? ExcludeId = null)
        {
            return await context.Estudiantes.AnyAsync(e => e.Email == email && (!ExcludeId.HasValue || e.Id != ExcludeId.Value));
        }

        public async Task<Estudiante?> GetByIdAsync(int Id)
        {
            return await context.Estudiantes
                .Include(e => e.Curso)
                .FirstOrDefaultAsync(e => e.Id == Id);

        }

        public async Task<DataTableResponseDto<EstudianteTableDto>> GetPagedAsync(DataTablesRequestDto request)
        {
            var query = context.Estudiantes
                .Include(e => e.Curso)
                .AsQueryable();
            var recordsTotal = await query.CountAsync();
            var search = request.Search?.Value?.Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(e => e.Nombre.Contains(search)
                || e.Apellido.Contains(search) ||
                e.Email.Contains(search) ||
                e.Curso.Nombre.Contains(search));
            }
            var recordsFiltered = await query.CountAsync();
            var orderColumn = request.Order.FirstOrDefault()?.Column ?? 0;
            var orderDir = request.Order.FirstOrDefault()?.Dir ?? "asc";
            var ColumnName = request.Columns.ElementAtOrDefault(orderColumn)?.Data ?? "nombre";

            query = ColumnName.ToLowerInvariant() switch
            {
                "apellido" => orderDir == "desc" ? query.OrderByDescending(c => c.Apellido) : query.OrderBy(c => c.Apellido),
                "email" => orderDir == "desc" ?
                query.OrderByDescending(c => c.Email) : query.OrderBy(c => c.Email),
                "cursonombre" => orderDir == "desc" ? query.OrderByDescending(c => c.Curso.Nombre) : query.OrderBy(c => c.Curso.Nombre),
                "id" => orderDir == "desc" ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id),
                _ => orderDir == "desc" ? query.OrderByDescending(c => c.Nombre) : query.OrderBy(c => c.Nombre)

            };

            var data = await query
                .Skip(request.Start)
                .Take(request.Length > 0 ? request.Length : 10)
                .Select(c => new EstudianteTableDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Apellido = c.Apellido,
                    Email = c.Email,
                    CursoId = c.CursoId,
                    CursoNombre = c.Curso.Nombre
                }).ToListAsync();
            return new DataTableResponseDto<EstudianteTableDto>
            {
                Draw = request.Draw,
                RecordsTotal = recordsTotal,
                RecordsTotalFiltered = recordsFiltered,
                Data = data
            };
        }

        public async Task<bool> UpdateAsync(Estudiante estudiante)
        {
            context.Estudiantes.Update(estudiante);
            await context.SaveChangesAsync();
            return true;
        }
    
    }
}

