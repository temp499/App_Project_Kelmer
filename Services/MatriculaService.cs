using App_Proyecto.DTOS;
using App_Proyecto.Models;
using App_Proyecto.Repositories;

namespace App_Proyecto.Services
{
    public class MatriculaService(IMatriculaRepository repository) : IMatriculaService
    {
        public async Task<(int? MatriculaId, string? ErrorMessage)> CreateAsync(
            int estudianteId,
            CrearMatriculaDto dto,
            CancellationToken cancellationToken = default)
        {
            if (estudianteId <= 0)
                return (null, "El estudiante no es válido");

            if (dto.PeriodoAcademicoId <= 0)
                return (null, "Selecciona un período académico");

            var cursoIds = dto.CursoIds;
            if (cursoIds is null || cursoIds.Length == 0)
                return (null, "Selecciona al menos un curso");

            if (cursoIds.Any(id => id <= 0))
                return (null, "La selección contiene un curso no válido");

            if (cursoIds.Distinct().Count() != cursoIds.Length)
                return (null, "No puedes seleccionar el mismo curso más de una vez");

            var matricula = new Matricula
            {
                EstudianteId = estudianteId,
                PeriodoAcademicoId = dto.PeriodoAcademicoId,
                Detalles = cursoIds
                    .Select(id => new DetalleMatricula { CursoId = id })
                    .ToList()
            };

            return await repository.CreateAsync(matricula, cancellationToken);
        }
    }
}
