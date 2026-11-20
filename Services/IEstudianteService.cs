using App_Proyecto.DTOS;

namespace App_Proyecto.Services
{
    public interface IEstudianteService
    {
        Task<DataTableResponseDto<EstudianteTableDto>> GetPagedAsync(DataTablesRequestDto request);

        Task<IReadOnlyList<EstudianteTableDto>> GetAllAsync();

        Task<SaveEstudianteDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveEstudianteDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(SaveEstudianteDto dto);
        Task<(bool Success, string? ErrorMessage)> VincularUsuarioAsync(int estudianteId, int usuarioId);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
