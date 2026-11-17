using System.Data;
using App_Proyecto.Data;
using App_Proyecto.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace App_Proyecto.Repositories
{
    public class MatriculaRepository(AppDBContext context) : IMatriculaRepository
    {
        private const int DuplicateIndexError = 2601;

        public async Task<(int? MatriculaId, string? ErrorMessage)> CreateAsync(
            Matricula matricula,
            CancellationToken cancellationToken)
        {
            await using var transaction = await context.Database.BeginTransactionAsync(
                IsolationLevel.RepeatableRead, cancellationToken);

            var matriculaAbierta = await context.PeriodosAcademicos
                .Where(periodo => periodo.Id == matricula.PeriodoAcademicoId)
                .Select(periodo => (bool?)periodo.MatriculaAbierta)
                .SingleOrDefaultAsync(cancellationToken);

            if (matriculaAbierta is null)
                return (null, "El período académico no existe");

            if (matriculaAbierta == false)
                return (null, "La matrícula está cerrada para este período");

            if (!await context.Estudiantes.AnyAsync(e => e.Id == matricula.EstudianteId, cancellationToken))
                return (null, "El estudiante no existe");

            var cursoIds = matricula.Detalles.Select(detalle => detalle.CursoId).ToArray();
            var cursosEncontrados = await context.Cursos
                .CountAsync(curso => cursoIds.Contains(curso.Id), cancellationToken);

            if (cursosEncontrados != cursoIds.Length)
                return (null, "Uno o más cursos seleccionados no existen");

            context.Matriculas.Add(matricula);

            try
            {
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException error) when (
                error.InnerException is SqlException sqlError && sqlError.Number == DuplicateIndexError)
            {
                foreach (var detalle in matricula.Detalles.ToArray())
                    context.Entry(detalle).State = EntityState.Detached;

                context.Entry(matricula).State = EntityState.Detached;
                return (null, "El estudiante ya tiene matrícula en este período");
            }

            await transaction.CommitAsync(cancellationToken);
            return (matricula.Id, null);
        }
    }
}
