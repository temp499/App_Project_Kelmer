using App_Proyecto.DTOS;
using App_Proyecto.Models;
using App_Proyecto.Repositories;

namespace App_Proyecto.Services
{
    public class DetalleOfertaAcademicaService(IDetalleOfertaAcademicaRepository repository, IOfertaAcademicaRepository ofertas, ICursoRepository cursos, IDocenteRepository docentes, IDetallePlanEstudioRepository planes) : IDetalleOfertaAcademicaService
    {
        public async Task<IReadOnlyList<SaveDetalleOfertaAcademicaDto>> GetAllAsync(int? ofertaAcademicaId)
        {
            var items = await repository.GetAllAsync(ofertaAcademicaId);
            return items.Select(item => new SaveDetalleOfertaAcademicaDto
            {
                Id = item.Id,
                OfertaAcademicaId = item.OfertaAcademicaId,
                CursoId = item.CursoId,
                DocenteId = item.DocenteId,
                Cupo = item.Cupo,
                Grupo = item.Grupo
            }).ToList();
        }

        public async Task<SaveDetalleOfertaAcademicaDto?> GetByIdAsync(int id)
        {
            var item = await repository.GetByIdAsync(id);
            if (item == null)
                return null;

            return new SaveDetalleOfertaAcademicaDto
            {
                Id = item.Id,
                OfertaAcademicaId = item.OfertaAcademicaId,
                CursoId = item.CursoId,
                DocenteId = item.DocenteId,
                Cupo = item.Cupo,
                Grupo = item.Grupo
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveDetalleOfertaAcademicaDto dto)
        {
            var error = await ValidateAsync(dto);
            if (error != null)
                return (false, error);

            var created = await repository.CreateAsync(new DetalleOfertaAcademica
            {
                OfertaAcademicaId = dto.OfertaAcademicaId,
                CursoId = dto.CursoId,
                DocenteId = dto.DocenteId,
                Cupo = dto.Cupo,
                Grupo = dto.Grupo.Trim()
            });
            return created ? (true, null) : (false, "Revisa si ese grupo ya existe en la oferta o si los datos relacionados cambiaron");
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(SaveDetalleOfertaAcademicaDto dto)
        {
            if (!dto.Id.HasValue)
                return (false, "Selecciona un registro valido");

            var error = await ValidateAsync(dto);
            if (error != null)
                return (false, error);

            var item = await repository.GetByIdAsync(dto.Id.Value);
            if (item == null)
                return (false, "Registro no encontrado");

            item.OfertaAcademicaId = dto.OfertaAcademicaId;
            item.CursoId = dto.CursoId;
            item.DocenteId = dto.DocenteId;
            item.Cupo = dto.Cupo;
            item.Grupo = dto.Grupo.Trim();
            var updated = await repository.UpdateAsync(item);
            return updated ? (true, null) : (false, "Revisa si ese grupo ya existe en la oferta o si los datos relacionados cambiaron");
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            if (await repository.GetByIdAsync(id) == null)
                return (false, "Registro no encontrado");

            var deleted = await repository.DeleteAsync(id);
            return deleted
                ? (true, null)
                : (false, "No se puede eliminar porque tiene registros relacionados");
        }
        private async Task<string?> ValidateAsync(SaveDetalleOfertaAcademicaDto dto)
        {
            if (dto.Cupo <= 0)
                return "El cupo debe ser mayor que cero";

            if (string.IsNullOrWhiteSpace(dto.Grupo) || dto.Grupo.Trim().Length > 20)
                return "Escribe un nombre de grupo de hasta 20 caracteres";

            var oferta = await ofertas.GetByIdAsync(dto.OfertaAcademicaId);
            if (oferta == null)
                return "La oferta academica no existe";

            if (await cursos.GetByIdAsync(dto.CursoId) == null)
                return "El curso no existe";

            if (await docentes.GetByIdAsync(dto.DocenteId) == null)
                return "El docente no existe";

            var detalles = await planes.GetAllAsync(oferta.PlanEstudioId);
            if (!detalles.Any(detalle => detalle.CursoId == dto.CursoId))
                return "El curso no pertenece al plan de esta oferta";

            return null;
        }
    }
}
