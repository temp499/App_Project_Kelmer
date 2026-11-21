using App_Proyecto.Models;

namespace App_Proyecto.Repositories
{
    public interface IDetallePlanEstudioRepository
    {
        Task<IReadOnlyList<DetallePlanEstudio>> GetAllAsync(int? planEstudioId);
        Task<DetallePlanEstudio?> GetByIdAsync(int id);
        Task<bool> CreateAsync(DetallePlanEstudio item);
        Task<bool> UpdateAsync(DetallePlanEstudio item);
        Task<bool> DeleteAsync(int id);
    }
}
