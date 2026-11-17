using App_Proyecto.Models;

namespace App_Proyecto.Repositories
{
    public interface IMatriculaRepository
    {
        Task<(int? MatriculaId, string? ErrorMessage)> CreateAsync(
            Matricula matricula,
            CancellationToken cancellationToken);
    }
}
