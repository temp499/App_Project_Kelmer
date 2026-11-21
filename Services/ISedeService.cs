using App_Proyecto.DTOS;

namespace App_Proyecto.Services
{
    public interface ISedeService
    {
        Task<IReadOnlyList<SaveSedeDto>> GetAllAsync();
        Task<SaveSedeDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveSedeDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(SaveSedeDto dto);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
