using App_Proyecto.Models;

namespace App_Proyecto.Repositories
{
    public interface IDetalleOfertaAcademicaRepository
    {
        Task<IReadOnlyList<DetalleOfertaAcademica>> GetAllAsync(int? ofertaAcademicaId);
        Task<DetalleOfertaAcademica?> GetByIdAsync(int id);
        Task<bool> CreateAsync(DetalleOfertaAcademica item);
        Task<bool> UpdateAsync(DetalleOfertaAcademica item);
        Task<bool> DeleteAsync(int id);
    }
}
