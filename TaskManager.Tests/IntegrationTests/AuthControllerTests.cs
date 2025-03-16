using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Controllers;
using ApplicationLayer.Services.AuthServices;
using DomainLayer.DTO;
using FluentAssertions;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _controller = new AuthController(_authServiceMock.Object);
    }

    [Fact]
    public void Login_Returns_Token_When_Valid_Credentials()
    {
        // ✅ Prueba: Verifica que el login exitoso devuelve un token JWT

        var request = new AuthenticateRequest { Username = "admin", Password = "password" };
        var response = new AuthenticateResponse { StatusCode = 200, JWTToken = "fake_token" };

        _authServiceMock.Setup(s => s.Authenticate(request)).Returns(response);

        var result = _controller.Login(request) as OkObjectResult;

        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
    }

    [Fact]
    public void Register_Returns_BadRequest_If_User_Exists()
    {
        // ✅ Prueba: Intentar registrar un usuario ya existente devuelve 400 BadRequest

        _authServiceMock.Setup(s => s.RegisterUser("admin", "password", "admin@mail.com"))
                        .Returns(new AuthenticateResponse { StatusCode = 400 });

        var result = _controller.Register("admin", "password", "admin@mail.com") as BadRequestObjectResult;

        result.Should().NotBeNull();
        result.StatusCode.Should().Be(400);
    }
}
