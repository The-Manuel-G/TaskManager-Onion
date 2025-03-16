


    using ApplicationLayer.Services.TaskServices;
using DomainLayer.Delegates;
using DomainLayer.DTO;
using DomainLayer.Models;
using InfrastructureLayer.Repositorio.Commons;
using InfrastructureLayer.Repositorio.UserRepository;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class TaskServiceTests
{
    private readonly Mock<ICommonsProcess<Tareas>> _taskRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ILogger<TaskService>> _loggerMock;
    private readonly Mock<ValidarTareaDelegate> _validarTareaMock;
    // Se crea un delegado de notificar cambio que no hace nada
    private readonly NotificarCambioDelegate _notificarCambio;

    private readonly TaskService _taskService;

    public TaskServiceTests()
    {
        _taskRepositoryMock = new Mock<ICommonsProcess<Tareas>>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<TaskService>>();
        _validarTareaMock = new Mock<ValidarTareaDelegate>();

        _notificarCambio = (tarea) => { }; // Se define un delegado vacío

        _taskService = new TaskService(
            _taskRepositoryMock.Object,
            null,
            _userRepositoryMock.Object,
            _validarTareaMock.Object,
            _notificarCambio,  // Ahora se pasa el delegado correcto
            _loggerMock.Object);
    }


    [Fact]
    public async Task AddTaskAsync_ValidUser_ReturnsSuccess()
    {
        // Arrange
        var tarea = new Tareas { Id = 4, Description = "New Task", UserId = 1 };

        // Simulamos que el usuario existe
        _userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(tarea.UserId))
            .ReturnsAsync(new User { Id = tarea.UserId });

        // Simulamos que la validación de la tarea pasa
        _validarTareaMock
            .Setup(v => v(It.IsAny<Tareas>()))
            .Returns(true);

        // Simulamos que la tarea se agrega correctamente
        _taskRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Tareas>()))
            .ReturnsAsync((true, "Tarea creada con éxito."));

        // Act
        var response = await _taskService.AddTaskAsync(tarea);

        // Assert
        Assert.NotNull(response); // Verifica que la respuesta no es nula
        Assert.True(response.Successful); // Asegura que la tarea fue agregada con éxito
        Assert.Equal("Tarea creada con éxito.", response.Message); // Verifica mensaje correcto

        // Validar que los métodos de los mocks fueron llamados correctamente
        _userRepositoryMock.Verify(repo => repo.GetByIdAsync(tarea.UserId), Times.Once);
        _taskRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Tareas>()), Times.Once);
        _validarTareaMock.Verify(v => v(It.IsAny<Tareas>()), Times.Once);
    }


    [Fact]
    public async Task AddTaskAsync_InvalidUser_ReturnsFailure()
    {
        // Arrange
        var tarea = new Tareas { Id = 1, Description = "New Task", UserId = 99 };

        // Simulamos que el usuario NO existe
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(tarea.UserId)).ReturnsAsync((User)null);

        // Act
        var response = await _taskService.AddTaskAsync(tarea);

        // Assert
        Assert.False(response.Successful);
        Assert.Equal("El usuario especificado no existe.", response.Message);
    }

    [Fact]
    public async Task GetTaskByIdAsync_ValidId_ReturnsTask()
    {
        // Arrange
        var tarea = new Tareas { Id = 1, Description = "Test Task", UserId = 1 };
        _taskRepositoryMock.Setup(repo => repo.GetIdAsync(tarea.Id)).ReturnsAsync(tarea);

        // Act
        var response = await _taskService.GetTaskByIdAllAsync(tarea.Id);

        // Assert
        Assert.True(response.Successful);
        Assert.Equal(tarea.Id, response.SingleData.Id);
    }

    [Fact]
    public async Task GetTaskByIdAsync_InvalidId_ReturnsNotFound()
    {
        // Arrange
        _taskRepositoryMock.Setup(repo => repo.GetIdAsync(It.IsAny<int>())).ReturnsAsync((Tareas)null);

        // Act
        var response = await _taskService.GetTaskByIdAllAsync(99);

        // Assert
        Assert.False(response.Successful);
        Assert.Equal("No se encontró información...", response.Message);
    }
}
