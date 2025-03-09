using DomainLayer.Models;
using System;

namespace ApplicationLayer.Factories
{
    public static class TaskFactory
    {
        public static Tareas CreateHighPriorityTask(string description, int userId)
        {
            return new Tareas
            {
                UserId = userId,  // Asegurar que la tarea tiene un usuario asignado
                Description = description,
                DueDate = DateTime.Now.AddDays(1),
                Status = "Pending",
                AdditionalData = "High Priority"
            };
        }

        public static Tareas CreateLowPriorityTask(string description, int userId)
        {
            return new Tareas
            {
                UserId = userId,  // Asegurar que la tarea tiene un usuario asignado
                Description = description,
                DueDate = DateTime.Now.AddDays(7),
                Status = "Pending",
                AdditionalData = "Low Priority"
            };
        }

        public static Tareas CreateCustomTask(string description, DateTime dueDate, string additionalData, int userId)
        {
            return new Tareas
            {
                UserId = userId,  // Asegurar que la tarea tiene un usuario asignado
                Description = description,
                DueDate = dueDate,
                Status = "Pending",
                AdditionalData = additionalData
            };
        }
    }
}
