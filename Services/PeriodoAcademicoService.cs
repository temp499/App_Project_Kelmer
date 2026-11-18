using App_Proyecto.DTOS;
using App_Proyecto.Models;
using App_Proyecto.Repositories;

namespace App_Proyecto.Services
{
    public class PeriodoAcademicoService(IPeriodoAcademicoRepository repository) : IPeriodoAcademicoService
    {
        public async Task<IReadOnlyList<SavePeriodoAcademicoDto>> GetAllAsync()
        {
            var periodos = await repository.GetAllAsync();
            return periodos.Select(periodo => new SavePeriodoAcademicoDto
            {
                Id = periodo.Id,
                Nombre = periodo.Nombre,
                MatriculaAbierta = periodo.MatriculaAbierta
            }).ToList();
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(SavePeriodoAcademicoDto dto)
        {
            var nombre = dto.Nombre?.Trim();
            if (string.IsNullOrEmpty(nombre) || nombre.Length < 2 || nombre.Length > 100)
                return (false, "El nombre debe tener entre 2 y 100 caracteres");

            await repository.CreateAsync(new PeriodoAcademico
            {
                Nombre = nombre,
                MatriculaAbierta = dto.MatriculaAbierta
            });

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(SavePeriodoAcademicoDto dto)
        {
            if (!dto.Id.HasValue || dto.Id.Value <= 0)
                return (false, "El periodo no es valido");

            var nombre = dto.Nombre?.Trim();
            if (string.IsNullOrEmpty(nombre) || nombre.Length < 2 || nombre.Length > 100)
                return (false, "El nombre debe tener entre 2 y 100 caracteres");

            var periodo = await repository.GetByIdAsync(dto.Id.Value);
            if (periodo == null)
                return (false, "Periodo no encontrado");

            periodo.Nombre = nombre;
            periodo.MatriculaAbierta = dto.MatriculaAbierta;
            await repository.UpdateAsync(periodo);

            return (true, null);
        }
    }
}
