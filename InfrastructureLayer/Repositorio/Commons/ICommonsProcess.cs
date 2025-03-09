using System.Collections.Generic;
using System.Threading.Tasks;

namespace InfrastructureLayer.Repositorio.Commons
{
    public interface ICommonsProcess<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetIdAsync(int id);
        Task<bool> ExistsAsync(int id); // ✅ Nuevo método
        Task<(bool IsSuccess, string Message)> AddAsync(T entry);
        Task<(bool IsSuccess, string Message)> UpdateAsync(T entry);
        Task<(bool IsSuccess, string Message)> DeleteAsync(int id);
        Task SaveChangesAsync(); // ✅ Método agregado
    }
}
