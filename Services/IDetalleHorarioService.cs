using App_Proyecto.DTOS;

namespace App_Proyecto.Services
{
    public interface IDetalleHorarioService
    {
        Task<IReadOnlyList<SaveDetalleHorarioDto>> GetAllAsync(int? detalleOfertaAcademicaId);
        Task<SaveDetalleHorarioDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveDetalleHorarioDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(SaveDetalleHorarioDto dto);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
