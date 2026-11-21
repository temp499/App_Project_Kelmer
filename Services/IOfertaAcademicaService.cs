using App_Proyecto.DTOS;

namespace App_Proyecto.Services
{
    public interface IOfertaAcademicaService
    {
        Task<IReadOnlyList<SaveOfertaAcademicaDto>> GetAllAsync();
        Task<SaveOfertaAcademicaDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveOfertaAcademicaDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(SaveOfertaAcademicaDto dto);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
