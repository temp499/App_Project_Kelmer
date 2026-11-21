using App_Proyecto.Models;

namespace App_Proyecto.Repositories
{
    public interface IPlanEstudioRepository
    {
        Task<IReadOnlyList<PlanEstudio>> GetAllAsync();
        Task<PlanEstudio?> GetByIdAsync(int id);
        Task<bool> CreateAsync(PlanEstudio item);
        Task<bool> UpdateAsync(PlanEstudio item);
        Task<bool> DeleteAsync(int id);
    }
}
