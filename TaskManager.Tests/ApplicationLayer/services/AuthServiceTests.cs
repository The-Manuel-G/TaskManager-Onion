using ApplicationLayer.Services.AuthServices;
using DomainLayer.DTO;
using DomainLayer.Models;
using InfrastructureLayer.Repositorio.UserRepository;
using InfrastructureLayer.Repositorio;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using ApplicationLayer.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        _configurationMock = new Mock<IConfiguration>();

        // Se actualiza la clave secreta para cumplir con el tamaño mínimo requerido para HS256
        _configurationMock.Setup(c => c["JwtSettings:SecretKey"]).Returns("MySuperSecretKeyMySuperSecretKey");

        _authService = new AuthService(
            _userRepositoryMock.Object,
            _refreshTokenRepositoryMock.Object,
            _configurationMock.Object);
    }

    [Fact]
    public async Task AuthenticateAsync_ValidUser_ReturnsSuccess()
    {
        // Arrange
        var request = new AuthenticateRequest { Username = "testuser", Password = "password123" };
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            Email = "test@example.com",
            IsVerified = true,
            Roles = new List<string> { "User" }
        };

        _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(request.Username)).ReturnsAsync(user);

        // Act
        var response = await _authService.AuthenticateAsync(request);

        // Assert
        Assert.NotNull(response.JWTToken);
        Assert.Equal(200, response.StatusCode);
    }

    [Fact]
    public async Task AuthenticateAsync_InvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var request = new AuthenticateRequest { Username = "testuser", Password = "wrongpassword" };
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123")
        };

        _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(request.Username)).ReturnsAsync(user);

        // Act
        var response = await _authService.AuthenticateAsync(request);

        // Assert
        Assert.Equal(401, response.StatusCode);
    }

    [Fact]
    public async Task AuthenticateAsync_NonExistingUser_ReturnsUnauthorized()
    {
        // Arrange
        var request = new AuthenticateRequest { Username = "nonexistent", Password = "password123" };
        _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(request.Username)).ReturnsAsync((User)null);

        // Act
        var response = await _authService.AuthenticateAsync(request);

        // Assert
        Assert.Equal(401, response.StatusCode);
    }
}