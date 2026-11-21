using System.Data;
using App_Proyecto.Data;
using App_Proyecto.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace App_Proyecto.Repositories
{
    public class DetalleHorarioRepository(AppDBContext context) : IDetalleHorarioRepository
    {
        public async Task<IReadOnlyList<DetalleHorario>> GetAllAsync(int? detalleOfertaAcademicaId)
        {
            return await context.DetallesHorario
                .AsNoTracking()
                .Where(item => !detalleOfertaAcademicaId.HasValue || item.DetalleOfertaAcademicaId == detalleOfertaAcademicaId.Value)
                .OrderBy(item => item.DiaSemana)
                .ThenBy(item => item.HoraInicio)
                .ThenBy(item => item.Id)
                .ToListAsync();
        }

        public async Task<DetalleHorario?> GetByIdAsync(int id)
        {
            return await context.DetallesHorario.FindAsync(id);
        }

        public Task<bool> CreateAsync(DetalleHorario item)
        {
            return SaveAsync(item, true);
        }

        public Task<bool> UpdateAsync(DetalleHorario item)
        {
            return SaveAsync(item, false);
        }

        private async Task<bool> SaveAsync(DetalleHorario item, bool nuevo)
        {
            await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                var grupo = await context.DetallesOfertaAcademica
                    .Include(grupo => grupo.OfertaAcademica)
                    .SingleOrDefaultAsync(grupo => grupo.Id == item.DetalleOfertaAcademicaId);
                var aula = await context.Aulas.FindAsync(item.AulaId);
                if (grupo?.OfertaAcademica == null || aula == null)
                    return false;

                if (await context.DetallesMatricula.AnyAsync(detalle => detalle.DetalleOfertaAcademicaId == grupo.Id))
                    return false;

                if (!nuevo)
                {
                    var actual = await context.DetallesHorario.AsNoTracking().SingleOrDefaultAsync(horario => horario.Id == item.Id);
                    if (actual == null || await context.DetallesMatricula.AnyAsync(detalle =>
                        detalle.DetalleOfertaAcademicaId == actual.DetalleOfertaAcademicaId))
                        return false;
                }

                if (aula.SedeId != grupo.OfertaAcademica.SedeId || aula.Capacidad < grupo.Cupo)
                    return false;

                var conflicto = await context.DetallesHorario.AnyAsync(horario =>
                    horario.Id != item.Id
                    && horario.DiaSemana == item.DiaSemana
                    && horario.HoraInicio < item.HoraFin
                    && item.HoraInicio < horario.HoraFin
                    && horario.DetalleOfertaAcademica!.OfertaAcademica!.PeriodoAcademicoId == grupo.OfertaAcademica.PeriodoAcademicoId
                    && (horario.AulaId == item.AulaId
                        || horario.DetalleOfertaAcademica.DocenteId == grupo.DocenteId
                        || horario.DetalleOfertaAcademicaId == grupo.Id));
                if (conflicto)
                    return false;

                if (nuevo)
                    context.DetallesHorario.Add(item);
                else
                    context.DetallesHorario.Update(item);

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (DbUpdateException error) when (error.InnerException is SqlException sqlError
                && (sqlError.Number == 547 || sqlError.Number == 2601 || sqlError.Number == 2627 || sqlError.Number == 1205))
            {
                context.Entry(item).State = nuevo ? EntityState.Detached : EntityState.Unchanged;
                return false;
            }
            catch (SqlException error) when (error.Number == 1205)
            {
                context.Entry(item).State = nuevo ? EntityState.Detached : EntityState.Unchanged;
                return false;
            }
            catch (InvalidOperationException error) when (
                (error.InnerException is SqlException sqlError && sqlError.Number == 1205)
                || (error.InnerException is DbUpdateException updateError
                    && updateError.InnerException is SqlException sqlUpdate && sqlUpdate.Number == 1205))
            {
                context.Entry(item).State = nuevo ? EntityState.Detached : EntityState.Unchanged;
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            DetalleHorario? item = null;
            await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                item = await context.DetallesHorario.FindAsync(id);
                if (item == null)
                    return false;

                if (await context.DetallesMatricula.AnyAsync(detalle =>
                    detalle.DetalleOfertaAcademicaId == item.DetalleOfertaAcademicaId))
                    return false;

                context.DetallesHorario.Remove(item);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (DbUpdateException error) when (error.InnerException is SqlException sqlError
                && (sqlError.Number == 547 || sqlError.Number == 1205))
            {
                if (item != null)
                    context.Entry(item).State = EntityState.Unchanged;
                return false;
            }
            catch (SqlException error) when (error.Number == 1205)
            {
                if (item != null)
                    context.Entry(item).State = EntityState.Unchanged;
                return false;
            }
            catch (InvalidOperationException error) when (
                (error.InnerException is SqlException sqlError && sqlError.Number == 1205)
                || (error.InnerException is DbUpdateException updateError
                    && updateError.InnerException is SqlException sqlUpdate && sqlUpdate.Number == 1205))
            {
                if (item != null)
                    context.Entry(item).State = EntityState.Unchanged;
                return false;
            }
        }
    }
}
