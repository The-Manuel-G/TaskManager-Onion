using System.Threading.Tasks;
using DomainLayer.Models;

namespace InfrastructureLayer.Repositorio.UserRepository
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(int id);  // Ensure this method is correctly named
        Task AddAsync(User user);
        Task SaveChangesAsync();

        void Add(User user);
        void Update(User user);
        void Delete(User user);

    }
}



