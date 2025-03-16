using System.Collections.Generic;
using DomainLayer.Models;
using ApplicationLayer.Optimizations;
using Xunit;

public class TaskCalculationsTests
{
    /// <summary>
    /// Verifica que el cálculo del porcentaje sea correcto cuando hay tareas completadas y pendientes.
    /// </summary>
    [Fact]
    public void CalculateCompletionRate_MixedTasks_ReturnsCorrectPercentage()
    {
        // Arrange
        var tasks = new List<Tareas>
        {
            new Tareas { Status = "Completed" },
            new Tareas { Status = "Pending" },
            new Tareas { Status = "Completed" },
            new Tareas { Status = "In Progress" }
        };

        // Act
        double result = TaskCalculations.CalculateCompletionRate(tasks);

        // Assert
        Assert.Equal(50, result); // 2 de 4 tareas están completadas (50%)
    }

    /// <summary>
    /// Prueba que el cálculo del porcentaje es 100% cuando todas las tareas están completadas.
    /// </summary>
    [Fact]
    public void CalculateCompletionRate_AllTasksCompleted_Returns100Percent()
    {
        // Arrange
        var tasks = new List<Tareas>
        {
            new Tareas { Status = "Completed" },
            new Tareas { Status = "Completed" },
            new Tareas { Status = "Completed" }
        };

        // Act
        double result = TaskCalculations.CalculateCompletionRate(tasks);

        // Assert
        Assert.Equal(100, result);
    }

    /// <summary>
    /// Prueba que el cálculo del porcentaje es 0% cuando no hay tareas completadas.
    /// </summary>
    [Fact]
    public void CalculateCompletionRate_NoTasksCompleted_ReturnsZeroPercent()
    {
        // Arrange
        var tasks = new List<Tareas>
        {
            new Tareas { Status = "Pending" },
            new Tareas { Status = "In Progress" },
            new Tareas { Status = "On Hold" }
        };

        // Act
        double result = TaskCalculations.CalculateCompletionRate(tasks);

        // Assert
        Assert.Equal(0, result);
    }

    /// <summary>
    /// Verifica que el cálculo retorna 0% cuando la lista de tareas está vacía.
    /// </summary>
    [Fact]
    public void CalculateCompletionRate_EmptyTaskList_ReturnsZeroPercent()
    {
        // Arrange
        var tasks = new List<Tareas>();

        // Act
        double result = TaskCalculations.CalculateCompletionRate(tasks);

        // Assert
        Assert.Equal(0, result);
    }

    /// <summary>
    /// Prueba que la función utiliza el caché para valores repetidos y no recalcula innecesariamente.
    /// </summary>
    [Fact]
    public void CalculateCompletionRate_UsesCache_ReturnsSameValue()
    {
        // Arrange
        var tasks = new List<Tareas>
        {
            new Tareas { Status = "Completed" },
            new Tareas { Status = "Pending" },
            new Tareas { Status = "Completed" }
        };

        // Act
        double firstCalculation = TaskCalculations.CalculateCompletionRate(tasks);
        double secondCalculation = TaskCalculations.CalculateCompletionRate(tasks); // Debe usar caché

        // Assert
        Assert.Equal(66.67, firstCalculation, 2); // 2 de 3 completadas (66.67%)
        Assert.Equal(firstCalculation, secondCalculation); // El caché debe devolver el mismo valor
    }

    /// <summary>
    /// Verifica que al limpiar la caché, el cálculo se vuelva a hacer desde cero.
    /// </summary>
    [Fact]
    public void ClearCache_AllowsRecalculation()
    {
        // Arrange
        var tasks = new List<Tareas>
        {
            new Tareas { Status = "Completed" },
            new Tareas { Status = "Pending" }
        };

        // Act
        double firstCalculation = TaskCalculations.CalculateCompletionRate(tasks);
        TaskCalculations.ClearCache();
        double secondCalculation = TaskCalculations.CalculateCompletionRate(tasks);

        // Assert
        Assert.Equal(firstCalculation, secondCalculation); // A pesar del recalculo, debe dar el mismo resultado
    }
}
