using ApplicationLayer.Services.TaskServices;
using DomainLayer;
using DomainLayer.Delegates;
using DomainLayer.Models;
using InfrastructureLayer;
using InfrastructureLayer.Repositorio.Commons;
using InfrastructureLayer.Repositorio.TaskReprository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<TaskManagerContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("TaskManagerDB"));
});

// Register the delegates
builder.Services.AddScoped<ValidarTareaDelegate>(_ => ValidacionesTarea.ValidarDescripcionYFecha);
builder.Services.AddScoped<NotificarCambioDelegate>(_ => (Tareas tarea) =>
{
    // Simple notification example (console log).
    // You could replace this with a more sophisticated logging or notification system.
    Console.WriteLine($"[EVENT] Cambios en Tarea (ID: {tarea.Id}).");
});

// Register repository and TaskService
builder.Services.AddScoped<ICommonsProcess<Tareas>, TaskReprository>();
builder.Services.AddScoped<TaskService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();