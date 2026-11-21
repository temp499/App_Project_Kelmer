using App_Proyecto.Data;
using App_Proyecto.DTOS;
using App_Proyecto.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

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

        public async Task<bool> TieneMatriculasAsync(int id)
        {
            return await context.Matriculas.AnyAsync(matricula => matricula.EstudianteId == id);
        }

        public async Task<Estudiante?> GetByUsuarioIdAsync(
            int usuarioId, CancellationToken cancellationToken)
        {
            return await context.Estudiantes
                .AsNoTracking()
                .Include(estudiante => estudiante.Carrera)
                .SingleOrDefaultAsync(estudiante => estudiante.UsuarioId == usuarioId, cancellationToken);
        }

        public async Task<bool> AsignarCarreraAsync(int estudianteId, int carreraId)
        {
            await using var transaction = await context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            try
            {
                var estudiante = await context.Estudiantes.AsNoTracking()
                    .SingleOrDefaultAsync(item => item.Id == estudianteId);
                if (estudiante == null)
                    return false;

                if (estudiante.CarreraId == carreraId)
                    return true;

                if (estudiante.CarreraId.HasValue
                    && await context.Matriculas.AnyAsync(matricula => matricula.EstudianteId == estudianteId))
                    return false;

                if (await context.DetallesMatricula.AnyAsync(detalle =>
                    detalle.Matricula!.EstudianteId == estudianteId
                    && detalle.DetalleOfertaAcademicaId.HasValue
                    && detalle.DetalleOfertaAcademica!.OfertaAcademica!.PlanEstudio!.CarreraId != carreraId))
                    return false;

                var actualizados = await context.Estudiantes
                    .Where(item => item.Id == estudianteId)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(item => item.CarreraId, carreraId));
                await transaction.CommitAsync();
                return actualizados == 1;
            }
            catch (SqlException error) when (error.Number == 547 || error.Number == 1205)
            {
                return false;
            }
            catch (InvalidOperationException error) when (
                (error.InnerException is SqlException sqlError && sqlError.Number == 1205)
                || (error.InnerException is DbUpdateException updateError
                    && updateError.InnerException is SqlException sqlUpdate && sqlUpdate.Number == 1205))
            {
                return false;
            }
        }

        public async Task<bool> VincularUsuarioAsync(int estudianteId, int usuarioId)
        {
            try
            {
                var actualizados = await context.Estudiantes
                    .Where(estudiante => estudiante.Id == estudianteId && estudiante.UsuarioId == null)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(estudiante => estudiante.UsuarioId, usuarioId));

                return actualizados == 1;
            }
            catch (SqlException error) when (
                error.Number == 2601 || error.Number == 2627 || error.Number == 547)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int Id)
        {
            var estudiante = await context.Estudiantes.FindAsync(Id);
            if (estudiante == null) return false;

            context.Estudiantes.Remove(estudiante);

            try
            {
                await context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException error) when (
                error.InnerException is SqlException sqlError && sqlError.Number == 547)
            {
                context.Entry(estudiante).State = EntityState.Unchanged;
                return false;
            }

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
                query = query.Where(e => e.Nombre!.Contains(search)
                || e.Apellido!.Contains(search) ||
                e.Email!.Contains(search) ||
                e.Curso!.Nombre.Contains(search));
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
                "cursonombre" => orderDir == "desc" ? query.OrderByDescending(c => c.Curso!.Nombre) : query.OrderBy(c => c.Curso!.Nombre),
                "id" => orderDir == "desc" ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id),
                _ => orderDir == "desc" ? query.OrderByDescending(c => c.Nombre) : query.OrderBy(c => c.Nombre)

            };

            var data = await query
                .Skip(request.Start)
                .Take(request.Length > 0 ? request.Length : 10)
                .Select(ToTableRow).ToListAsync();
            return new DataTableResponseDto<EstudianteTableDto>
            {
                Draw = request.Draw,
                RecordsTotal = recordsTotal,
                RecordsTotalFiltered = recordsFiltered,
                Data = data
            };
        }

        public async Task<IReadOnlyList<EstudianteTableDto>> GetAllAsync() =>
            await context.Estudiantes.OrderBy(e => e.Nombre).ThenBy(e => e.Id).Select(ToTableRow).ToListAsync();

        private static readonly System.Linq.Expressions.Expression<Func<Estudiante, EstudianteTableDto>> ToTableRow =
            e => new EstudianteTableDto
            {
                Id = e.Id,
                Nombre = e.Nombre ?? string.Empty,
                Apellido = e.Apellido ?? string.Empty,
                Email = e.Email ?? string.Empty,
                CursoId = e.CursoId,
                CursoNombre = e.Curso!.Nombre
            };

        public async Task<bool> UpdateAsync(Estudiante estudiante)
        {
            context.Estudiantes.Update(estudiante);
            await context.SaveChangesAsync();
            return true;
        }
    
    }
}

