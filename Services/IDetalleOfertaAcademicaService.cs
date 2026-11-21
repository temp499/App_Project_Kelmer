using App_Proyecto.DTOS;

namespace App_Proyecto.Services
{
    public interface IDetalleOfertaAcademicaService
    {
        Task<IReadOnlyList<SaveDetalleOfertaAcademicaDto>> GetAllAsync(int? ofertaAcademicaId);
        Task<SaveDetalleOfertaAcademicaDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveDetalleOfertaAcademicaDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(SaveDetalleOfertaAcademicaDto dto);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
