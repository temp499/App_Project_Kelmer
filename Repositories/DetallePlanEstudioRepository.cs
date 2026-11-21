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
            context.DetallesPlanEstudio.Update(item);
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

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await context.DetallesPlanEstudio.FindAsync(id);
            if (item == null)
                return false;

            context.DetallesPlanEstudio.Remove(item);
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
