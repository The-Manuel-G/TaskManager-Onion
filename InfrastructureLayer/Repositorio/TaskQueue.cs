using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using DomainLayer;
using Microsoft.Extensions.DependencyInjection;

namespace InfrastructureLayer.Repositorio
{
    public class TaskQueue : ITaskQueue
    {
        private readonly ConcurrentQueue<Func<IServiceProvider, Task>> _taskQueue = new();
        private readonly object _lock = new();
        private bool _isProcessing = false;
        private readonly IServiceProvider _serviceProvider;

        public TaskQueue(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Enqueue(Func<IServiceProvider, Task> task)
        {
            lock (_lock)
            {
                _taskQueue.Enqueue(task);
                if (!_isProcessing)
                {
                    _isProcessing = true;
                    _ = ProcessQueueAsync();
                }
            }
        }

        private async Task ProcessQueueAsync()
        {
            while (true)
            {
                Func<IServiceProvider, Task> task;
                lock (_lock)
                {
                    if (!_taskQueue.TryDequeue(out task))
                    {
                        _isProcessing = false;
                        return;
                    }
                }

                try
                {
                    // Crear un nuevo scope para cada tarea encolada
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        await task(scope.ServiceProvider);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error en la tarea: {ex.Message}");
                }
            }
        }

        public async Task DequeueAsync()
        {
            Func<IServiceProvider, Task> task;
            bool taskExists;
            lock (_lock)
            {
                taskExists = _taskQueue.TryDequeue(out task);
            }

            if (taskExists)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        await task(scope.ServiceProvider);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error en la tarea: {ex.Message}");
                }
            }
        }
    }
}
