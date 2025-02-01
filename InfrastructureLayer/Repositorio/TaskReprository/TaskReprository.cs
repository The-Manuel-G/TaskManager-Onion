using DomainLayer.Models;
using InfrastructureLayer.Repositorio.Commons;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.Repositorio.TaskReprository
{
    public class TaskReprository : ICommonsProcess<Tareas>
    {
        private readonly TaskManagerContext _context;

        public TaskReprository(TaskManagerContext taskManagerContext)
        {
            _context = taskManagerContext;
        }
        public async Task<IEnumerable<Tareas>> GetAllAsync()
            => await _context.Tarea.ToListAsync();

        public async Task<Tareas> GetIdAsync(int id)
             => await _context.Tarea.FirstOrDefaultAsync(x=>x.Id ==id);

        public async Task<(bool IsSuccess, string Message)> AddAsync(Tareas entry)
        {
            try
            {
                var exists =  _context.Tarea.Any(x => x.Description == entry.Description);

                if (exists)
                {
                    return (false, "Ya existe una tarea con ese nombre...");
                }
                await _context.Tarea.AddAsync(entry);
                await _context.SaveChangesAsync();
                return (true,"La tarea se guardo correctamente...");
            }
            catch (Exception)
            {

                return (false, "No se pudo guardar la tarea...");
            }
        }
         public async Task<(bool IsSuccess, string Message)> UpdateAsync(Tareas entry)
        {
            try
            {
                 _context.Tarea.Update(entry);
                await _context.SaveChangesAsync();
                return (true, "La tarea se actualizo correctamente...");
            }
            catch (Exception)
            {

                return (false, "No se pudo actualizar la tarea...");
            }
        }
        public async Task<(bool IsSuccess, string Message)> DeleteAsync(int id)
        {
            try
            {
                var tarea = await _context.Tarea.FindAsync(id);
                if (tarea != null)
                {
                    _context.Tarea.Remove(tarea);
                    await _context.SaveChangesAsync();
                    return (true, "La tarea se eliminó correctamente...");
                }
                else
                {
                    return (false, "No se encontró la tarea...");
                }

            }
            catch (Exception)
            {

                return (false, "No se pudo eliminar la tarea...");
            }
          
        }

       
    }
}
