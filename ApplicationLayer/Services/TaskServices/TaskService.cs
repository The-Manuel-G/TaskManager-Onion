using ApplicationLayer.Factories;
using DomainLayer.DTO;
using DomainLayer.Models;
using DomainLayer.Delegates;
using InfrastructureLayer.Repositorio.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.Services.TaskServices
{
    public class TaskService
    {
        private readonly ICommonsProcess<Tareas> _commonsProcess;
        private readonly ValidarTareaDelegate _validarTarea;
        private readonly NotificarCambioDelegate _notificarCambio;
        private readonly ILogger<TaskService> _logger;

        public TaskService(
            ICommonsProcess<Tareas> commonsProcess,
            ValidarTareaDelegate validarTarea,
            NotificarCambioDelegate notificarCambio,
            ILogger<TaskService> logger)
        {
            _commonsProcess = commonsProcess;
            _validarTarea = validarTarea;
            _notificarCambio = notificarCambio;
            _logger = logger;
        }

        public async Task<Response<Tareas>> GetTaskAllAsync()
        {
            var response = new Response<Tareas>();

            try
            {
                response.DataList = await _commonsProcess.GetAllAsync();
                response.Successful = response.DataList != null && response.DataList.Any();
            }
            catch (Exception e)
            {
                _logger.LogError($"Error al obtener todas las tareas: {e.Message}");
                response.Errors.Add(e.Message);
            }

            return response;
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

        public async Task<Response<string>> AddHighPriorityTaskAsync(string description)
        {
            var response = new Response<string>();
            try
            {
                var tarea = ApplicationLayer.Factories.TaskFactory.CreateHighPriorityTask(description);

                if (!_validarTarea(tarea))
                {
                    response.Successful = false;
                    response.Message = "La tarea no es válida";
                    return response;
                }

                var result = await _commonsProcess.AddAsync(tarea);
                response.Successful = result.IsSuccess;
                response.Message = result.Message;

                if (result.IsSuccess)
                {
                    _logger.LogInformation($"Tarea de alta prioridad creada con éxito: {tarea.Id}");
                    _notificarCambio(tarea);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error al agregar una tarea de alta prioridad: {e.Message}");
                response.Errors.Add(e.Message);
            }
            return response;
        }

        public async Task<Response<string>> AddLowPriorityTaskAsync(string description)
        {
            var response = new Response<string>();
            try
            {
                var tarea = ApplicationLayer.Factories.TaskFactory.CreateLowPriorityTask(description);

                if (!_validarTarea(tarea))
                {
                    response.Successful = false;
                    response.Message = "La tarea no es válida";
                    return response;
                }

                var result = await _commonsProcess.AddAsync(tarea);
                response.Successful = result.IsSuccess;
                response.Message = result.Message;

                if (result.IsSuccess)
                {
                    _logger.LogInformation($"Tarea de baja prioridad creada con éxito: {tarea.Id}");
                    _notificarCambio(tarea);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error al agregar una tarea de baja prioridad: {e.Message}");
                response.Errors.Add(e.Message);
            }
            return response;
        }

        public async Task<Response<string>> AddCustomTaskAsync(string description, DateTime dueDate, string additionalData)
        {
            var response = new Response<string>();
            try
            {
                var tarea = ApplicationLayer.Factories.TaskFactory.CreateCustomTask(description, dueDate, additionalData);

                if (!_validarTarea(tarea))
                {
                    response.Successful = false;
                    response.Message = "La tarea no es válida";
                    return response;
                }

                var result = await _commonsProcess.AddAsync(tarea);
                response.Successful = result.IsSuccess;
                response.Message = result.Message;

                if (result.IsSuccess)
                {
                    _logger.LogInformation($"Tarea personalizada creada con éxito: {tarea.Id}");
                    _notificarCambio(tarea);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error al agregar una tarea personalizada: {e.Message}");
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
                    response.Successful = false;
                    response.Message = "La tarea no es válida";
                    return response;
                }

                var result = await _commonsProcess.UpdateAsync(tarea);
                response.Successful = result.IsSuccess;
                response.Message = result.Message;

                if (result.IsSuccess)
                {
                    _logger.LogInformation($"Tarea {tarea.Id} actualizada con éxito.");
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
                response.Successful = result.IsSuccess;
                response.Message = result.Message;

                if (result.IsSuccess)
                {
                    _logger.LogInformation($"Tarea eliminada con éxito: {id}");
                    _notificarCambio(tareaEliminada);
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