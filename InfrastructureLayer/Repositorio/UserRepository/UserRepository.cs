using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace InfrastructureLayer.Repositorio.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly TaskManagerContext _context;

        public UserRepository(TaskManagerContext context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Set<User>().FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetByIdAsync(int id) // This method matches the interface
        {
            return await _context.Set<User>().FindAsync(id);
        }

        public async Task AddAsync(User user)
        {
            await _context.Set<User>().AddAsync(user);
        }

        public void Add(User user)
        {
            _context.Set<User>().Add(user);
        }

        public void Update(User user)
        {
            _context.Set<User>().Update(user);
        }

        public void Delete(User user)
        {
            _context.Set<User>().Remove(user);
        }
    }
}
