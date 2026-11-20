using App_Proyecto.Data;
using App_Proyecto.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace App_Proyecto.Repositories
{
    public class FacultadRepository(AppDBContext context) : IFacultadRepository
    {
        public async Task<IReadOnlyList<Facultad>> GetAllAsync()
        {
            return await context.Facultades
                .AsNoTracking()
                .OrderBy(item => item.Nombre)
                .ThenBy(item => item.Id)
                .ToListAsync();
        }

        public async Task<Facultad?> GetByIdAsync(int id)
        {
            return await context.Facultades.FindAsync(id);
        }

        public async Task<bool> CreateAsync(Facultad item)
        {
            context.Facultades.Add(item);
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

        public async Task<bool> UpdateAsync(Facultad item)
        {
            context.Facultades.Update(item);
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
            var item = await context.Facultades.FindAsync(id);
            if (item == null)
                return false;

            context.Facultades.Remove(item);
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
