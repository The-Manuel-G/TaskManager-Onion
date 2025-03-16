using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using FluentAssertions;
using TaskManager.Controllers;
using InfrastructureLayer.Repositorio.UserRepository;
using DomainLayer.Models;
using System.Collections.Generic;

public class UserControllerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _controller = new UserController(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task GetById_Returns_User_When_Exists()
    {
        // ✅ Prueba: Verifica que obtenemos un usuario si existe en la base de datos.
        var user = new User { Id = 1, Username = "admin" };
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(user);

        var result = await _controller.GetById(1) as OkObjectResult;

        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Value.Should().Be(user);
    }

    [Fact]
    public async Task GetById_Returns_NotFound_When_User_Does_Not_Exist()
    {
        // ✅ Prueba: Devuelve 404 Not Found si el usuario no existe.
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((User)null);

        var result = await _controller.GetById(1) as NotFoundObjectResult;

        result.Should().NotBeNull();
        result.StatusCode.Should().Be(404);
        result.Value.Should().BeEquivalentTo(new { message = "Usuario no encontrado" });
    }

    [Fact]
    public async Task Create_Returns_Created_When_Successful()
    {
        // ✅ Prueba: Crea un nuevo usuario y devuelve CreatedAtAction.
        var user = new User { Id = 1, Username = "newuser", PasswordHash = "hashedpass", Roles = new List<string> { "Client" } };

        _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(user.Username)).ReturnsAsync((User)null);
        _userRepositoryMock.Setup(repo => repo.AddAsync(user)).Returns(Task.CompletedTask);

        var result = await _controller.Create(user) as CreatedAtActionResult;

        result.Should().NotBeNull();
        result.StatusCode.Should().Be(201);
        result.Value.Should().Be(user);
    }

    [Fact]
    public async Task Create_Returns_BadRequest_When_User_Exists()
    {
        // ✅ Prueba: Si el usuario ya existe, devuelve 400 BadRequest.
        var user = new User { Id = 1, Username = "existinguser", Roles = new List<string> { "Client" } };
        _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(user.Username)).ReturnsAsync(user);

        var result = await _controller.Create(user) as BadRequestObjectResult;

        result.Should().NotBeNull();
        result.StatusCode.Should().Be(400);
        result.Value.Should().BeEquivalentTo(new { message = "El nombre de usuario ya está en uso." });
    }

    [Fact]
    public async Task Update_Returns_Ok_When_Successful()
    {
        // ✅ Prueba: Actualiza un usuario existente y devuelve 200 OK.
        var user = new User { Id = 1, Username = "updateduser", Roles = new List<string> { "Client" } };
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);
        _userRepositoryMock.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _controller.Update(1, user) as OkObjectResult;

        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Value.Should().Be(user);
    }

    [Fact]
    public async Task Delete_Returns_NoContent_When_Successful()
    {
        // ✅ Prueba: Elimina un usuario existente y devuelve 204 NoContent.
        var user = new User { Id = 1 };
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);

        var result = await _controller.Delete(1) as NoContentResult;

        result.Should().NotBeNull();
        result.StatusCode.Should().Be(204);
    }

    [Fact]
    public async Task Delete_Returns_NotFound_When_User_Does_Not_Exist()
    {
        // ✅ Prueba: Devuelve 404 Not Found si el usuario a eliminar no existe.
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((User)null);

        var result = await _controller.Delete(1) as NotFoundObjectResult;

        result.Should().NotBeNull();
        result.StatusCode.Should().Be(404);
        result.Value.Should().BeEquivalentTo(new { message = "Usuario no encontrado" });
    }
}