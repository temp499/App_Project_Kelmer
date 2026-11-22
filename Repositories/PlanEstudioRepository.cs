using App_Proyecto.Data;
using App_Proyecto.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace App_Proyecto.Repositories
{
    public class PlanEstudioRepository(AppDBContext context) : IPlanEstudioRepository
    {
        public async Task<IReadOnlyList<PlanEstudio>> GetAllAsync()
        {
            return await context.PlanesEstudio
                .AsNoTracking()
                .OrderBy(item => item.Nombre)
                .ThenBy(item => item.Id)
                .ToListAsync();
        }

        public async Task<PlanEstudio?> GetByIdAsync(int id)
        {
            return await context.PlanesEstudio.FindAsync(id);
        }

        public async Task<bool> CreateAsync(PlanEstudio item)
        {
            context.PlanesEstudio.Add(item);
            try
            {
                await context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException error) when (
                error.InnerException is SqlException sqlError && sqlError.Number == 547)
            {
                context.Entry(item).State = EntityState.Detached;
                return false;
            }
        }

        public async Task<bool> UpdateAsync(PlanEstudio item)
        {
            await using var transaction = await context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            try
            {
                var actual = await context.PlanesEstudio.AsNoTracking().SingleOrDefaultAsync(value => value.Id == item.Id);
                if (actual == null)
                    return false;

                if (actual.CarreraId != item.CarreraId
                    && await context.OfertasAcademicas.AnyAsync(oferta => oferta.PlanEstudioId == item.Id))
                    return false;

                context.PlanesEstudio.Update(item);
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
            var item = await context.PlanesEstudio.FindAsync(id);
            if (item == null)
                return false;

            context.PlanesEstudio.Remove(item);
            try
            {
                await context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException error) when (
                error.InnerException is SqlException sqlError && sqlError.Number == 547)
            {
                context.Entry(item).State = EntityState.Unchanged;
                return false;
            }
        }
    }
}
