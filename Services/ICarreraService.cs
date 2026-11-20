using App_Proyecto.DTOS;

namespace App_Proyecto.Services
{
    public interface ICarreraService
    {
        Task<IReadOnlyList<SaveCarreraDto>> GetAllAsync();
        Task<SaveCarreraDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveCarreraDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(SaveCarreraDto dto);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
