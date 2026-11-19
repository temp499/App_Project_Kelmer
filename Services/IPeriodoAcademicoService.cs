using App_Proyecto.DTOS;

namespace App_Proyecto.Services
{
    public interface IPeriodoAcademicoService
    {
        Task<IReadOnlyList<SavePeriodoAcademicoDto>> GetAllAsync();
        Task<(bool Success, string? ErrorMessage)> CreateAsync(SavePeriodoAcademicoDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(SavePeriodoAcademicoDto dto);
    }
}
