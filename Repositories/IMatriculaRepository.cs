using App_Proyecto.Models;

namespace App_Proyecto.Repositories
{
    public interface IMatriculaRepository
    {
        Task<Matricula?> GetByIdAsync(int id, CancellationToken cancellationToken);

        Task<(int? MatriculaId, string? ErrorMessage)> CreateAsync(
            Matricula matricula,
            CancellationToken cancellationToken);
    }
}
