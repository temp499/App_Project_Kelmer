using App_Proyecto.DTOS;
using App_Proyecto.Models;

namespace App_Proyecto.Repositories
{
    public interface ICursoRepository
    {

        Task<DataTableResponseDto<CursoTableDto>> GetPagedAsync(DataTablesRequestDto request);
        Task<IReadOnlyList<Curso>> GetAllAsync();
        Task<Curso?> GetByIdAsync(int id);
        Task<bool> NombreExisteAsync(string nombre);
        Task<Curso> CreateAsync(Curso curso);
        Task<Curso> UpdateAsync(Curso curso);
        Task<Curso>DeleteAsync(int id);
        Task<bool> HasAnyEstudentAsync(int id);
        
        



    }
}
