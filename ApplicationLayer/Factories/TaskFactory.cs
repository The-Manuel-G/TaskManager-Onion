using DomainLayer.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace ApplicationLayer.Factories
{
    public static class TaskFactory
    {
        public static Tareas CreateHighPriorityTask(string description)
        {
            return new Tareas
            {
                Description = description,
                DueDate = DateTime.Now.AddDays(1),
                Status = "Pending",
                AdditionalData = "High Priority"
            };
        }

        public static Tareas CreateLowPriorityTask(string description)
        {
            return new Tareas
            {
                Description = description,
                DueDate = DateTime.Now.AddDays(7),
                Status = "Pending",
                AdditionalData = "Low Priority"
            };
        }

        public static Tareas CreateCustomTask(string description, DateTime dueDate, string additionalData)
        {
            return new Tareas
            {
                Description = description,
                DueDate = dueDate,
                Status = "Pending",
                AdditionalData = additionalData
            };
        }
    }
}
