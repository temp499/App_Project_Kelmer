using App_Proyecto.Models;

namespace App_Proyecto.Repositories
{
    public interface IAulaRepository
    {
        Task<IReadOnlyList<Aula>> GetAllAsync();
        Task<Aula?> GetByIdAsync(int id);
        Task<bool> CreateAsync(Aula item);
        Task<bool> UpdateAsync(Aula item);
        Task<bool> DeleteAsync(int id);
    }
}
