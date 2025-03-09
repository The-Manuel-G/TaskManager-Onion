using DomainLayer.Models;
using DomainLayer.Delegates;
using InfrastructureLayer.Repositorio.Commons;
using InfrastructureLayer.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfrastructureLayer.Repositorio.TaskReprository
{
    public class TaskRepository : ICommonsProcess<Tareas>
    {
        private readonly TaskManagerContext _context;
        private readonly ValidarTareaDelegate _validarTarea;
        private readonly NotificarCambioDelegate _notificarCambio;
        private readonly IHubContext<TaskHub> _hubContext;
        private readonly ILogger<TaskRepository> _logger;

        public TaskRepository(
            TaskManagerContext taskManagerContext,
            ValidarTareaDelegate validarTarea,
            NotificarCambioDelegate notificarCambio,
            IHubContext<TaskHub> hubContext,
            ILogger<TaskRepository> logger)
        {
            _context = taskManagerContext ?? throw new ArgumentNullException(nameof(taskManagerContext));
            _validarTarea = validarTarea ?? throw new ArgumentNullException(nameof(validarTarea));
            _notificarCambio = notificarCambio ?? throw new ArgumentNullException(nameof(notificarCambio));
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Tareas>> GetAllAsync()
        {
            return await _context.Tareas.ToListAsync();
        }


        public async Task<bool> ExistsAsync(int id) // ✅ Implementado
        {
            return await _context.Tareas.AnyAsync(x => x.Id == id);
        }

        public async Task SaveChangesAsync() // ✅ Implementado
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Tareas> GetIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("ID inválido", nameof(id));

            return await _context.Tareas.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<(bool IsSuccess, string Message)> AddAsync(Tareas entry)
        {
            if (entry == null) return (false, "La tarea no puede ser nula.");

            try
            {
                if (_validarTarea != null && !_validarTarea(entry))
                {
                    return (false, "La tarea no pasó la validación.");
                }

                // ✅ Verificar si el usuario existe antes de asignarlo a la tarea
                var userExists = await _context.Users.AnyAsync(u => u.Id == entry.UserId);
                if (!userExists)
                {
                    return (false, "El usuario asignado a la tarea no existe en la base de datos.");
                }

                bool exists = await _context.Tareas.AnyAsync(x => x.Description == entry.Description);
                if (exists)
                {
                    return (false, "Ya existe una tarea con ese nombre...");
                }

                await _context.Tareas.AddAsync(entry);
                await _context.SaveChangesAsync();

                _notificarCambio?.Invoke(entry);

                var message = $"Nueva tarea creada: {entry.Description}";
                await _hubContext.Clients.All.SendAsync("ReceiveTaskNotification", message);

                _logger.LogInformation($"[Servidor] {message}");
                Console.WriteLine($"[SignalR] {message}");

                return (true, "La tarea se guardó correctamente...");
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError($"Error de base de datos al guardar la tarea: {dbEx.InnerException?.Message ?? dbEx.Message}");
                return (false, $"Error al guardar la tarea: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error inesperado al guardar la tarea: {ex.Message}");
                return (false, "Error inesperado al guardar la tarea.");
            }
        }

        public async Task<(bool IsSuccess, string Message)> UpdateAsync(Tareas entry)
        {
            if (entry == null) return (false, "La tarea no puede ser nula.");

            try
            {
                if (_validarTarea != null && !_validarTarea(entry))
                {
                    return (false, "La tarea no pasó la validación.");
                }

                var existingTask = await _context.Tareas.FindAsync(entry.Id);
                if (existingTask == null)
                {
                    return (false, "No se encontró la tarea a actualizar.");
                }

                existingTask.Description = entry.Description;
                existingTask.DueDate = entry.DueDate;
                existingTask.Status = entry.Status;
                existingTask.AdditionalData = entry.AdditionalData;

                await _context.SaveChangesAsync();

                _notificarCambio?.Invoke(existingTask);

                var message = $"Tarea actualizada: {existingTask.Description}";
                await _hubContext.Clients.All.SendAsync("ReceiveTaskNotification", message);

                _logger.LogInformation($"[Servidor] {message}");
                Console.WriteLine($"[SignalR] {message}");

                return (true, "La tarea se actualizó correctamente...");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error inesperado al actualizar la tarea: {ex.Message}");
                return (false, "Error inesperado al actualizar la tarea.");
            }
        }

        public async Task<(bool IsSuccess, string Message)> DeleteAsync(int id)
        {
            if (id <= 0) return (false, "ID inválido.");

            try
            {
                var tarea = await _context.Tareas.FindAsync(id);
                if (tarea == null)
                {
                    return (false, "No se encontró la tarea...");
                }

                _context.Tareas.Remove(tarea);
                await _context.SaveChangesAsync();

                _notificarCambio?.Invoke(tarea);

                var message = $"Tarea eliminada: {tarea.Description}";
                await _hubContext.Clients.All.SendAsync("ReceiveTaskNotification", message);

                _logger.LogInformation($"[Servidor] {message}");
                Console.WriteLine($"[SignalR] {message}");

                return (true, "La tarea se eliminó correctamente...");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error inesperado al eliminar la tarea: {ex.Message}");
                return (false, "Error inesperado al eliminar la tarea.");
            }
        }
    }
}
