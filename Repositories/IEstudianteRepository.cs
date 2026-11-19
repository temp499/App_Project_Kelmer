using App_Proyecto.DTOS;
using App_Proyecto.Models;

namespace App_Proyecto.Repositories
{
    public interface IEstudianteRepository
    {

        Task<DataTableResponseDto<EstudianteTableDto>> GetPagedAsync(DataTablesRequestDto request);
        Task<IReadOnlyList<EstudianteTableDto>> GetAllAsync();
        Task<Estudiante?> GetByIdAsync(int Id);
        Task<bool> EmailExistsAsync(string email,int? excludeId=null);
        Task<Estudiante> CreateAsync(Estudiante estudiante);
        Task<bool> UpdateAsync(Estudiante estudiante);

        Task<bool> TieneMatriculasAsync(int id);
        Task<bool> DeleteAsync(int Id);

    }
}
