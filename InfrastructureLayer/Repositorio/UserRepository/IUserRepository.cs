using DomainLayer.Models;

namespace InfrastructureLayer.Repositorio.UserRepository
{
    public interface IUserRepository
    {

        void Add(User user);
        void Update(User user);
        void Delete(User user);


        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(int id);
        Task AddAsync(User user);
        Task SaveChangesAsync();

      
        User GetByUsername(object value);
    }
}
