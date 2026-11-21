using App_Proyecto.DTOS;

namespace App_Proyecto.Services
{
    public interface IAulaService
    {
        Task<IReadOnlyList<SaveAulaDto>> GetAllAsync();
        Task<SaveAulaDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveAulaDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(SaveAulaDto dto);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
