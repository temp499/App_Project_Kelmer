using App_Proyecto.Data;
using App_Proyecto.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace App_Proyecto.Repositories
{
    public class AulaRepository(AppDBContext context) : IAulaRepository
    {
        public async Task<IReadOnlyList<Aula>> GetAllAsync()
        {
            return await context.Aulas
                .AsNoTracking()
                .OrderBy(item => item.Nombre)
                .ThenBy(item => item.Id)
                .ToListAsync();
        }

        public async Task<Aula?> GetByIdAsync(int id)
        {
            return await context.Aulas.FindAsync(id);
        }

        public async Task<bool> CreateAsync(Aula item)
        {
            context.Aulas.Add(item);
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

        public async Task<bool> UpdateAsync(Aula item)
        {
            await using var transaction = await context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            try
            {
                var actual = await context.Aulas.AsNoTracking().SingleOrDefaultAsync(value => value.Id == item.Id);
                if (actual == null)
                    return false;

                var horarios = context.DetallesHorario.Where(horario => horario.AulaId == item.Id);
                if (actual.SedeId != item.SedeId && await horarios.AnyAsync())
                    return false;

                if (await horarios.AnyAsync(horario => horario.DetalleOfertaAcademica!.Cupo > item.Capacidad))
                    return false;

                context.Aulas.Update(item);
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
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await context.Aulas.FindAsync(id);
            if (item == null)
                return false;

            context.Aulas.Remove(item);
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
