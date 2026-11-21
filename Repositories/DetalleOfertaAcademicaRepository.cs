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
            context.DetallesOfertaAcademica.Update(item);
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
