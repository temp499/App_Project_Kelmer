using App_Proyecto.Data;
using App_Proyecto.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace App_Proyecto.Repositories
{
    public class CarreraRepository(AppDBContext context) : ICarreraRepository
    {
        public async Task<IReadOnlyList<Carrera>> GetAllAsync()
        {
            return await context.Carreras
                .AsNoTracking()
                .OrderBy(item => item.Nombre)
                .ThenBy(item => item.Id)
                .ToListAsync();
        }

        public async Task<Carrera?> GetByIdAsync(int id)
        {
            return await context.Carreras.FindAsync(id);
        }

        public async Task<bool> CreateAsync(Carrera item)
        {
            context.Carreras.Add(item);
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

        public async Task<bool> UpdateAsync(Carrera item)
        {
            context.Carreras.Update(item);
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
            var item = await context.Carreras.FindAsync(id);
            if (item == null)
                return false;

            context.Carreras.Remove(item);
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
