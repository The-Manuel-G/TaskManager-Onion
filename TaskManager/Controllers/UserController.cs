using InfrastructureLayer.Repositorio.UserRepository;
using Microsoft.AspNetCore.Mvc;
using DomainLayer.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
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

      
        // 🔹 Obtener un usuario por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound(new { message = "Usuario no encontrado" });
            return Ok(user);
        }

        // 🔹 Crear un usuario
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            if (user == null) return BadRequest(new { message = "Datos de usuario inválidos" });

            // Verificar si el usuario ya existe
            var existingUser = await _userRepository.GetByUsernameAsync(user.Username);
            if (existingUser != null)
            {
                return BadRequest(new { message = "El nombre de usuario ya está en uso." });
            }

            // Hashear la contraseña antes de guardarla
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        // 🔹 Actualizar un usuario por ID
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] User userData)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound(new { message = "Usuario no encontrado" });

            user.FirstName = userData.FirstName;
            user.LastName = userData.LastName;
            user.Email = userData.Email;
            user.Username = userData.Username;

            // Si se envió una nueva contraseña, la actualizamos
            if (!string.IsNullOrEmpty(userData.PasswordHash))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userData.PasswordHash);
            }

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return Ok(user);
        }

        // 🔹 Eliminar un usuario por ID
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
