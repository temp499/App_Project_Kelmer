using App_Proyecto.DTOS;

namespace App_Proyecto.Services
{
    public interface IDocenteService
    {
        Task<IReadOnlyList<SaveDocenteDto>> GetAllAsync();
        Task<SaveDocenteDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveDocenteDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(SaveDocenteDto dto);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
