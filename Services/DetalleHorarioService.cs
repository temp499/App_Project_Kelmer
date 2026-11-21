using App_Proyecto.DTOS;
using App_Proyecto.Models;
using App_Proyecto.Repositories;

namespace App_Proyecto.Services
{
    public class DetalleHorarioService(IDetalleHorarioRepository repository, IDetalleOfertaAcademicaRepository grupos, IAulaRepository aulas) : IDetalleHorarioService
    {
        public async Task<IReadOnlyList<SaveDetalleHorarioDto>> GetAllAsync(int? detalleOfertaAcademicaId)
        {
            var items = await repository.GetAllAsync(detalleOfertaAcademicaId);
            return items.Select(item => new SaveDetalleHorarioDto
            {
                Id = item.Id,
                DetalleOfertaAcademicaId = item.DetalleOfertaAcademicaId,
                AulaId = item.AulaId,
                DiaSemana = item.DiaSemana,
                HoraInicio = item.HoraInicio,
                HoraFin = item.HoraFin
            }).ToList();
        }

        public async Task<SaveDetalleHorarioDto?> GetByIdAsync(int id)
        {
            var item = await repository.GetByIdAsync(id);
            if (item == null)
                return null;

            return new SaveDetalleHorarioDto
            {
                Id = item.Id,
                DetalleOfertaAcademicaId = item.DetalleOfertaAcademicaId,
                AulaId = item.AulaId,
                DiaSemana = item.DiaSemana,
                HoraInicio = item.HoraInicio,
                HoraFin = item.HoraFin
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveDetalleHorarioDto dto)
        {
            var error = await ValidateAsync(dto);
            if (error != null)
                return (false, error);

            var created = await repository.CreateAsync(new DetalleHorario
            {
                DetalleOfertaAcademicaId = dto.DetalleOfertaAcademicaId,
                AulaId = dto.AulaId,
                DiaSemana = dto.DiaSemana,
                HoraInicio = dto.HoraInicio,
                HoraFin = dto.HoraFin
            });
            return created ? (true, null) : (false, "Revisa la sede, la capacidad del aula y los cruces de horario");
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(SaveDetalleHorarioDto dto)
        {
            if (!dto.Id.HasValue)
                return (false, "Selecciona un registro valido");

            var error = await ValidateAsync(dto);
            if (error != null)
                return (false, error);

            var item = await repository.GetByIdAsync(dto.Id.Value);
            if (item == null)
                return (false, "Registro no encontrado");

            item.DetalleOfertaAcademicaId = dto.DetalleOfertaAcademicaId;
            item.AulaId = dto.AulaId;
            item.DiaSemana = dto.DiaSemana;
            item.HoraInicio = dto.HoraInicio;
            item.HoraFin = dto.HoraFin;
            var updated = await repository.UpdateAsync(item);
            return updated ? (true, null) : (false, "Revisa la sede, la capacidad del aula y los cruces de horario");
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
        private async Task<string?> ValidateAsync(SaveDetalleHorarioDto dto)
        {
            if (dto.DiaSemana < 1 || dto.DiaSemana > 7)
                return "Selecciona un dia entre 1 y 7";

            if (dto.HoraInicio < TimeSpan.Zero || dto.HoraFin >= TimeSpan.FromDays(1)
                || dto.HoraFin <= dto.HoraInicio)
                return "La hora final debe ser posterior al inicio y estar dentro del mismo dia";

            if (await grupos.GetByIdAsync(dto.DetalleOfertaAcademicaId) == null)
                return "El grupo no existe";

            if (await aulas.GetByIdAsync(dto.AulaId) == null)
                return "El aula no existe";

            return null;
        }
    }
}
