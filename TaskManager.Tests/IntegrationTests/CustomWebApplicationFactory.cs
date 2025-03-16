using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using InfrastructureLayer;
using Microsoft.VisualStudio.TestPlatform.TestHost;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Eliminar el servicio de DbContext si ya existe
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TaskManagerContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Agregar un contexto de base de datos en memoria para pruebas
            services.AddDbContext<TaskManagerContext>(options =>
            {
                options.UseInMemoryDatabase("TestDB");
            });

            // Crear un ámbito para inicializar datos de prueba
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TaskManagerContext>();
                context.Database.EnsureCreated();
                SeedTestData(context);  // Agregar datos de prueba
            }
        });
    }

    private void SeedTestData(TaskManagerContext context)
    {
        // Agregar un usuario de prueba
        context.Users.Add(new DomainLayer.Models.User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            Email = "admin@test.com",
            Roles = new List<string> { "Admin" },
            IsVerified = true
        });

        // Agregar una tarea de prueba
        context.Tareas.Add(new DomainLayer.Models.Tareas
        {
            Id = 1,
            Description = "Test Task",
            Status = "Pending",
            UserId = 1
        });

        context.SaveChanges();
    }
}
