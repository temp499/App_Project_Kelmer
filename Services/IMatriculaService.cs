using App_Proyecto.DTOS;

namespace App_Proyecto.Services
{
    public interface IMatriculaService
    {
        Task<(int? MatriculaId, string? ErrorMessage)> CreateAsync(
            int estudianteId,
            CrearMatriculaDto dto,
            CancellationToken cancellationToken = default);
    }
}
