using App_Proyecto.DTOS;
using App_Proyecto.Models;
using App_Proyecto.Repositories;

namespace App_Proyecto.Services
{
    public class FacultadService(IFacultadRepository repository) : IFacultadService
    {
        public async Task<IReadOnlyList<SaveFacultadDto>> GetAllAsync()
        {
            var items = await repository.GetAllAsync();
            return items.Select(item => new SaveFacultadDto
            {
                Id = item.Id,
                Nombre = item.Nombre,
                Descripcion = item.Descripcion
            }).ToList();
        }

        public async Task<SaveFacultadDto?> GetByIdAsync(int id)
        {
            var item = await repository.GetByIdAsync(id);
            if (item == null)
                return null;

            return new SaveFacultadDto
            {
                Id = item.Id,
                Nombre = item.Nombre,
                Descripcion = item.Descripcion
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveFacultadDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre) || dto.Nombre.Trim().Length < 2 || dto.Nombre.Trim().Length > 100)
                return (false, "El nombre debe tener entre 2 y 100 caracteres");

            var created = await repository.CreateAsync(new Facultad
            {
                Nombre = dto.Nombre.Trim(),
                Descripcion = dto.Descripcion?.Trim() ?? string.Empty
            });
            return created ? (true, null) : (false, "No se pudo guardar el registro");
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(SaveFacultadDto dto)
        {
            if (!dto.Id.HasValue)
                return (false, "Selecciona un registro valido");

            if (string.IsNullOrWhiteSpace(dto.Nombre) || dto.Nombre.Trim().Length < 2 || dto.Nombre.Trim().Length > 100)
                return (false, "El nombre debe tener entre 2 y 100 caracteres");

            var item = await repository.GetByIdAsync(dto.Id.Value);
            if (item == null)
                return (false, "Registro no encontrado");

            item.Nombre = dto.Nombre.Trim();
            item.Descripcion = dto.Descripcion?.Trim() ?? string.Empty;
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
