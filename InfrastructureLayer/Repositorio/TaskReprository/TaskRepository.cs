using DomainLayer.Models;
using DomainLayer.Delegates;
using InfrastructureLayer.Repositorio.Commons;
using Microsoft.EntityFrameworkCore;
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

        public TaskRepository(
            TaskManagerContext taskManagerContext,
            ValidarTareaDelegate validarTarea,
            NotificarCambioDelegate notificarCambio)
        {
            _context = taskManagerContext ?? throw new ArgumentNullException(nameof(taskManagerContext));
            _validarTarea = validarTarea ?? throw new ArgumentNullException(nameof(validarTarea));
            _notificarCambio = notificarCambio ?? throw new ArgumentNullException(nameof(notificarCambio));
        }

        public async Task<IEnumerable<Tareas>> GetAllAsync()
        {
            return await _context.Tareas.ToListAsync(); // 🔹 Se corrigió de `_context.Tarea` a `_context.Tareas`
        }

        public async Task<Tareas> GetIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("ID inválido", nameof(id));

            return await _context.Tareas.FirstOrDefaultAsync(x => x.Id == id); // 🔹 Se corrigió `_context.Tarea`
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

                bool exists = await _context.Tareas.AnyAsync(x => x.Description == entry.Description);
                if (exists)
                {
                    return (false, "Ya existe una tarea con ese nombre...");
                }

                await _context.Tareas.AddAsync(entry);
                await _context.SaveChangesAsync();

                _notificarCambio?.Invoke(entry);

                return (true, "La tarea se guardó correctamente...");
            }
            catch (Exception ex)
            {
                return (false, $"Error al guardar la tarea: {ex.Message}");
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

                _context.Tareas.Update(entry);
                await _context.SaveChangesAsync();

                _notificarCambio?.Invoke(entry);

                return (true, "La tarea se actualizó correctamente...");
            }
            catch (Exception ex)
            {
                return (false, $"Error al actualizar la tarea: {ex.Message}");
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

                return (true, "La tarea se eliminó correctamente...");
            }
            catch (Exception ex)
            {
                return (false, $"Error al eliminar la tarea: {ex.Message}");
            }
        }
    }
}
