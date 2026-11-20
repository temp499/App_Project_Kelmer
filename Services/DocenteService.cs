using App_Proyecto.DTOS;
using App_Proyecto.Models;
using App_Proyecto.Repositories;

namespace App_Proyecto.Services
{
    public class DocenteService(IDocenteRepository repository) : IDocenteService
    {
        public async Task<IReadOnlyList<SaveDocenteDto>> GetAllAsync()
        {
            var items = await repository.GetAllAsync();
            return items.Select(item => new SaveDocenteDto
            {
                Id = item.Id,
                Nombre = item.Nombre,
                Apellido = item.Apellido,
                Email = item.Email
            }).ToList();
        }

        public async Task<SaveDocenteDto?> GetByIdAsync(int id)
        {
            var item = await repository.GetByIdAsync(id);
            if (item == null)
                return null;

            return new SaveDocenteDto
            {
                Id = item.Id,
                Nombre = item.Nombre,
                Apellido = item.Apellido,
                Email = item.Email
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveDocenteDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre) || dto.Nombre.Trim().Length < 2 || dto.Nombre.Trim().Length > 100)
                return (false, "El nombre debe tener entre 2 y 100 caracteres");

            if (string.IsNullOrWhiteSpace(dto.Apellido) || string.IsNullOrWhiteSpace(dto.Email))
                return (false, "Completa el apellido y el correo del docente");

            var created = await repository.CreateAsync(new Docente
            {
                Nombre = dto.Nombre.Trim(),
                Apellido = dto.Apellido?.Trim() ?? string.Empty,
                Email = dto.Email?.Trim() ?? string.Empty
            });
            return created ? (true, null) : (false, "No se pudo guardar el registro");
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(SaveDocenteDto dto)
        {
            if (!dto.Id.HasValue)
                return (false, "Selecciona un registro valido");

            if (string.IsNullOrWhiteSpace(dto.Nombre) || dto.Nombre.Trim().Length < 2 || dto.Nombre.Trim().Length > 100)
                return (false, "El nombre debe tener entre 2 y 100 caracteres");

            if (string.IsNullOrWhiteSpace(dto.Apellido) || string.IsNullOrWhiteSpace(dto.Email))
                return (false, "Completa el apellido y el correo del docente");

            var item = await repository.GetByIdAsync(dto.Id.Value);
            if (item == null)
                return (false, "Registro no encontrado");

            item.Nombre = dto.Nombre.Trim();
            item.Apellido = dto.Apellido?.Trim() ?? string.Empty;
            item.Email = dto.Email?.Trim() ?? string.Empty;
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
