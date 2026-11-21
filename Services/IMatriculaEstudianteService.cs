using App_Proyecto.DTOS;

namespace App_Proyecto.Services
{
    public interface IMatriculaEstudianteService
    {
        Task<IReadOnlyList<GrupoDisponibleDto>> GetOfertaAsync(
            int usuarioId, int periodoId, CancellationToken cancellationToken);

        Task<IReadOnlyList<MatriculaConsultaDto>> GetHistorialAsync(int usuarioId, CancellationToken cancellationToken);

        Task<(int? MatriculaId, string? ErrorMessage)> CreateAsync(
            int usuarioId, CrearMatriculaDto dto, CancellationToken cancellationToken);

        Task<PerfilEstudianteDto?> GetPerfilAsync(int usuarioId, CancellationToken cancellationToken);

        Task<MatriculaConsultaDto?> GetByIdAsync(
            int usuarioId, int matriculaId, CancellationToken cancellationToken);
    }
}
