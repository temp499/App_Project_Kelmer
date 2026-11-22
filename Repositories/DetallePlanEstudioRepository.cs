using App_Proyecto.Data;
using App_Proyecto.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace App_Proyecto.Repositories
{
    public class DetallePlanEstudioRepository(AppDBContext context) : IDetallePlanEstudioRepository
    {
        public async Task<IReadOnlyList<DetallePlanEstudio>> GetAllAsync(int? planEstudioId)
        {
            return await context.DetallesPlanEstudio
                .AsNoTracking()
                .Where(item => !planEstudioId.HasValue || item.PlanEstudioId == planEstudioId.Value)
                .OrderBy(item => item.Semestre)
                .ThenBy(item => item.Id)
                .ToListAsync();
        }

        public async Task<DetallePlanEstudio?> GetByIdAsync(int id)
        {
            return await context.DetallesPlanEstudio.FindAsync(id);
        }

        public async Task<bool> CreateAsync(DetallePlanEstudio item)
        {
            context.DetallesPlanEstudio.Add(item);
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

        public async Task<bool> UpdateAsync(DetallePlanEstudio item)
        {
            await using var transaction = await context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            try
            {
                var actual = await context.DetallesPlanEstudio.AsNoTracking().SingleOrDefaultAsync(value => value.Id == item.Id);
                if (actual == null)
                    return false;

                if ((actual.PlanEstudioId != item.PlanEstudioId || actual.CursoId != item.CursoId)
                    && await context.DetallesOfertaAcademica.AnyAsync(grupo =>
                        grupo.CursoId == actual.CursoId && grupo.OfertaAcademica!.PlanEstudioId == actual.PlanEstudioId))
                    return false;

                context.DetallesPlanEstudio.Update(item);
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
            catch (InvalidOperationException error) when (
                (error.InnerException is SqlException sqlError && sqlError.Number == 1205)
                || (error.InnerException is DbUpdateException updateError
                    && updateError.InnerException is SqlException sqlUpdate && sqlUpdate.Number == 1205))
            {
                context.Entry(item).State = EntityState.Unchanged;
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            DetallePlanEstudio? item = null;
            await using var transaction = await context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            try
            {
                item = await context.DetallesPlanEstudio.FindAsync(id);
                if (item == null)
                    return false;

                if (await context.DetallesOfertaAcademica.AnyAsync(grupo =>
                    grupo.CursoId == item.CursoId && grupo.OfertaAcademica!.PlanEstudioId == item.PlanEstudioId))
                    return false;

                context.DetallesPlanEstudio.Remove(item);
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
