using App_Proyecto.Models;

namespace App_Proyecto.Repositories
{
    public interface IPeriodoAcademicoRepository
    {
        Task<IReadOnlyList<PeriodoAcademico>> GetAllAsync();
        Task<PeriodoAcademico?> GetByIdAsync(int id);
        Task CreateAsync(PeriodoAcademico periodo);
        Task UpdateAsync(PeriodoAcademico periodo);
    }
}
