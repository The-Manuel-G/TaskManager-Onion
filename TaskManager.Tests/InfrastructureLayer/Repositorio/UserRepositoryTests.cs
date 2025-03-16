using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using InfrastructureLayer.Repositorio.UserRepository;
using DomainLayer.Models;
using InfrastructureLayer;

public class UserRepositoryTests
{
    private readonly TaskManagerContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        // Use a unique in-memory database for each test to avoid key collisions.
        var options = new DbContextOptionsBuilder<TaskManagerContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TaskManagerContext(options);
        _repository = new UserRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddUser()
    {
        var user = new User { Username = "testuser", Email = "test@example.com", PasswordHash = "hashedpass" };
        await _repository.AddAsync(user);
        await _repository.SaveChangesAsync();

        var savedUser = await _repository.GetByUsernameAsync("testuser");
        Assert.NotNull(savedUser);
        Assert.Equal("test@example.com", savedUser.Email);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser()
    {
        var user = new User { Id = 1, Username = "user1", Email = "juan.perez@eddss.com", PasswordHash = "hashedpass1" };
        await _repository.AddAsync(user);
        await _repository.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(1);
        Assert.NotNull(result);
        Assert.Equal("user1", result.Username);
    }

    [Fact]
    public async Task Delete_ShouldRemoveUser()
    {
        var user = new User { Id = 2, Username = "deleteuser", Email = "delete@example.com", PasswordHash = "hashedpass2" };
        await _repository.AddAsync(user);
        await _repository.SaveChangesAsync();

        _repository.Delete(user);
        await _repository.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(2);
        Assert.Null(result);
    }
}