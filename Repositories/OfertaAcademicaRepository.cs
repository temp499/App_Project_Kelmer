using App_Proyecto.Data;
using App_Proyecto.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace App_Proyecto.Repositories
{
    public class OfertaAcademicaRepository(AppDBContext context) : IOfertaAcademicaRepository
    {
        public async Task<IReadOnlyList<OfertaAcademica>> GetAllAsync()
        {
            return await context.OfertasAcademicas
                .AsNoTracking()
                .OrderBy(item => item.Nombre)
                .ThenBy(item => item.Id)
                .ToListAsync();
        }

        public async Task<OfertaAcademica?> GetByIdAsync(int id)
        {
            return await context.OfertasAcademicas.FindAsync(id);
        }

        public async Task<bool> CreateAsync(OfertaAcademica item)
        {
            context.OfertasAcademicas.Add(item);
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

        public async Task<bool> UpdateAsync(OfertaAcademica item)
        {
            context.OfertasAcademicas.Update(item);
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
            var item = await context.OfertasAcademicas.FindAsync(id);
            if (item == null)
                return false;

            context.OfertasAcademicas.Remove(item);
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
