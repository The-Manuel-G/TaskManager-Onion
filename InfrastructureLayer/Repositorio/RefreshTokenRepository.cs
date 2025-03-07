using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DomainLayer.Models;

namespace InfrastructureLayer.Repositorio
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly TaskManagerContext _context;

        public RefreshTokenRepository(TaskManagerContext context)
        {
            _context = context;
        }

        public RefreshToken? GetByToken(string token)
        {
            return _context.RefreshTokens.FirstOrDefault(r => r.Token == token);
        }

        public async Task<RefreshToken?> GetByTokenAsync(string refreshToken)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken);
        }

        public void Add(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
        }

        public async Task AddAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
        }

        public void Delete(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Remove(refreshToken);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
