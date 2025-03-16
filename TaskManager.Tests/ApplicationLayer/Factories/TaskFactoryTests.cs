using System;
using DomainLayer.Models;
using ApplicationLayer.Factories;
using Xunit;

public class TaskFactoryTests
{
    /// <summary>
    /// Prueba que se pueda crear una tarea de alta prioridad correctamente.
    /// Valida que las propiedades asignadas sean las esperadas.
    /// </summary>
    [Fact]
    public void CreateHighPriorityTask_ValidInput_ReturnsTaskWithCorrectProperties()
    {
        // Arrange
        string description = "Urgent task";
        int userId = 1;

        // Act
        var task = ApplicationLayer.Factories.TaskFactory.CreateHighPriorityTask(description, userId);

        // Assert
        Assert.NotNull(task);
        Assert.Equal(userId, task.UserId);
        Assert.Equal(description, task.Description);
        Assert.Equal("Pending", task.Status);
        Assert.Equal("High Priority", task.AdditionalData);
        Assert.True((task.DueDate - DateTime.Now).TotalDays <= 1.1); // Pequeño margen de error permitido
    }

    /// <summary>
    /// Prueba que se pueda crear una tarea de baja prioridad correctamente.
    /// Se verifica que los valores de la tarea sean los esperados.
    /// </summary>
    [Fact]
    public void CreateLowPriorityTask_ValidInput_ReturnsTaskWithCorrectProperties()
    {
        // Arrange
        string description = "Non-urgent task";
        int userId = 2;

        // Act
        var task = ApplicationLayer.Factories.TaskFactory.CreateLowPriorityTask(description, userId);

        // Assert
        Assert.NotNull(task);
        Assert.Equal(userId, task.UserId);
        Assert.Equal(description, task.Description);
        Assert.Equal("Pending", task.Status);
        Assert.Equal("Low Priority", task.AdditionalData);
        Assert.True((task.DueDate - DateTime.Now).TotalDays <= 7.1);
    }

    /// <summary>
    /// Prueba la creación de una tarea personalizada con una fecha y datos adicionales específicos.
    /// Se verifica que todas las propiedades estén correctamente asignadas.
    /// </summary>
    [Fact]
    public void CreateCustomTask_ValidInput_ReturnsTaskWithCorrectProperties()
    {
        // Arrange
        string description = "Custom task";
        int userId = 3;
        DateTime dueDate = DateTime.Now.AddDays(10);
        string additionalData = "Special instructions";

        // Act
        var task = ApplicationLayer.Factories.TaskFactory.CreateCustomTask(description, dueDate, additionalData, userId);

        // Assert
        Assert.NotNull(task);
        Assert.Equal(userId, task.UserId);
        Assert.Equal(description, task.Description);
        Assert.Equal("Pending", task.Status);
        Assert.Equal(additionalData, task.AdditionalData);
        Assert.Equal(dueDate.Date, task.DueDate.Date);
    }

    /// <summary>
    /// Verifica que la fábrica de tareas pueda manejar descripciones vacías sin generar errores.
    /// </summary>
    [Fact]
    public void CreateHighPriorityTask_EmptyDescription_StillCreatesValidTask()
    {
        // Arrange
        string description = "";
        int userId = 4;

        // Act
        var task = ApplicationLayer.Factories.TaskFactory.CreateHighPriorityTask(description, userId);

        // Assert
        Assert.NotNull(task);
        Assert.Equal(userId, task.UserId);
        Assert.Equal(description, task.Description);
        Assert.Equal("Pending", task.Status);
        Assert.Equal("High Priority", task.AdditionalData);
    }

    /// <summary>
    /// Prueba que se pueda asignar una fecha en el pasado sin que la fábrica falle.
    /// Se permite que las fechas pasadas sean creadas sin restricciones.
    /// </summary>
    [Fact]
    public void CreateCustomTask_InvalidDueDate_AllowsPastDates()
    {
        // Arrange
        string description = "Past task";
        int userId = 5;
        DateTime dueDate = DateTime.Now.AddDays(-5);
        string additionalData = "Expired task";

        // Act
        var task = ApplicationLayer.Factories.TaskFactory.CreateCustomTask(description, dueDate, additionalData, userId);

        // Assert
        Assert.NotNull(task);
        Assert.Equal(userId, task.UserId);
        Assert.Equal(description, task.Description);
        Assert.Equal("Pending", task.Status);
        Assert.Equal(additionalData, task.AdditionalData);
        Assert.Equal(dueDate.Date, task.DueDate.Date);
    }
}
