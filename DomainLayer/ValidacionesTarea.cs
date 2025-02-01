using System;
using DomainLayer.Models;

namespace DomainLayer
{
    public static class ValidacionesTarea
    {
        /// <summary>
        /// Returns true if the Description is not empty
        /// and the DueDate is in the future.
        /// </summary>
        public static bool ValidarDescripcionYFecha(Tareas tarea)
        {
            return !string.IsNullOrEmpty(tarea.Description)
                   && tarea.DueDate > DateTime.Now;
        }
    }
}