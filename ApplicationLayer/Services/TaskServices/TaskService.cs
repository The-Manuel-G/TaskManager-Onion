using DomainLayer.DTO;
using DomainLayer.Models;
using DomainLayer.Delegates;
using InfrastructureLayer.Repositorio.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;  // Se agregó para manejar logs

namespace ApplicationLayer.Services.TaskServices
{
    public class TaskService
    {
        private readonly ICommonsProcess<Tareas> _commonsProcess;
        private readonly ValidarTareaDelegate _validarTarea;
        private readonly NotificarCambioDelegate _notificarCambio;
        private readonly ILogger<TaskService> _logger; // Agregado para logs

        public TaskService(
            ICommonsProcess<Tareas> commonsProcess,
            ValidarTareaDelegate validarTarea,
            NotificarCambioDelegate notificarCambio,
            ILogger<TaskService> logger) // Inyectamos el logger
        {
            _commonsProcess = commonsProcess;
            _validarTarea = validarTarea;
            _notificarCambio = notificarCambio;
            _logger = logger;
        }

        public async Task<Response<Tareas>> GetTaskAllAsync()
        {
            var response = new Response<Tareas>(); // ✅ Se inicializa correctamente

            try
            {
                response.DataList = await _commonsProcess.GetAllAsync();
                response.Successful = response.DataList != null && response.DataList.Any(); // ✅ Se verifica si hay datos
            }
            catch (Exception e)
            {
                _logger.LogError($"Error al obtener todas las tareas: {e.Message}");
                response.Errors.Add(e.Message);
            }

            return response; // ✅ Siempre devuelve un objeto válido
        }

        public async Task<Response<Tareas>> GetTaskByIdAllAsync(int id)
        {
            var response = new Response<Tareas>();
            try
            {
                var result = await _commonsProcess.GetIdAsync(id);
                response.Successful = result != null;

                if (response.Successful)
                {
                    response.SingleData = result;
                }
                else
                {
                    response.Message = "No se encontró información...";
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error al obtener la tarea con ID {id}: {e.Message}");
                response.Errors.Add(e.Message);
            }
            return response;
        }

        public async Task<Response<string>> AddTaskAllAsync(Tareas tarea)
        {
            var response = new Response<string>();
            try
            {
                if (!_validarTarea(tarea))
                {
                    _logger.LogWarning($"Validación fallida al agregar la tarea: {tarea.Description}");
                    response.Successful = false;
                    response.Message = "La tarea no es válida";
                    return response;
                }

                var result = await _commonsProcess.AddAsync(tarea);
                response.Message = result.Message;
                response.Successful = result.IsSuccess;

                if (result.IsSuccess)
                {
                    _logger.LogInformation($"Tarea creada con éxito: {tarea.Id}");
                    _notificarCambio(tarea);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error al agregar una tarea: {e.Message}");
                response.Errors.Add(e.Message);
            }
            return response;
        }

        public async Task<Response<string>> UpdateTaskAllAsync(Tareas tarea)
        {
            var response = new Response<string>();
            try
            {
                if (!_validarTarea(tarea))
                {
                    _logger.LogWarning($"Validación fallida al actualizar la tarea {tarea.Id}");
                    response.Successful = false;
                    response.Message = "La tarea no es válida";
                    return response;
                }

                var result = await _commonsProcess.UpdateAsync(tarea);
                response.Message = result.Message;
                response.Successful = result.IsSuccess;

                if (result.IsSuccess)
                {
                    _logger.LogInformation($"Tarea actualizada con éxito: {tarea.Id}");
                    _notificarCambio(tarea);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error al actualizar la tarea {tarea.Id}: {e.Message}");
                response.Errors.Add(e.Message);
            }
            return response;
        }

        public async Task<Response<string>> DeleteTaskAllAsync(int id)
        {
            var response = new Response<string>();
            try
            {
                var tareaEliminada = await _commonsProcess.GetIdAsync(id);
                if (tareaEliminada == null)
                {
                    response.Successful = false;
                    response.Message = "No se encontró la tarea para eliminar";
                    return response;
                }

                var result = await _commonsProcess.DeleteAsync(id);
                response.Message = result.Message;
                response.Successful = result.IsSuccess;

                if (result.IsSuccess)
                {
                    _logger.LogInformation($"Tarea eliminada con éxito: {id}");
                    _notificarCambio(tareaEliminada); // ✅ Ahora pasamos la tarea completa
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error al eliminar la tarea con ID {id}: {e.Message}");
                response.Errors.Add(e.Message);
            }
            return response;
        }
    }
}
