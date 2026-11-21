using App_Proyecto.Models;

namespace App_Proyecto.Repositories
{
    public interface IOfertaAcademicaRepository
    {
        Task<IReadOnlyList<OfertaAcademica>> GetAllAsync();
        Task<OfertaAcademica?> GetByIdAsync(int id);
        Task<bool> CreateAsync(OfertaAcademica item);
        Task<bool> UpdateAsync(OfertaAcademica item);
        Task<bool> DeleteAsync(int id);
    }
}
