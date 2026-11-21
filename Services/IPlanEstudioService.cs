using App_Proyecto.DTOS;

namespace App_Proyecto.Services
{
    public interface IPlanEstudioService
    {
        Task<IReadOnlyList<SavePlanEstudioDto>> GetAllAsync();
        Task<SavePlanEstudioDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreateAsync(SavePlanEstudioDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(SavePlanEstudioDto dto);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
