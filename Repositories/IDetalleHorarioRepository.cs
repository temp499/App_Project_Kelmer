using App_Proyecto.Models;

namespace App_Proyecto.Repositories
{
    public interface IDetalleHorarioRepository
    {
        Task<IReadOnlyList<DetalleHorario>> GetAllAsync(int? detalleOfertaAcademicaId);
        Task<DetalleHorario?> GetByIdAsync(int id);
        Task<bool> CreateAsync(DetalleHorario item);
        Task<bool> UpdateAsync(DetalleHorario item);
        Task<bool> DeleteAsync(int id);
    }
}
