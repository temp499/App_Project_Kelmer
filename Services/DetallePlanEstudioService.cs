using App_Proyecto.DTOS;
using App_Proyecto.Models;
using App_Proyecto.Repositories;

namespace App_Proyecto.Services
{
    public class DetallePlanEstudioService(IDetallePlanEstudioRepository repository, IPlanEstudioRepository planes, ICursoRepository cursos) : IDetallePlanEstudioService
    {
        public async Task<IReadOnlyList<SaveDetallePlanEstudioDto>> GetAllAsync(int? planEstudioId)
        {
            var items = await repository.GetAllAsync(planEstudioId);
            return items.Select(item => new SaveDetallePlanEstudioDto
            {
                Id = item.Id,
                PlanEstudioId = item.PlanEstudioId,
                CursoId = item.CursoId,
                Creditos = item.Creditos,
                Semestre = item.Semestre
            }).ToList();
        }

        public async Task<SaveDetallePlanEstudioDto?> GetByIdAsync(int id)
        {
            var item = await repository.GetByIdAsync(id);
            if (item == null)
                return null;

            return new SaveDetallePlanEstudioDto
            {
                Id = item.Id,
                PlanEstudioId = item.PlanEstudioId,
                CursoId = item.CursoId,
                Creditos = item.Creditos,
                Semestre = item.Semestre
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveDetallePlanEstudioDto dto)
        {
            var error = await ValidateAsync(dto);
            if (error != null)
                return (false, error);

            var created = await repository.CreateAsync(new DetallePlanEstudio
            {
                PlanEstudioId = dto.PlanEstudioId,
                CursoId = dto.CursoId,
                Creditos = dto.Creditos,
                Semestre = dto.Semestre
            });
            return created ? (true, null) : (false, "Revisa si el curso ya pertenece al plan o si los datos relacionados cambiaron");
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(SaveDetallePlanEstudioDto dto)
        {
            if (!dto.Id.HasValue)
                return (false, "Selecciona un registro valido");

            var error = await ValidateAsync(dto);
            if (error != null)
                return (false, error);

            var item = await repository.GetByIdAsync(dto.Id.Value);
            if (item == null)
                return (false, "Registro no encontrado");

            item.PlanEstudioId = dto.PlanEstudioId;
            item.CursoId = dto.CursoId;
            item.Creditos = dto.Creditos;
            item.Semestre = dto.Semestre;
            var updated = await repository.UpdateAsync(item);
            return updated ? (true, null) : (false, "Revisa si el curso ya pertenece al plan o si los datos relacionados cambiaron");
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
        private async Task<string?> ValidateAsync(SaveDetallePlanEstudioDto dto)
        {
            if (dto.Creditos <= 0 || dto.Semestre <= 0)
                return "Los creditos y el semestre deben ser mayores que cero";

            if (await planes.GetByIdAsync(dto.PlanEstudioId) == null)
                return "El plan de estudio no existe";

            if (await cursos.GetByIdAsync(dto.CursoId) == null)
                return "El curso no existe";

            return null;
        }

    }
}
