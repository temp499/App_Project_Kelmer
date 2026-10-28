using App_Proyecto.DTOS;
using App_Proyecto.Models;

namespace App_Proyecto.Repositories
{
    public interface ICursoRepository
    {
        Task<DataTableResponseDto<CursoTableDto>> GetPagedAsync(DataTablesRequestDto request);
        Task<IReadOnlyList<Curso>> GetAllAsync(); 
        Task<Curso?> GetByIdAsync(int Id);
        Task<bool> NombreExisteAsync(string nombre, int? excludeId =null);
        Task<Curso> CreateAsync(Curso curso);
        Task<bool> UpdateAsync(Curso curso);

        Task<bool> DeleteAsync(int Id);
        Task<bool> HasAnyEstudentAsync(int Id);
    }
}
