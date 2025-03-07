using DomainLayer.DTO;
using ApplicationLayer.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationLayer.Services.AuthServices;
using Microsoft.AspNetCore.Authorization;


namespace TaskManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] AuthenticateRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                return BadRequest(new { message = "Faltan credenciales" });

            var response = _authService.Authenticate(request);
            if (response.StatusCode == 401 || string.IsNullOrEmpty(response.JWTToken))
                return Unauthorized(new { message = "Usuario o contraseña incorrectos" });

            return Ok(response);
        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshTokenRequest request)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
                return BadRequest(new { message = "Falta refreshToken" });

            var response = _authService.RefreshToken(request.RefreshToken);
            if (response.StatusCode == 401 || string.IsNullOrEmpty(response.JWTToken))
                return Unauthorized(new { message = "Refresh token inválido o expirado" });

            return Ok(response);
        }

        [HttpPost("register")]
        public IActionResult Register(string username, string password, string email)
        {
            var response = _authService.RegisterUser(username, password, email);
            if (response.StatusCode == 400)
                return BadRequest(new { message = "El usuario ya existe" });

            return Ok(response);
        }
    }
}
