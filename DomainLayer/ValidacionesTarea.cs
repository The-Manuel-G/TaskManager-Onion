using System;
using DomainLayer.Models;

namespace DomainLayer
{
    public static class ValidacionesTarea
    {
       
        public static bool ValidarDescripcionYFecha(Tareas tarea)
        {
            return !string.IsNullOrEmpty(tarea.Description)
                   && tarea.DueDate > DateTime.Now;
        }
    }
}