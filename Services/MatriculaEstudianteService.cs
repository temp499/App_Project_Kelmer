using App_Proyecto.DTOS;
using App_Proyecto.Repositories;

namespace App_Proyecto.Services
{
    public class MatriculaEstudianteService(
        IEstudianteRepository estudiantes,
        IMatriculaService matriculas) : IMatriculaEstudianteService
    {
        public async Task<(int? MatriculaId, string? ErrorMessage)> CreateAsync(
            int usuarioId, CrearMatriculaDto dto, CancellationToken cancellationToken)
        {
            if (usuarioId <= 0)
                return (null, "La cuenta no es valida");

            var estudiante = await estudiantes.GetByUsuarioIdAsync(usuarioId, cancellationToken);
            if (estudiante == null)
                return (null, "Tu cuenta no tiene un estudiante vinculado");

            return await matriculas.CreateAsync(estudiante.Id, dto, cancellationToken);
        }

        public async Task<MatriculaConsultaDto?> GetByIdAsync(
            int usuarioId, int matriculaId, CancellationToken cancellationToken)
        {
            if (usuarioId <= 0 || matriculaId <= 0)
                return null;

            var estudiante = await estudiantes.GetByUsuarioIdAsync(usuarioId, cancellationToken);
            if (estudiante == null)
                return null;

            var matricula = await matriculas.GetByIdAsync(matriculaId, cancellationToken);
            if (matricula == null || matricula.EstudianteId != estudiante.Id)
                return null;

            return matricula;
        }
    }
}
