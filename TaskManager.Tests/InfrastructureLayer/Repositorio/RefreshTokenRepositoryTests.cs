using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using InfrastructureLayer.Repositorio;
using DomainLayer.Models;
using InfrastructureLayer;

public class RefreshTokenRepositoryTests
{
    private readonly TaskManagerContext _context;
    private readonly RefreshTokenRepository _repository;

    public RefreshTokenRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TaskManagerContext>()
            .UseInMemoryDatabase(databaseName: "TestDB")
            .Options;

        _context = new TaskManagerContext(options);
        _repository = new RefreshTokenRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddToken()
    {
        var token = new RefreshToken { Token = "test-token", UserId = 1 };
        await _repository.AddAsync(token);
        await _repository.SaveChangesAsync();

        var savedToken = await _repository.GetByTokenAsync("test-token");
        Assert.NotNull(savedToken);
        Assert.Equal("test-token", savedToken.Token);
    }

    [Fact]
    public async Task GetByTokenAsync_ShouldReturnToken()
    {
        var token = new RefreshToken { Token = "another-token", UserId = 2 };
        await _repository.AddAsync(token);
        await _repository.SaveChangesAsync();

        var result = await _repository.GetByTokenAsync("another-token");
        Assert.NotNull(result);
        Assert.Equal(2, result.UserId);
    }

    [Fact]
    public async Task Delete_ShouldRemoveToken()
    {
        var token = new RefreshToken { Token = "to-delete", UserId = 3 };
        await _repository.AddAsync(token);
        await _repository.SaveChangesAsync();

        _repository.Delete(token);
        await _repository.SaveChangesAsync();

        var result = await _repository.GetByTokenAsync("to-delete");
        Assert.Null(result);
    }
}
