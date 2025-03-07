using InfrastructureLayer.Repositorio;
using Microsoft.AspNetCore.Mvc;
using DomainLayer.Models;
using Microsoft.AspNetCore.Authorization;

namespace TaskManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // Ejemplo: solo Admin puede gestionar usuarios
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _userRepository.GetById(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public IActionResult Create(User user)
        {
            _userRepository.Add(user);
            _userRepository.SaveChangesAsync().Wait();
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, User userData)
        {
            var user = _userRepository.GetById(id);
            if (user == null) return NotFound();

            user.FirstName = userData.FirstName;
            user.LastName = userData.LastName;
            user.Email = userData.Email;
            user.Username = userData.Username;
            // etc.

            _userRepository.Update(user);
            _userRepository.SaveChangesAsync().Wait();
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var user = _userRepository.GetById(id);
            if (user == null) return NotFound();

            _userRepository.Delete(user);
            _userRepository.SaveChangesAsync().Wait();
            return NoContent();
        }
    }
}
