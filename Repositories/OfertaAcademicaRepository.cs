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
            await using var transaction = await context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            try
            {
                var actual = await context.OfertasAcademicas.AsNoTracking().SingleOrDefaultAsync(value => value.Id == item.Id);
                if (actual == null)
                    return false;

                if ((actual.PlanEstudioId != item.PlanEstudioId
                        || actual.PeriodoAcademicoId != item.PeriodoAcademicoId
                        || actual.SedeId != item.SedeId)
                    && await context.DetallesOfertaAcademica.AnyAsync(grupo => grupo.OfertaAcademicaId == item.Id))
                    return false;

                context.OfertasAcademicas.Update(item);
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
