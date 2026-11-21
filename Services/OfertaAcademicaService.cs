using App_Proyecto.DTOS;
using App_Proyecto.Models;
using App_Proyecto.Repositories;

namespace App_Proyecto.Services
{
    public class OfertaAcademicaService(IOfertaAcademicaRepository repository, IPlanEstudioRepository parentRepository, IPeriodoAcademicoRepository periodos, ISedeRepository sedes) : IOfertaAcademicaService
    {
        public async Task<IReadOnlyList<SaveOfertaAcademicaDto>> GetAllAsync()
        {
            var items = await repository.GetAllAsync();
            return items.Select(item => new SaveOfertaAcademicaDto
            {
                Id = item.Id,
                Nombre = item.Nombre,
                PeriodoAcademicoId = item.PeriodoAcademicoId,
                SedeId = item.SedeId,
                Activa = item.Activa,
                PlanEstudioId = item.PlanEstudioId
            }).ToList();
        }

        public async Task<SaveOfertaAcademicaDto?> GetByIdAsync(int id)
        {
            var item = await repository.GetByIdAsync(id);
            if (item == null)
                return null;

            return new SaveOfertaAcademicaDto
            {
                Id = item.Id,
                Nombre = item.Nombre,
                PeriodoAcademicoId = item.PeriodoAcademicoId,
                SedeId = item.SedeId,
                Activa = item.Activa,
                PlanEstudioId = item.PlanEstudioId
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveOfertaAcademicaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre) || dto.Nombre.Trim().Length < 2 || dto.Nombre.Trim().Length > 100)
                return (false, "El nombre debe tener entre 2 y 100 caracteres");

            var plan = await parentRepository.GetByIdAsync(dto.PlanEstudioId);
            if (plan == null || (dto.Activa && !plan.Activo))
                return (false, "Selecciona un plan de estudio activo");

            if (await periodos.GetByIdAsync(dto.PeriodoAcademicoId) == null)
                return (false, "El periodo academico no existe");

            if (await sedes.GetByIdAsync(dto.SedeId) == null)
                return (false, "La sede no existe");

            var created = await repository.CreateAsync(new OfertaAcademica
            {
                Nombre = dto.Nombre.Trim(),
                PeriodoAcademicoId = dto.PeriodoAcademicoId,
                SedeId = dto.SedeId,
                Activa = dto.Activa,
                PlanEstudioId = dto.PlanEstudioId
            });
            return created ? (true, null) : (false, "Revisa si ya existe una oferta para ese plan, periodo y sede");
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(SaveOfertaAcademicaDto dto)
        {
            if (!dto.Id.HasValue)
                return (false, "Selecciona un registro valido");

            if (string.IsNullOrWhiteSpace(dto.Nombre) || dto.Nombre.Trim().Length < 2 || dto.Nombre.Trim().Length > 100)
                return (false, "El nombre debe tener entre 2 y 100 caracteres");

            var plan = await parentRepository.GetByIdAsync(dto.PlanEstudioId);
            if (plan == null || (dto.Activa && !plan.Activo))
                return (false, "Selecciona un plan de estudio activo");

            if (await periodos.GetByIdAsync(dto.PeriodoAcademicoId) == null)
                return (false, "El periodo academico no existe");

            if (await sedes.GetByIdAsync(dto.SedeId) == null)
                return (false, "La sede no existe");

            var item = await repository.GetByIdAsync(dto.Id.Value);
            if (item == null)
                return (false, "Registro no encontrado");

            item.Nombre = dto.Nombre.Trim();
            item.PeriodoAcademicoId = dto.PeriodoAcademicoId;
            item.SedeId = dto.SedeId;
            item.Activa = dto.Activa;
            item.PlanEstudioId = dto.PlanEstudioId;
            var updated = await repository.UpdateAsync(item);
            return updated ? (true, null) : (false, "Revisa si ya existe una oferta para ese plan, periodo y sede");
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
    }
}
