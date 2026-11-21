using App_Proyecto.DTOS;
using App_Proyecto.Models;
using App_Proyecto.Repositories;
using System.ComponentModel.DataAnnotations;

namespace App_Proyecto.Services
{
    public class MatriculaService(IMatriculaRepository repository) : IMatriculaService
    {
        public async Task<MatriculaConsultaDto?> GetByIdAsync(
            int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                return null;

            var matricula = await repository.GetByIdAsync(id, cancellationToken);
            if (matricula == null)
                return null;

            return new MatriculaConsultaDto
            {
                Id = matricula.Id,
                EstudianteId = matricula.EstudianteId,
                EstudianteNombre = $"{matricula.Estudiante.Nombre} {matricula.Estudiante.Apellido}".Trim(),
                PeriodoAcademicoId = matricula.PeriodoAcademicoId,
                PeriodoNombre = matricula.PeriodoAcademico.Nombre,
                Cursos = matricula.Detalles
                    .OrderBy(detalle => detalle.Curso.Nombre)
                    .ThenBy(detalle => detalle.CursoId)
                    .Select(detalle => new CursoMatriculaDto
                    {
                        Id = detalle.CursoId,
                        Nombre = detalle.Curso.Nombre,
                        Descripcion = detalle.Curso.Descripcion,
                        GrupoId = detalle.DetalleOfertaAcademicaId,
                        Grupo = detalle.DetalleOfertaAcademica?.Grupo ?? string.Empty,
                        Horarios = detalle.DetalleOfertaAcademica?.Horarios
                            .OrderBy(horario => horario.DiaSemana).ThenBy(horario => horario.HoraInicio)
                            .Select(horario => new HorarioGrupoDto
                            {
                                DiaSemana = horario.DiaSemana,
                                HoraInicio = horario.HoraInicio,
                                HoraFin = horario.HoraFin,
                                Aula = horario.Aula?.Nombre ?? string.Empty
                            }).ToList() ?? []
                    }).ToList()
            };
        }

        public Task <HistorialMatriculaDto?> GetHistorialAsync(
            ConsultaHistorialDto consulta, CancellationToken cancellationToken = default)
        {
            Validator.ValidateObject(consulta, new ValidationContext(consulta), true);
            return repository.GetHistorialAsync(consulta,cancellationToken);
        }

        public async Task<(int? MatriculaId, string? ErrorMessage)> CreateAsync(
            int estudianteId,
            CrearMatriculaDto dto,
            CancellationToken cancellationToken = default)
        {
            if (estudianteId <= 0)
                return (null, "El estudiante no es valido");

            if (dto.PeriodoAcademicoId <= 0)
                return (null, "Selecciona un periodo academico");

            var cursoIds = dto.CursoIds;
            var grupoIds = dto.GrupoIds;
            if (cursoIds == null || grupoIds == null || (cursoIds.Length == 0 && grupoIds.Length == 0))
                return (null, "Selecciona al menos un curso o grupo");

            if (cursoIds.Any(id => id <= 0) || grupoIds.Any(id => id <= 0))
                return (null, "La seleccion contiene un registro no valido");

            if (cursoIds.Distinct().Count() != cursoIds.Length || grupoIds.Distinct().Count() != grupoIds.Length)
                return (null, "No puedes seleccionar el mismo curso o grupo mas de una vez");

            var matricula = new Matricula
            {
                EstudianteId = estudianteId,
                PeriodoAcademicoId = dto.PeriodoAcademicoId,
                Detalles = cursoIds
                    .Select(id => new DetalleMatricula { CursoId = id })
                    .ToList()
            };

            return await repository.CreateAsync(matricula, grupoIds, cancellationToken);
        }


    }
}
