using App_Proyecto.DTOS;
using App_Proyecto.Models;
using App_Proyecto.Repositories;

namespace App_Proyecto.Services
{
    public class PlanEstudioService(IPlanEstudioRepository repository, ICarreraRepository parentRepository) : IPlanEstudioService
    {
        public async Task<IReadOnlyList<SavePlanEstudioDto>> GetAllAsync()
        {
            var items = await repository.GetAllAsync();
            return items.Select(item => new SavePlanEstudioDto
            {
                Id = item.Id,
                Nombre = item.Nombre,
                Activo = item.Activo,
                CarreraId = item.CarreraId
            }).ToList();
        }

        public async Task<SavePlanEstudioDto?> GetByIdAsync(int id)
        {
            var item = await repository.GetByIdAsync(id);
            if (item == null)
                return null;

            return new SavePlanEstudioDto
            {
                Id = item.Id,
                Nombre = item.Nombre,
                Activo = item.Activo,
                CarreraId = item.CarreraId
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(SavePlanEstudioDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre) || dto.Nombre.Trim().Length < 2 || dto.Nombre.Trim().Length > 100)
                return (false, "El nombre debe tener entre 2 y 100 caracteres");

            if (await parentRepository.GetByIdAsync(dto.CarreraId) == null)
                return (false, "El registro relacionado no existe");

            var created = await repository.CreateAsync(new PlanEstudio
            {
                Nombre = dto.Nombre.Trim(),
                Activo = dto.Activo,
                CarreraId = dto.CarreraId
            });
            return created ? (true, null) : (false, "No se pudo guardar el registro");
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(SavePlanEstudioDto dto)
        {
            if (!dto.Id.HasValue)
                return (false, "Selecciona un registro valido");

            if (string.IsNullOrWhiteSpace(dto.Nombre) || dto.Nombre.Trim().Length < 2 || dto.Nombre.Trim().Length > 100)
                return (false, "El nombre debe tener entre 2 y 100 caracteres");

            if (await parentRepository.GetByIdAsync(dto.CarreraId) == null)
                return (false, "El registro relacionado no existe");

            var item = await repository.GetByIdAsync(dto.Id.Value);
            if (item == null)
                return (false, "Registro no encontrado");

            item.Nombre = dto.Nombre.Trim();
            item.Activo = dto.Activo;
            item.CarreraId = dto.CarreraId;
            var updated = await repository.UpdateAsync(item);
            return updated ? (true, null) : (false, "No se pudo actualizar el registro");
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
