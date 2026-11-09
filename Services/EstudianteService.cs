using App_Proyecto.DTOS;
using App_Proyecto.Models;
using App_Proyecto.Repositories;

namespace App_Proyecto.Services;

public class EstudianteService(IEstudianteRepository repository, ICursoRepository cursos) : IEstudianteService
{
    public Task<DataTableResponseDto<EstudianteTableDto>> GetPagedAsync(DataTablesRequestDto request) =>
        repository.GetPagedAsync(request);

    public Task<IReadOnlyList<EstudianteTableDto>> GetAllAsync() => repository.GetAllAsync();

    public async Task<SaveEstudianteDto?> GetByIdAsync(int id)
    {
        var estudiante = await repository.GetByIdAsync(id);
        return estudiante is null ? null : new SaveEstudianteDto
        {
            Id = estudiante.Id,
            Nombre = estudiante.Nombre ?? string.Empty,
            Apellido = estudiante.Apellido ?? string.Empty,
            Email = estudiante.Email ?? string.Empty,
            CursoId = estudiante.CursoId
        };
    }

    public async Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveEstudianteDto dto)
    {
        var error = await ValidateAsync(dto, null);
        if (error is not null)
            return (false, error);

        var estudiante = new Estudiante();
        Apply(estudiante, dto);
        await repository.CreateAsync(estudiante);
        return (true, null);
    }

    public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(SaveEstudianteDto dto)
    {
        var estudiante = dto.Id is int id ? await repository.GetByIdAsync(id) : null;
        if (estudiante is null)
            return (false, "Estudiante no encontrado");

        var error = await ValidateAsync(dto, estudiante.Id);
        if (error is not null)
            return (false, error);

        Apply(estudiante, dto);
        await repository.UpdateAsync(estudiante);
        return (true, null);
    }

    public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id) =>
        await repository.DeleteAsync(id) ? (true, null) : (false, "Estudiante no encontrado");

    private async Task<string?> ValidateAsync(SaveEstudianteDto dto, int? id)
    {
        if (await cursos.GetByIdAsync(dto.CursoId) is null)
            return "El curso seleccionado no existe";
        if (await repository.EmailExistsAsync(dto.Email.Trim(), id))
            return "El correo ya está registrado";
        return null;
    }

    private static void Apply(Estudiante estudiante, SaveEstudianteDto dto)
    {
        estudiante.Nombre = dto.Nombre.Trim();
        estudiante.Apellido = dto.Apellido.Trim();
        estudiante.Email = dto.Email.Trim();
        estudiante.CursoId = dto.CursoId;
    }
}
