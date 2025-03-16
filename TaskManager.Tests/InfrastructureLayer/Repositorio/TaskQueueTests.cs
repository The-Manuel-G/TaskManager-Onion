using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using InfrastructureLayer.Repositorio;

public class TaskQueueTests
{
    [Fact]
    public async Task Enqueue_ShouldProcessTask()
    {
        // Arrange: use a real service provider
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var taskQueue = new TaskQueue(serviceProvider);
        bool taskExecuted = false;

        // Act: enqueue a task that sets taskExecuted to true
        taskQueue.Enqueue(async sp =>
        {
            taskExecuted = true;
            await Task.CompletedTask;
        });

        // Give some time for the queued task to be processed
        await Task.Delay(100);

        // Assert: verify that the task was executed
        Assert.True(taskExecuted);
    }

    [Fact]
    public async Task DequeueAsync_ShouldProcessSingleTask()
    {
        // Arrange: use a real service provider
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var taskQueue = new TaskQueue(serviceProvider);
        bool taskExecuted = false;

        // Enqueue a task that sets taskExecuted to true
        taskQueue.Enqueue(async sp =>
        {
            taskExecuted = true;
            await Task.CompletedTask;
        });

        // Act: invoke DequeueAsync to process one queued task manually
        await taskQueue.DequeueAsync();

        // Assert: verify that the task was executed
        Assert.True(taskExecuted);
    }
}