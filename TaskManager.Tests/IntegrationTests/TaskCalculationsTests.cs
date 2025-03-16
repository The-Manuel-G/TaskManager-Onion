using Xunit;
using ApplicationLayer.Optimizations;
using DomainLayer.Models;
using ApplicationLayer.Optimizations;
using System.Collections.Generic;
using FluentAssertions;

public class TaskCalculationsTestss
{
    [Fact]
    public void CalculateCompletionRate_Returns_Correct_Percentage()
    {
        // ✅ Prueba: Calcula el porcentaje de tareas completadas correctamente

        var tasks = new List<Tareas>
        {
            new Tareas { Status = "Completed" },
            new Tareas { Status = "Pending" },
            new Tareas { Status = "Completed" }
        };

        double result = TaskCalculations.CalculateCompletionRate(tasks);

        result.Should().BeApproximately(66.67, 2);
    }

    [Fact]
    public void CalculateCompletionRate_Returns_Zero_If_No_Tasks()
    {
        // ✅ Prueba: Si no hay tareas, el porcentaje completado debe ser 0%

        var tasks = new List<Tareas>();

        double result = TaskCalculations.CalculateCompletionRate(tasks);

        result.Should().Be(0);
    }
}
