using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace InfrastructureLayer
{
    public class TaskManagerContext : DbContext
    {
        public TaskManagerContext(DbContextOptions options) :
            base(options)
        {

        }
        public DbSet<Tareas> Tarea { get; set; }


        public DbSet<RefreshToken> RefreshTokens { get; set; } // si usas refresh tokens

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Convertir la lista de roles a JSON
            modelBuilder.Entity<User>()
                .Property(u => u.Roles)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)
                );

            base.OnModelCreating(modelBuilder);







        }
    }
}