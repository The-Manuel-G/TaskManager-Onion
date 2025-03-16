using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using InfrastructureLayer.Repositorio.TaskReprository;
using DomainLayer.Models;
using DomainLayer.Delegates;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using InfrastructureLayer.SignalR;
using InfrastructureLayer;

public class TaskRepositoryTests
{
    private readonly TaskManagerContext _context;
    private readonly TaskRepository _repository;

    public TaskRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TaskManagerContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB instance per test
            .Options;

        _context = new TaskManagerContext(options);

        // Create mocks for the dependencies.
        var validarTareaMock = new Mock<ValidarTareaDelegate>();
        validarTareaMock.Setup(v => v(It.IsAny<Tareas>())).Returns(true);

        var notificarCambioMock = new Mock<NotificarCambioDelegate>();
        var hubContextMock = new Mock<IHubContext<TaskHub>>();
        var loggerMock = new Mock<ILogger<TaskRepository>>();

        _repository = new TaskRepository(
            _context,
            validarTareaMock.Object,       // Mock for task validation
            notificarCambioMock.Object,    // Mock for notification
            hubContextMock.Object,         // Mock for SignalR hub context
            loggerMock.Object              // Mock for Logger
        );
    }

    
    [Fact]
    public async Task GetByIdAsync_ShouldReturnTask()
    {
        // Arrange: Ensure that a user exists.
        var existingUser = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com"
        };
        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var task = new Tareas
        {
            Description = "Test Task",
            Status = "Completed",
            UserId = 1,
            AdditionalData = "Test data" // Required property provided
        };
        await _repository.AddAsync(task);
        await _repository.SaveChangesAsync();

        // Get the auto-generated Id from EF
        var insertedTaskId = task.Id;

        // Act
        var result = await _repository.GetIdAsync(insertedTaskId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Task", result.Description);
    }

  
}