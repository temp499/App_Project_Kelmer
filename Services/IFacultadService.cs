using App_Proyecto.DTOS;

namespace App_Proyecto.Services
{
    public interface IFacultadService
    {
        Task<IReadOnlyList<SaveFacultadDto>> GetAllAsync();
        Task<SaveFacultadDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveFacultadDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(SaveFacultadDto dto);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
