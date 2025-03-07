using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using ApplicationLayer.Services.TaskServices;
using DomainLayer.Delegates;
using DomainLayer.Models;

using InfrastructureLayer.Repositorio;
using InfrastructureLayer.Repositorio.Commons;
using InfrastructureLayer.Repositorio.TaskReprository;
using Microsoft.OpenApi.Models;
using ApplicationLayer.Services.Auth;
using InfrastructureLayer.Repositorio.UserRepository;
using DomainLayer;
using InfrastructureLayer;
using ApplicationLayer.Services.Auth;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar JWT
var secretKey = builder.Configuration["JwtSettings:SecretKey"];
// Sugerencia: usar directamente el indexador en vez de .GetSection(...).Value

var key = Encoding.UTF8.GetBytes(secretKey!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // En entornos de producción, se recomienda exigir HTTPS
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

// 2. Configurar DbContext
builder.Services.AddDbContext<TaskManagerContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("TaskManagerDB"));
});

// 3. Registrar Delegates
builder.Services.AddScoped<ValidarTareaDelegate>(_ => ValidacionesTarea.ValidarDescripcionYFecha);
builder.Services.AddScoped<NotificarCambioDelegate>(_ => (Tareas tarea) =>
{
    Console.WriteLine($"[EVENT] Cambios en Tarea (ID: {tarea.Id}).");
});

// 4. Registrar Repositorios y Servicios
builder.Services.AddSingleton<ITaskQueue, TaskQueue>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICommonsProcess<Tareas>, TaskRepository>();
builder.Services.AddScoped<TaskService>();

// 5. Agregar controladores y configuración de Swagger
builder.Services.AddControllers();

// Explorador de endpoints + Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Task Manager API",
        Version = "v1"
    });
    // Opcional: si deseas integrar JWT en SwaggerUI, puedes configurar aquí
});

// Construir la aplicación
var app = builder.Build();

// 6. Configurar Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirección a HTTPS
app.UseHttpsRedirection();

// Autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// Mapear controladores
app.MapControllers();

// Correr la aplicación
app.Run();
