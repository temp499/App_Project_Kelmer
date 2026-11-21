using App_Proyecto.Models;

namespace App_Proyecto.Repositories
{
    public interface ICarreraRepository
    {
        Task<IReadOnlyList<Carrera>> GetAllAsync();
        Task<Carrera?> GetByIdAsync(int id);
        Task<bool> CreateAsync(Carrera item);
        Task<bool> UpdateAsync(Carrera item);
        Task<bool> DeleteAsync(int id);
    }
}
