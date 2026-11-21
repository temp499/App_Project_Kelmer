using App_Proyecto.Data;
using App_Proyecto.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace App_Proyecto.Repositories
{
    public class DetalleOfertaAcademicaRepository(AppDBContext context) : IDetalleOfertaAcademicaRepository
    {
        public async Task<IReadOnlyList<DetalleOfertaAcademica>> GetAllAsync(int? ofertaAcademicaId)
        {
            return await context.DetallesOfertaAcademica
                .AsNoTracking()
                .Where(item => !ofertaAcademicaId.HasValue || item.OfertaAcademicaId == ofertaAcademicaId.Value)
                .OrderBy(item => item.Grupo)
                .ThenBy(item => item.Id)
                .ToListAsync();
        }

        public async Task<DetalleOfertaAcademica?> GetByIdAsync(int id)
        {
            return await context.DetallesOfertaAcademica.FindAsync(id);
        }

        public async Task<bool> CreateAsync(DetalleOfertaAcademica item)
        {
            context.DetallesOfertaAcademica.Add(item);
            try
            {
                await context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException error) when (
                error.InnerException is SqlException sqlError && (sqlError.Number == 547 || sqlError.Number == 2601 || sqlError.Number == 2627))
            {
                context.Entry(item).State = EntityState.Detached;
                return false;
            }
        }

        public async Task<bool> UpdateAsync(DetalleOfertaAcademica item)
        {
            await using var transaction = await context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            try
            {
                var actual = await context.DetallesOfertaAcademica.AsNoTracking().SingleOrDefaultAsync(value => value.Id == item.Id);
                if (actual == null)
                    return false;

                var inscritos = await context.DetallesMatricula
                    .CountAsync(detalle => detalle.DetalleOfertaAcademicaId == item.Id);
                if (item.Cupo < inscritos)
                    return false;

                var horarios = context.DetallesHorario.Where(horario => horario.DetalleOfertaAcademicaId == item.Id);
                var cambioRelacion = actual.OfertaAcademicaId != item.OfertaAcademicaId
                    || actual.CursoId != item.CursoId || actual.DocenteId != item.DocenteId;
                if (cambioRelacion && (inscritos > 0 || await horarios.AnyAsync()))
                    return false;

                if (await horarios.AnyAsync(horario => horario.Aula!.Capacidad < item.Cupo))
                    return false;

                context.DetallesOfertaAcademica.Update(item);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (DbUpdateException error) when (error.InnerException is SqlException sqlError
                && (sqlError.Number == 547 || sqlError.Number == 2601 || sqlError.Number == 2627 || sqlError.Number == 1205))
            {
                context.Entry(item).State = EntityState.Unchanged;
                return false;
            }
            catch (SqlException error) when (error.Number == 1205)
            {
                context.Entry(item).State = EntityState.Unchanged;
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await context.DetallesOfertaAcademica.FindAsync(id);
            if (item == null)
                return false;

            context.DetallesOfertaAcademica.Remove(item);
            try
            {
                await context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException error) when (
                error.InnerException is SqlException sqlError && (sqlError.Number == 547 || sqlError.Number == 2601 || sqlError.Number == 2627))
            {
                context.Entry(item).State = EntityState.Unchanged;
                return false;
            }
        }
    }
}
