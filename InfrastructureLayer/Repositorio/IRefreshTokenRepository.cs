using System.Threading.Tasks;
using DomainLayer.Models;

namespace InfrastructureLayer.Repositorio
{
    public interface IRefreshTokenRepository
    {
        RefreshToken? GetByToken(string token);
        Task<RefreshToken?> GetByTokenAsync(string refreshToken); // Fixed return type
        void Add(RefreshToken refreshToken);
        Task AddAsync(RefreshToken refreshToken);
        void Delete(RefreshToken refreshToken);
        Task SaveChangesAsync();
    }
}
