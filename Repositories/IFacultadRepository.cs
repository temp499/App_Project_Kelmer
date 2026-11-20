using App_Proyecto.Models;

namespace App_Proyecto.Repositories
{
    public interface IFacultadRepository
    {
        Task<IReadOnlyList<Facultad>> GetAllAsync();
        Task<Facultad?> GetByIdAsync(int id);
        Task<bool> CreateAsync(Facultad item);
        Task<bool> UpdateAsync(Facultad item);
        Task<bool> DeleteAsync(int id);
    }
}
