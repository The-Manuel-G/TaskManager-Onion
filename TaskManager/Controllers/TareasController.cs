using ApplicationLayer.Services.TaskServices;
using DomainLayer.DTO;
using DomainLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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

        /// <summary>
        /// Obtiene todas las tareas.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<Response<Tareas>>> GetTaskAllAsync()
        {
            try
            {
                var response = await _service.GetTaskAllAsync();
                if (response == null || response.DataList == null || !response.DataList.Any())
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
        /// Agrega una nueva tarea de alta prioridad.
        /// </summary>
        [HttpPost("high-priority")]
        public async Task<ActionResult<Response<string>>> AddHighPriorityTaskAsync([FromBody] TaskRequestDTO taskRequest)
        {
            if (taskRequest == null || string.IsNullOrWhiteSpace(taskRequest.Description) || taskRequest.UserId <= 0)
            {
                _logger.LogWarning("Solicitud inválida: descripción de la tarea o ID de usuario no válido.");
                return BadRequest("La descripción de la tarea y el ID de usuario no pueden estar vacíos.");
            }

            try
            {
                var response = await _service.AddHighPriorityTaskAsync(taskRequest.Description, taskRequest.UserId);
                if (!response.Successful)
                {
                    _logger.LogWarning($"Error al agregar la tarea de alta prioridad: {response.Message}");
                    return BadRequest(response.Message);
                }

                _logger.LogInformation($"Tarea de alta prioridad agregada con éxito.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al agregar una tarea de alta prioridad: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Agrega una nueva tarea de baja prioridad.
        /// </summary>
        [HttpPost("low-priority")]
        public async Task<ActionResult<Response<string>>> AddLowPriorityTaskAsync([FromBody] TaskRequestDTO taskRequest)
        {
            if (taskRequest == null || string.IsNullOrWhiteSpace(taskRequest.Description) || taskRequest.UserId <= 0)
            {
                _logger.LogWarning("Solicitud inválida: descripción de la tarea o ID de usuario no válido.");
                return BadRequest("La descripción de la tarea y el ID de usuario no pueden estar vacíos.");
            }

            try
            {
                var response = await _service.AddLowPriorityTaskAsync(taskRequest.Description, taskRequest.UserId);
                if (!response.Successful)
                {
                    _logger.LogWarning($"Error al agregar la tarea de baja prioridad: {response.Message}");
                    return BadRequest(response.Message);
                }

                _logger.LogInformation($"Tarea de baja prioridad agregada con éxito.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al agregar una tarea de baja prioridad: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Agrega una nueva tarea personalizada.
        /// </summary>
        [HttpPost("custom")]
        public async Task<ActionResult<Response<string>>> AddCustomTaskAsync([FromBody] CustomTaskDTO customTask)
        {
            if (customTask == null || string.IsNullOrWhiteSpace(customTask.Description) || customTask.UserId <= 0)
            {
                _logger.LogWarning("Solicitud inválida: la tarea personalizada está vacía o el ID de usuario no es válido.");
                return BadRequest("La tarea personalizada y el ID de usuario no pueden estar vacíos.");
            }

            try
            {
                var response = await _service.AddCustomTaskAsync(customTask.Description, customTask.DueDate, customTask.AdditionalData, customTask.UserId);
                if (!response.Successful)
                {
                    _logger.LogWarning($"Error al agregar la tarea personalizada: {response.Message}");
                    return BadRequest(response.Message);
                }

                _logger.LogInformation($"Tarea personalizada agregada con éxito.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al agregar una tarea personalizada: {ex.Message}");
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

    public class TaskRequestDTO
    {
        public string Description { get; set; }
        public int UserId { get; set; }
    }

    public class CustomTaskDTO
    {
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string AdditionalData { get; set; }
        public int UserId { get; set; }
    }
}