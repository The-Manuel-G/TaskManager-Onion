using InfrastructureLayer.Repositorio.UserRepository;
using Microsoft.AspNetCore.Mvc;
using DomainLayer.Models;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Linq;
using BCrypt.Net;

namespace TaskManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [Authorize(Roles = "Admin,Dev,Client")] // Permite Admin, Dev y Client
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound(new { message = "Usuario no encontrado" });
            return Ok(user);
        }

        [AllowAnonymous] // Permitir que cualquier usuario se registre
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            if (user == null) return BadRequest(new { message = "Datos de usuario inválidos" });

            // Validar que el rol sea "Admin", "Dev" o "Client"
            var validRoles = new string[] { "Admin", "Dev", "Client" };
            if (user.Roles == null || !user.Roles.All(r => validRoles.Contains(r)))
            {
                return BadRequest(new { message = "Rol inválido. Solo se permiten: Admin, Dev, Client" });
            }

            // Validar si el usuario ya existe
            var existingUser = await _userRepository.GetByUsernameAsync(user.Username);
            if (existingUser != null)
            {
                return BadRequest(new { message = "El nombre de usuario ya está en uso." });
            }

            // 🔹 Hashear la contraseña antes de guardarla
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        [Authorize(Roles = "Admin")] // Solo Admin puede actualizar usuarios
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] User userData)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound(new { message = "Usuario no encontrado" });

            // Validar que el rol sea "Admin", "Dev" o "Client"
            var validRoles = new string[] { "Admin", "Dev", "Client" };
            if (userData.Roles == null || !userData.Roles.All(r => validRoles.Contains(r)))
            {
                return BadRequest(new { message = "Rol inválido. Solo se permiten: Admin, Dev, Client" });
            }

            user.FirstName = userData.FirstName;
            user.LastName = userData.LastName;
            user.Email = userData.Email;
            user.Username = userData.Username;
            user.Roles = userData.Roles;

            // 🔹 Si la contraseña fue enviada, la actualizamos y la hasheamos
            if (!string.IsNullOrEmpty(userData.PasswordHash))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userData.PasswordHash);
            }

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return Ok(user);
        }

        [Authorize(Roles = "Admin")] // Solo Admin puede eliminar usuarios
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound(new { message = "Usuario no encontrado" });

            _userRepository.Delete(user);
            await _userRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
