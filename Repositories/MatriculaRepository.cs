using System.Data;
using App_Proyecto.Data;
using App_Proyecto.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using App_Proyecto.DTOS;
using static App_Proyecto.DTOS.HistorialMatriculaDto;

namespace App_Proyecto.Repositories
{
    public class MatriculaRepository(AppDBContext context) : IMatriculaRepository
    {
        private const int DuplicateIndexError = 2601;

        public async Task<Matricula?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await context.Matriculas
                .OrderByDescending(matricula => matricula.Id)
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

        public async Task<HistorialMatriculaDto?> GetHistorialAsync(
            ConsultaHistorialDto consulta, CancellationToken cancellationToken)

        {
            var estudiante = await context.Estudiantes
                 .AsNoTracking()
                 .Where(e => e.Id == consulta.EstudianteId)
                 .Select(e => new { e.Id, e.Nombre, e.Apellido })
                 .SingleOrDefaultAsync(cancellationToken);

            if (estudiante == null)
                return null;

            var query = context.Matriculas
                .AsNoTracking()
                .Where(matricula => matricula.EstudianteId == consulta.EstudianteId);

            var total = await query.CountAsync(cancellationToken);
            var registrosOmitidos = (consulta.Pagina - 1) * consulta.TamañoPagina;

            var matriculas = await query
                .OrderByDescending(matricula => matricula.Id)
                .Skip(registrosOmitidos)
                .Take(consulta.TamañoPagina)
                .Select(matricula => new MatriculaResumenDto
                {
                    Id = matricula.Id,
                    PeriodoAcademicoId = matricula.PeriodoAcademicoId,
                    PeriodoNombre = matricula.PeriodoAcademico.Nombre,
                    TotalCursos = matricula.Detalles.Count
                })
                .ToListAsync(cancellationToken);

            return new HistorialMatriculaDto
            {
                EstudianteId = estudiante.Id,
                EstudianteNombre = $"{estudiante.Nombre} {estudiante.Apellido}",
                Pagina = consulta.Pagina,
                TamañoPagina = consulta.TamañoPagina,
                TotalRegistros = total,
                Matriculas = matriculas

            };

        }     
    }
}
