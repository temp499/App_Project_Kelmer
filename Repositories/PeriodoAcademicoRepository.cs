using App_Proyecto.Data;
using App_Proyecto.Models;
using Microsoft.EntityFrameworkCore;

namespace App_Proyecto.Repositories
{
    public class PeriodoAcademicoRepository(AppDBContext context) : IPeriodoAcademicoRepository
    {
        public async Task<IReadOnlyList<PeriodoAcademico>> GetAllAsync()
        {
            return await context.PeriodosAcademicos
                .AsNoTracking()
                .OrderByDescending(periodo => periodo.Id)
                .ToListAsync();
        }

        public async Task<PeriodoAcademico?> GetByIdAsync(int id)
        {
            return await context.PeriodosAcademicos.FindAsync(id);
        }

        public async Task CreateAsync(PeriodoAcademico periodo)
        {
            context.PeriodosAcademicos.Add(periodo);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PeriodoAcademico periodo)
        {
            context.PeriodosAcademicos.Update(periodo);
            await context.SaveChangesAsync();
        }
    }
}
