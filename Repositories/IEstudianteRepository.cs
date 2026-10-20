using App_Proyecto.DTOS;
using App_Proyecto.Models;

namespace App_Proyecto.Repositories
{
    public interface IEstudianteRepository
    {
        Task<DataTableResponseDto<EstudianteTableDto>> GetPagedAsync(DataTablesRequestDto request);

        Task<IReadOnlyList<Estudiante>> GetAllAsync();

        Task<Estudiante?> GetByIdAsync(int id);
        Task<bool> EmailExistAsync(string email, int? excludeId= null);
        Task<bool> CreateAsync(Estudiante estudiante);
        Task<bool> UpdateAsync(Estudiante estudiante);
        Task<bool> DeleteAsync(int id);



    }






}

