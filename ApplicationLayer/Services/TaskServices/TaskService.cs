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
using Microsoft.Extensions.DependencyInjection;
using ApplicationLayer.Optimizations;
using InfrastructureLayer.Repositorio.UserRepository;
using DomainLayer;
using TaskFactory = ApplicationLayer.Factories.TaskFactory;

namespace ApplicationLayer.Services.TaskServices
{
    public class TaskService
    {
        private readonly ICommonsProcess<Tareas> _commonsProcess;
        private readonly ITaskQueue _taskQueue;
        private readonly IUserRepository _userRepository;
        private readonly ValidarTareaDelegate _validarTarea;
        private readonly NotificarCambioDelegate _notificarCambio;
        private readonly ILogger<TaskService> _logger;

        public TaskService(
            ICommonsProcess<Tareas> commonsProcess,
            ITaskQueue taskQueue,
            IUserRepository userRepository,
            ValidarTareaDelegate validarTarea,
            NotificarCambioDelegate notificarCambio,
            ILogger<TaskService> logger)
        {
            _commonsProcess = commonsProcess;
            _taskQueue = taskQueue;
            _userRepository = userRepository;
            _validarTarea = validarTarea;
            _notificarCambio = notificarCambio;
            _logger = logger;
        }

        private async Task<bool> UserExists(int userId)
        {
            return await _userRepository.GetByIdAsync(userId) != null;
        }
        // Métodos de consulta (lectura)
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

        // Método privado para verificar la existencia del usuario


        // Método privado común para agregar una tarea
        public async Task<Response<string>> AddTaskAsync(Tareas tarea)
        {
            var response = new Response<string>();
            try
            {
                if (!await UserExists(tarea.UserId))
                {
                    response.Successful = false;
                    response.Message = "El usuario especificado no existe.";
                    return response;
                }

                if (!_validarTarea(tarea))
                {
                    response.Successful = false;
                    response.Message = "La tarea no es válida.";
                    return response;
                }

                var result = await _commonsProcess.AddAsync(tarea);
                if (result.IsSuccess)
                {
                    _logger.LogInformation($"Tarea creada con éxito: {tarea?.Id}");
                    _notificarCambio(tarea);
                    response.Successful = true;
                    response.Message = "Tarea creada con éxito.";
                }
                else
                {
                    _logger.LogError($"Error al agregar la tarea: {result.Message}");
                    response.Successful = false;
                    response.Message = result.Message;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error inesperado: {e.Message}");
                response.Successful = false;
                response.Errors.Add(e.Message);
            }
            return response;
        }

        // Métodos para agregar tareas utilizando el patrón de fábrica
        public async Task<Response<string>> AddHighPriorityTaskAsync(string description, int userId)
        {
            var tarea = TaskFactory.CreateHighPriorityTask(description, userId);
            return await AddTaskAsync(tarea);
        }

        public async Task<Response<string>> AddLowPriorityTaskAsync(string description, int userId)
        {
            var tarea = TaskFactory.CreateLowPriorityTask(description, userId);
            return await AddTaskAsync(tarea);
        }

        public async Task<Response<string>> AddCustomTaskAsync(string description, DateTime dueDate, string additionalData, int userId)
        {
            var tarea = TaskFactory.CreateCustomTask(description, dueDate, additionalData, userId);
            return await AddTaskAsync(tarea);
        }

        // Actualización de tareas
        public async Task<Response<string>> UpdateTaskAllAsync(Tareas tarea)
        {
            var response = new Response<string>();
            try
            {
                if (!_validarTarea(tarea))
                {
                    response.Successful = false;
                    response.Message = "La tarea no es válida.";
                    return response;
                }

                var result = await _commonsProcess.UpdateAsync(tarea);
                response.Successful = result.IsSuccess;
                response.Message = result.Message;

                if (result.IsSuccess)
                {
                    _logger.LogInformation($"Tarea {tarea?.Id} actualizada con éxito.");
                    _notificarCambio(tarea);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error al actualizar la tarea {tarea?.Id}: {e.Message}");
                response.Errors.Add(e.Message);
            }
            return response;
        }

        // Eliminación de tareas
        public async Task<Response<string>> DeleteTaskAllAsync(int id)
        {
            var response = new Response<string>();
            try
            {
                var tareaEliminada = await _commonsProcess.GetIdAsync(id);
                if (tareaEliminada == null)
                {
                    response.Successful = false;
                    response.Message = "No se encontró la tarea para eliminar.";
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