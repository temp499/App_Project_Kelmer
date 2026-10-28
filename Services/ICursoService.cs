using App_Proyecto.DTOS;

namespace App_Proyecto.Services
{
    public interface ICursoService
    {
        Task<DataTableResponseDto<CursoTableDto>> GetPagedAsync(DataTablesRequestDto request);

        Task<IReadOnlyList<CursoTableDto>> GetAllAsync();

        Task<SaveCursoDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveCursoDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(SaveCursoDto dto);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
