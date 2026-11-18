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

        public async Task<Matricula?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await context.Matriculas
                .AsNoTracking()
                .Include(matricula => matricula.Estudiante)
                .Include(matricula => matricula.PeriodoAcademico)
                .Include(matricula => matricula.Detalles)
                    .ThenInclude(detalle => detalle.Curso)
                .SingleOrDefaultAsync(matricula => matricula.Id == id, cancellationToken);
        }

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
                return (null, "El periodo academico no existe");

            if (matriculaAbierta == false)
                return (null, "La matricula esta cerrada para este periodo");

            if (!await context.Estudiantes.AnyAsync(e => e.Id == matricula.EstudianteId, cancellationToken))
                return (null, "El estudiante no existe");

            var cursoIds = matricula.Detalles.Select(detalle => detalle.CursoId).ToArray();
            var cursosEncontrados = await context.Cursos
                .CountAsync(curso => cursoIds.Contains(curso.Id), cancellationToken);

            if (cursosEncontrados != cursoIds.Length)
                return (null, "Uno o mas cursos seleccionados no existen");

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
                return (null, "El estudiante ya tiene matricula en este periodo");
            }

            await transaction.CommitAsync(cancellationToken);
            return (matricula.Id, null);
        }
    }
}
