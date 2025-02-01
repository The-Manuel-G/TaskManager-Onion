using DomainLayer.Models;

namespace DomainLayer.Delegates
{
    public delegate bool ValidarTareaDelegate(Tareas tarea);
    public delegate void NotificarCambioDelegate(Tareas tarea);
}