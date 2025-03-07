using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.Json;

namespace InfrastructureLayer
{
    public class TaskManagerContext : DbContext
    {
        // Constructor principal con opciones de configuración
        public TaskManagerContext(DbContextOptions<TaskManagerContext> options) : base(options)
        {
        }

        // Constructor vacío necesario para ejecutar migraciones
        public TaskManagerContext() { }

        public DbSet<User> Users { get; set; }
        public DbSet<Tareas> Tareas { get; set; }  // Corregido (antes estaba como `Tarea`)
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Convertir la lista de roles a JSON
            modelBuilder.Entity<User>()
                .Property(u => u.Roles)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)
                );

            // Relación User - Tareas (Uno a Muchos)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Tareas)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Opcional: elimina las tareas si se borra un usuario

            base.OnModelCreating(modelBuilder);
        }
    }
}
