using App_Proyecto.Models;

namespace App_Proyecto.Repositories
{
    public interface ISedeRepository
    {
        Task<IReadOnlyList<Sede>> GetAllAsync();
        Task<Sede?> GetByIdAsync(int id);
        Task<bool> CreateAsync(Sede item);
        Task<bool> UpdateAsync(Sede item);
        Task<bool> DeleteAsync(int id);
    }
}
