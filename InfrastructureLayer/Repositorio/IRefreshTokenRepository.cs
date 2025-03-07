using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Models;

namespace InfrastructureLayer.Repositorio
{
        public interface IRefreshTokenRepository
        {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task AddAsync(RefreshToken token);
        Task DeleteAsync(RefreshToken token);
        Task SaveChangesAsync();

    }


}
