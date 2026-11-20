using App_Proyecto.DTOS;
using App_Proyecto.Models;
using App_Proyecto.Repositories;

namespace App_Proyecto.Services
{
    public class AulaService(IAulaRepository repository, ISedeRepository parentRepository) : IAulaService
    {
        public async Task<IReadOnlyList<SaveAulaDto>> GetAllAsync()
        {
            var items = await repository.GetAllAsync();
            return items.Select(item => new SaveAulaDto
            {
                Id = item.Id,
                Nombre = item.Nombre,
                Capacidad = item.Capacidad,
                SedeId = item.SedeId
            }).ToList();
        }

        public async Task<SaveAulaDto?> GetByIdAsync(int id)
        {
            var item = await repository.GetByIdAsync(id);
            if (item == null)
                return null;

            return new SaveAulaDto
            {
                Id = item.Id,
                Nombre = item.Nombre,
                Capacidad = item.Capacidad,
                SedeId = item.SedeId
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveAulaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre) || dto.Nombre.Trim().Length < 2 || dto.Nombre.Trim().Length > 100)
                return (false, "El nombre debe tener entre 2 y 100 caracteres");

            if (await parentRepository.GetByIdAsync(dto.SedeId) == null)
                return (false, "El registro relacionado no existe");

            if (dto.Capacidad <= 0)
                return (false, "La capacidad debe ser mayor que cero");

            var created = await repository.CreateAsync(new Aula
            {
                Nombre = dto.Nombre.Trim(),
                Capacidad = dto.Capacidad,
                SedeId = dto.SedeId
            });
            return created ? (true, null) : (false, "No se pudo guardar el registro");
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(SaveAulaDto dto)
        {
            if (!dto.Id.HasValue)
                return (false, "Selecciona un registro valido");

            if (string.IsNullOrWhiteSpace(dto.Nombre) || dto.Nombre.Trim().Length < 2 || dto.Nombre.Trim().Length > 100)
                return (false, "El nombre debe tener entre 2 y 100 caracteres");

            if (await parentRepository.GetByIdAsync(dto.SedeId) == null)
                return (false, "El registro relacionado no existe");

            if (dto.Capacidad <= 0)
                return (false, "La capacidad debe ser mayor que cero");

            var item = await repository.GetByIdAsync(dto.Id.Value);
            if (item == null)
                return (false, "Registro no encontrado");

            item.Nombre = dto.Nombre.Trim();
            item.Capacidad = dto.Capacidad;
            item.SedeId = dto.SedeId;
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
