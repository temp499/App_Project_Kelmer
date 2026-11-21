using App_Proyecto.DTOS;

namespace App_Proyecto.Services
{
    public interface IDetallePlanEstudioService
    {
        Task<IReadOnlyList<SaveDetallePlanEstudioDto>> GetAllAsync(int? planEstudioId);
        Task<SaveDetallePlanEstudioDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveDetallePlanEstudioDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(SaveDetallePlanEstudioDto dto);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
