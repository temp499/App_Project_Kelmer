using App_Proyecto.DTOS;
using App_Proyecto.Models;

namespace App_Proyecto.Repositories
{
    public interface IMatriculaRepository
    {
        Task<IReadOnlyList<GrupoDisponibleDto>> GetOfertaAsync(
            int carreraId, int periodoId, CancellationToken cancellationToken);

        Task<IReadOnlyList<int>> GetIdsEstudianteAsync(int estudianteId, CancellationToken cancellationToken);

        Task<Matricula?> GetByIdAsync(int id, CancellationToken cancellationToken);

        Task<(int? MatriculaId, string? ErrorMessage)> CreateAsync(
            Matricula matricula,
            int[] grupoIds,
            CancellationToken cancellationToken);
    }
}
