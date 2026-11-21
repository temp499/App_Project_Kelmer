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
            context.PlanesEstudio.Update(item);
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
