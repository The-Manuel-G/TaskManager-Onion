using DomainLayer.DTO;
using DomainLayer.Models;
using InfrastructureLayer.Repositorio;
using InfrastructureLayer.Repositorio.UserRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApplicationLayer.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _configuration = configuration;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest request)
        {
            // 1. Verify user
            var user = await _userRepository.GetByUsernameAsync(request.Username!);
            if (user == null)
            {
                return new AuthenticateResponse
                {
                    StatusCode = 401
                };
            }

            // 2. Validate password (ensure PasswordHash is not null)
            if (string.IsNullOrEmpty(user.PasswordHash) ||
                !BCrypt.Net.BCrypt.Verify(request.Password!, user.PasswordHash))
            {
                return new AuthenticateResponse
                {
                    StatusCode = 401
                };
            }

            // 3. Generate JWT token
            var jwtToken = GenerateJwtToken(user);

            // 4. Generate refresh token and save it
            var refreshToken = GenerateRefreshToken(user.Id);
            await _refreshTokenRepository.AddAsync(refreshToken);
            await _refreshTokenRepository.SaveChangesAsync();

            return new AuthenticateResponse
            {
                Id = user.Id.ToString(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Username = user.Username,
                Roles = user.Roles,
                IsVerified = user.IsVerified,
                PhoneNumber = user.PhoneNumber,
                StatusCode = 200,
                JWTToken = jwtToken,
                RefreshToken = refreshToken.Token
            };
        }

        public AuthenticateResponse RefreshToken(string refreshToken)
        {
            throw new NotImplementedException();
        }

        public async Task<AuthenticateResponse> RefreshTokenAsync(string refreshToken)
        {
            var existingRefresh = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (existingRefresh == null || existingRefresh.IsExpired)
            {
                return new DomainLayer.DTO.AuthenticateResponse
                {
                    StatusCode = 401,
                    JWTToken = null
                };
            }

            // Get user
            var user = await _userRepository.GetByIdAsync(existingRefresh.UserId);
            if (user == null)
            {
                return new DomainLayer.DTO.AuthenticateResponse
                {
                    StatusCode = 401
                };
            }

            // Generate new JWT
            var jwtToken = GenerateJwtToken(user);

            // Generate a new refresh token
            var newRefreshToken = GenerateRefreshToken(user.Id);

            // Remove old token and save the new one
            await _refreshTokenRepository.DeleteAsync(existingRefresh);
            await _refreshTokenRepository.AddAsync(newRefreshToken);
            await _refreshTokenRepository.SaveChangesAsync();

            return new AuthenticateResponse
            {
                Id = user.Id.ToString(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Username = user.Username,
                Roles = user.Roles,
                IsVerified = user.IsVerified,
                PhoneNumber = user.PhoneNumber,
                StatusCode = 200,
                JWTToken = jwtToken,
                RefreshToken = newRefreshToken.Token
            };
        }

        public AuthenticateResponse RegisterUser(string username, string password, string email)
        {
            throw new NotImplementedException();
        }

        public async Task<AuthenticateResponse> RegisterUserAsync(string username, string password, string email)
        {
            // Check that the user doesn't already exist
            var existing = await _userRepository.GetByUsernameAsync(username);
            if (existing != null)
            {
                return new DomainLayer.DTO.AuthenticateResponse
                {
                    StatusCode = 400,
                    JWTToken = null
                };
            }

            var newUser = new User
            {
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Email = email,
                IsVerified = true,
                Roles = new List<string> { "User" }
            };

            await _userRepository.AddAsync(newUser);
            await _userRepository.SaveChangesAsync();

            // Immediately log in the newly registered user
            var request = new AuthenticateRequest
            {
                Username = username,
                Password = password
            };

            return await AuthenticateAsync(request);
        }

        private string GenerateJwtToken(User user)
        {
            // Recommended to extract expiration time to configuration as well
            var secretKey = _configuration["JwtSettings:SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username ?? string.Empty)
                };

            if (user.Roles != null)
            {
                foreach (var role in user.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Expiration can be extracted from configuration (e.g., JwtSettings:ExpirationHours)
            var tokenDescriptor = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        private RefreshToken GenerateRefreshToken(int userId)
        {
            return new RefreshToken
            {
                UserId = userId,
                Token = Guid.NewGuid().ToString(),
                Expires = DateTime.UtcNow.AddDays(7)
            };
        }
    }
}

