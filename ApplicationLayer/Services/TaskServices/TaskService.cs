using ApplicationLayer.Factories;
using DomainLayer.DTO;
using DomainLayer.Models;
using DomainLayer.Delegates;
using InfrastructureLayer.Repositorio.Commons;
using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;
using DomainLayer;
using AppTaskFactory = ApplicationLayer.Factories.TaskFactory;
using InfrastructureLayer.Repositorio;
using Microsoft.Extensions.DependencyInjection;
using ApplicationLayer.Optimizations; // Asegúrate de agregar este using

namespace ApplicationLayer.Services.TaskServices
{
    public class TaskService
    {
        private readonly ICommonsProcess<Tareas> _commonsProcess;
        private readonly ITaskQueue _taskQueue;
        private readonly ValidarTareaDelegate _validarTarea;
        private readonly NotificarCambioDelegate _notificarCambio;
        private readonly ILogger<TaskService> _logger;

        public TaskService(
            ICommonsProcess<Tareas> commonsProcess,
            ITaskQueue taskQueue,
            ValidarTareaDelegate validarTarea,
            NotificarCambioDelegate notificarCambio,
            ILogger<TaskService> logger)
        {
            _commonsProcess = commonsProcess;
            _taskQueue = taskQueue;
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
                var tarea = AppTaskFactory.CreateHighPriorityTask(description);

                if (!_validarTarea(tarea))
                {
                    response.Successful = false;
                    response.Message = "La tarea no es válida";
                    return response;
                }

                _taskQueue.Enqueue(async serviceProvider =>
                {
                    var scopedProcess = serviceProvider.GetRequiredService<ICommonsProcess<Tareas>>();
                    var result = await scopedProcess.AddAsync(tarea);

                    if (result.IsSuccess)
                    {
                        _logger.LogInformation($"[Queue] Tarea de alta prioridad creada con éxito: {tarea?.Id}");
                        _notificarCambio(tarea);
                    }
                    else
                    {
                        _logger.LogError($"[Queue] Error al agregar la tarea de alta prioridad: {result.Message}");
                    }
                });

                response.Successful = true;
                response.Message = "Tarea agregada a la cola de procesamiento";
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
                var tarea = AppTaskFactory.CreateLowPriorityTask(description);

                if (!_validarTarea(tarea))
                {
                    response.Successful = false;
                    response.Message = "La tarea no es válida";
                    return response;
                }

                _taskQueue.Enqueue(async serviceProvider =>
                {
                    var scopedProcess = serviceProvider.GetRequiredService<ICommonsProcess<Tareas>>();
                    var result = await scopedProcess.AddAsync(tarea);

                    if (result.IsSuccess)
                    {
                        _logger.LogInformation($"[Queue] Tarea de baja prioridad creada con éxito: {tarea?.Id}");
                        _notificarCambio(tarea);
                    }
                    else
                    {
                        _logger.LogError($"[Queue] Error al agregar la tarea de baja prioridad: {result.Message}");
                    }
                });

                response.Successful = true;
                response.Message = "Tarea agregada a la cola de procesamiento";
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
                var tarea = AppTaskFactory.CreateCustomTask(description, dueDate, additionalData);

                if (!_validarTarea(tarea))
                {
                    response.Successful = false;
                    response.Message = "La tarea no es válida";
                    return response;
                }

                _taskQueue.Enqueue(async serviceProvider =>
                {
                    var scopedProcess = serviceProvider.GetRequiredService<ICommonsProcess<Tareas>>();
                    var result = await scopedProcess.AddAsync(tarea);

                    if (result.IsSuccess)
                    {
                        _logger.LogInformation($"[Queue] Tarea personalizada creada con éxito: {tarea?.Id}");
                        _notificarCambio(tarea);
                    }
                    else
                    {
                        _logger.LogError($"[Queue] Error al agregar la tarea personalizada: {result.Message}");
                    }
                });

                response.Successful = true;
                response.Message = "Tarea agregada a la cola de procesamiento";
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

        /// <summary>
        /// Nuevo método que calcula el porcentaje de tareas completadas usando memorización.
        /// </summary>
        /// <returns>Porcentaje de tareas completadas</returns>
        public async Task<Response<double>> GetTaskCompletionPercentageAsync()
        {
            var response = new Response<double>();
            try
            {
                // Obtiene todas las tareas
                var tasks = await _commonsProcess.GetAllAsync();
                if (tasks == null || !tasks.Any())
                {
                    response.Successful = false;
                    response.Message = "No se encontraron tareas";
                    return response;
                }

                // Usa la función memorizada para calcular el porcentaje
                double completionRate = TaskCalculations.CalculateCompletionRate(tasks);
                response.Successful = true;
                response.SingleData = completionRate;
                response.Message = "Porcentaje de tareas completadas calculado correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al calcular el porcentaje de tareas completadas: {ex.Message}");
                response.Errors.Add(ex.Message);
            }
            return response;
        }
    }
}
