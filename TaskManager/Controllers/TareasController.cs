using ApplicationLayer.Services.TaskServices;
using DomainLayer.DTO;
using DomainLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace TaskManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TareasController : ControllerBase
    {
        private readonly TaskService _service;
        private readonly ILogger<TareasController> _logger;

        public TareasController(TaskService service, ILogger<TareasController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<Response<Tareas>>> GetTaskAllAsync()
        {
            try
            {
                var response = await _service.GetTaskAllAsync(); // ✅ Se llama correctamente

                if (response == null || response.DataList == null || !response.DataList.Any()) // ✅ Corrección de la comparación
                {
                    _logger.LogWarning("No hay tareas disponibles.");
                    return NotFound("No se encontraron tareas.");
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener las tareas: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }



        /// <summary>
        /// Obtiene una tarea por ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Response<Tareas>>> GetTaskByIdAllAsync(int id)
        {
            try
            {
                var response = await _service.GetTaskByIdAllAsync(id);
                if (response == null || !response.Successful)
                {
                    _logger.LogWarning($"Tarea con ID {id} no encontrada.");
                    return NotFound(response?.Message ?? "Tarea no encontrada.");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener la tarea con ID {id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Agrega una nueva tarea.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Response<string>>> AddTaskAllAsync([FromBody] Tareas tarea)
        {
            if (tarea == null || string.IsNullOrWhiteSpace(tarea.Description))
            {
                _logger.LogWarning("Solicitud inválida: la tarea está vacía.");
                return BadRequest("La tarea no puede estar vacía.");
            }

            try
            {
                var response = await _service.AddTaskAllAsync(tarea);
                if (!response.Successful)
                {
                    _logger.LogWarning($"Error al agregar la tarea: {response.Message}");
                    return BadRequest(response.Message);
                }

                _logger.LogInformation($"Tarea agregada con éxito: {tarea.Id}");
                return CreatedAtAction(nameof(GetTaskByIdAllAsync), new { id = tarea.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al agregar una tarea: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Actualiza una tarea existente.
        /// </summary>
        [HttpPut]
        public async Task<ActionResult<Response<string>>> UpdateTaskAllAsync([FromBody] Tareas tarea)
        {
            if (tarea == null || tarea.Id <= 0)
            {
                _logger.LogWarning("Solicitud inválida: ID de tarea no válido.");
                return BadRequest("ID de tarea no válido.");
            }

            try
            {
                var response = await _service.UpdateTaskAllAsync(tarea);
                if (!response.Successful)
                {
                    _logger.LogWarning($"Error al actualizar la tarea {tarea.Id}: {response.Message}");
                    return BadRequest(response.Message);
                }

                _logger.LogInformation($"Tarea {tarea.Id} actualizada con éxito.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la tarea {tarea.Id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Elimina una tarea por ID.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Response<string>>> DeleteTaskAllAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Solicitud inválida: ID de tarea no válido.");
                return BadRequest("ID de tarea no válido.");
            }

            try
            {
                var response = await _service.DeleteTaskAllAsync(id);
                if (response == null || !response.Successful)
                {
                    _logger.LogWarning($"Error al eliminar la tarea {id}: {response?.Message ?? "Tarea no encontrada."}");
                    return NotFound(response?.Message ?? "No se encontró la tarea.");
                }

                _logger.LogInformation($"Tarea {id} eliminada con éxito.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar la tarea {id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }
    }
}
