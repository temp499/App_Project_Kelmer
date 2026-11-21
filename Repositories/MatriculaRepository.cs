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
        public async Task<IReadOnlyList<DTOS.GrupoDisponibleDto>> GetOfertaAsync(
            int carreraId, int periodoId, CancellationToken cancellationToken)
        {
            return await context.DetallesOfertaAcademica.AsNoTracking()
                .Where(grupo => grupo.OfertaAcademica!.Activa
                    && grupo.OfertaAcademica.PeriodoAcademicoId == periodoId
                    && grupo.OfertaAcademica.PeriodoAcademico!.MatriculaAbierta
                    && grupo.OfertaAcademica.PlanEstudio!.Activo
                    && grupo.OfertaAcademica.PlanEstudio.CarreraId == carreraId
                    && grupo.Horarios.Any()
                    && context.DetallesPlanEstudio.Any(detalle =>
                        detalle.PlanEstudioId == grupo.OfertaAcademica.PlanEstudioId
                        && detalle.CursoId == grupo.CursoId))
                .OrderBy(grupo => grupo.Curso!.Nombre).ThenBy(grupo => grupo.Grupo).ThenBy(grupo => grupo.Id)
                .Select(grupo => new DTOS.GrupoDisponibleDto
                {
                    Id = grupo.Id,
                    CursoId = grupo.CursoId,
                    CursoNombre = grupo.Curso!.Nombre,
                    Grupo = grupo.Grupo,
                    Sede = grupo.OfertaAcademica!.Sede!.Nombre,
                    Docente = grupo.Docente!.Nombre + " " + grupo.Docente.Apellido,
                    Cupo = grupo.Cupo,
                    Inscritos = context.DetallesMatricula.Count(detalle => detalle.DetalleOfertaAcademicaId == grupo.Id),
                    Horarios = grupo.Horarios.OrderBy(horario => horario.DiaSemana).ThenBy(horario => horario.HoraInicio)
                        .Select(horario => new DTOS.HorarioGrupoDto
                        {
                            DiaSemana = horario.DiaSemana,
                            HoraInicio = horario.HoraInicio,
                            HoraFin = horario.HoraFin,
                            Aula = horario.Aula!.Nombre
                        }).ToList()
                }).ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<int>> GetIdsEstudianteAsync(int estudianteId, CancellationToken cancellationToken)
        {
            return await context.Matriculas.AsNoTracking()
                .Where(matricula => matricula.EstudianteId == estudianteId)
                .OrderByDescending(matricula => matricula.Id)
                .Select(matricula => matricula.Id).ToListAsync(cancellationToken);
        }

        public async Task<Matricula?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await context.Matriculas
                .OrderByDescending(matricula => matricula.Id)
                .AsNoTracking()
                .Include(matricula => matricula.Estudiante)
                .Include(matricula => matricula.PeriodoAcademico)
                .Include(matricula => matricula.Detalles)
                    .ThenInclude(detalle => detalle.Curso)
                .Include(matricula => matricula.Detalles)
                    .ThenInclude(detalle => detalle.DetalleOfertaAcademica)
                        .ThenInclude(grupo => grupo!.Horarios)
                            .ThenInclude(horario => horario.Aula)
                .SingleOrDefaultAsync(matricula => matricula.Id == id, cancellationToken);
        }

        public async Task<(int? MatriculaId, string? ErrorMessage)> CreateAsync(
            Matricula matricula, int[] grupoIds, CancellationToken cancellationToken)
        {
            await using var transaction = await context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable, cancellationToken);
            try
            {
                var abierta = await context.PeriodosAcademicos.AnyAsync(periodo =>
                    periodo.Id == matricula.PeriodoAcademicoId && periodo.MatriculaAbierta, cancellationToken);
                if (!abierta)
                    return (null, "El periodo no existe o la matricula esta cerrada");

                var estudiante = await context.Estudiantes.AsNoTracking()
                    .SingleOrDefaultAsync(item => item.Id == matricula.EstudianteId, cancellationToken);
                if (estudiante?.CarreraId == null)
                    return (null, "El estudiante no existe o no tiene una carrera asignada");

                if (await context.Matriculas.AnyAsync(item => item.EstudianteId == estudiante.Id
                    && item.PeriodoAcademicoId == matricula.PeriodoAcademicoId, cancellationToken))
                    return (null, "El estudiante ya tiene matricula en este periodo");

                var cursoIds = matricula.Detalles.Select(detalle => detalle.CursoId).ToArray();
                var query = context.DetallesOfertaAcademica
                    .Include(grupo => grupo.OfertaAcademica)
                        .ThenInclude(oferta => oferta!.PlanEstudio)
                    .Include(grupo => grupo.Horarios)
                    .Where(grupo => grupo.OfertaAcademica!.Activa
                        && grupo.OfertaAcademica.PeriodoAcademicoId == matricula.PeriodoAcademicoId
                        && grupo.OfertaAcademica.PlanEstudio!.Activo
                        && grupo.OfertaAcademica.PlanEstudio.CarreraId == estudiante.CarreraId);

                if (grupoIds.Length > 0)
                    query = query.Where(grupo => grupoIds.Contains(grupo.Id));
                else
                    query = query.Where(grupo => cursoIds.Contains(grupo.CursoId));

                var disponibles = await query.OrderBy(grupo => grupo.Id).ToListAsync(cancellationToken);
                var seleccionados = new List<DetalleOfertaAcademica>();
                if (grupoIds.Length > 0)
                {
                    if (disponibles.Count != grupoIds.Length)
                        return (null, "Uno o mas grupos no estan disponibles para tu carrera y periodo");

                    seleccionados = disponibles;
                }
                else
                {
                    foreach (var cursoId in cursoIds)
                    {
                        var opciones = disponibles.Where(grupo => grupo.CursoId == cursoId).ToList();
                        if (opciones.Count == 0)
                            return (null, "Uno de los cursos no tiene oferta para tu carrera y periodo");
                        if (opciones.Count > 1)
                            return (null, "Selecciona el grupo de cada curso que tiene varias opciones");

                        seleccionados.Add(opciones[0]);
                    }
                }

                if (seleccionados.Count == 0)
                    return (null, "Selecciona al menos un curso o grupo");

                if (seleccionados.Select(grupo => grupo.CursoId).Distinct().Count() != seleccionados.Count)
                    return (null, "No puedes matricular dos grupos del mismo curso");

                if (cursoIds.Length > 0 && (cursoIds.Length != seleccionados.Count
                    || seleccionados.Any(grupo => !cursoIds.Contains(grupo.CursoId))))
                    return (null, "Los cursos no coinciden con los grupos seleccionados");

                foreach (var grupo in seleccionados)
                {
                    if (!await context.DetallesPlanEstudio.AnyAsync(detalle =>
                        detalle.PlanEstudioId == grupo.OfertaAcademica!.PlanEstudioId
                        && detalle.CursoId == grupo.CursoId, cancellationToken))
                        return (null, "Uno de los cursos no pertenece al plan de estudio");

                    if (grupo.Horarios.Count == 0)
                        return (null, "Uno de los grupos no tiene horarios asignados");

                    var inscritos = await context.DetallesMatricula.CountAsync(detalle =>
                        detalle.DetalleOfertaAcademicaId == grupo.Id, cancellationToken);
                    if (inscritos >= grupo.Cupo)
                        return (null, "Uno de los grupos ya no tiene cupos disponibles");

                    foreach (var horario in grupo.Horarios)
                    {
                        if (seleccionados.Any(otro => otro.Id != grupo.Id && otro.Horarios.Any(hora =>
                            hora.DiaSemana == horario.DiaSemana
                            && hora.HoraInicio < horario.HoraFin && horario.HoraInicio < hora.HoraFin)))
                            return (null, "Los grupos seleccionados tienen cruces de horario");
                    }
                }

                matricula.Detalles = seleccionados.Select(grupo => new DetalleMatricula
                {
                    CursoId = grupo.CursoId,
                    DetalleOfertaAcademicaId = grupo.Id
                }).ToList();
                context.Matriculas.Add(matricula);
                await context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return (matricula.Id, null);
            }
            catch (DbUpdateException error) when (error.InnerException is SqlException sqlError
                && (sqlError.Number == 2601 || sqlError.Number == 2627 || sqlError.Number == 547 || sqlError.Number == 1205))
            {
                foreach (var detalle in matricula.Detalles.ToArray())
                    context.Entry(detalle).State = EntityState.Detached;
                context.Entry(matricula).State = EntityState.Detached;
                return (null, "No se pudo registrar, revisa la matricula y vuelve a intentarlo");
            }
            catch (SqlException error) when (error.Number == 1205)
            {
                return (null, "Otra solicitud esta usando los mismos cupos, vuelve a intentarlo");
            }
            catch (InvalidOperationException error) when (error.InnerException is DbUpdateException updateError
                && updateError.InnerException is SqlException sqlError && sqlError.Number == 1205)
            {
                foreach (var detalle in matricula.Detalles.ToArray())
                    context.Entry(detalle).State = EntityState.Detached;
                context.Entry(matricula).State = EntityState.Detached;
                return (null, "Otra solicitud esta usando los mismos cupos, vuelve a intentarlo");
            }
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
