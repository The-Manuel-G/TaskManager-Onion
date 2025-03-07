using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            public void Add(RefreshToken refreshToken)
            {
                _context.RefreshTokens.Add(refreshToken);
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

