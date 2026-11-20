using App_Proyecto.Models;

namespace App_Proyecto.Repositories
{
    public interface IDocenteRepository
    {
        Task<IReadOnlyList<Docente>> GetAllAsync();
        Task<Docente?> GetByIdAsync(int id);
        Task<bool> CreateAsync(Docente item);
        Task<bool> UpdateAsync(Docente item);
        Task<bool> DeleteAsync(int id);
    }
}
