using App_Proyecto.DTOS;

namespace App_Proyecto.Services
{
    public interface IMatriculaService
    {
        Task<MatriculaConsultaDto?> GetByIdAsync(
            int id, CancellationToken cancellationToken = default);

        Task<(int? MatriculaId, string? ErrorMessage)> CreateAsync(
            int estudianteId,
            CrearMatriculaDto dto,
            CancellationToken cancellationToken = default);
    }
}
