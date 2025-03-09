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
using InfrastructureLayer.Repositorio.UserRepository;
using InfrastructureLayer;
using ApplicationLayer.Services.AuthServices;
using ApplicationLayer.Services;
using Microsoft.OpenApi.Models;
using DomainLayer;
using InfrastructureLayer.SignalR;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar JWT de forma segura
var secretKey = builder.Configuration["JwtSettings:SecretKey"];
if (string.IsNullOrEmpty(secretKey))
{
    throw new Exception("JWT SecretKey is not configured. Check appsettings.json");
}

var key = Encoding.UTF8.GetBytes(secretKey);



builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // En producción, poner en true
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

// 2. Configurar DbContext y asegurar que está disponible para `dotnet ef`
builder.Services.AddDbContext<TaskManagerContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("TaskManagerDB");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new Exception("Database connection string is not configured.");
    }
    options.UseSqlServer(connectionString);
});

// 3. Registrar Delegates
builder.Services.AddScoped<ValidarTareaDelegate>(_ => ValidacionesTarea.ValidarDescripcionYFecha);
builder.Services.AddScoped<NotificarCambioDelegate>(_ => (Tareas tarea) =>
{
    Console.WriteLine($"[EVENT] Cambios en Tarea (ID: {tarea.Id}).");
});
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
// 4. Registrar Repositorios y Servicios

builder.Services.AddSingleton<ITaskQueue, TaskQueue>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICommonsProcess<Tareas>, TaskRepository>();
builder.Services.AddScoped<TaskService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>(); // ✅ Se movió para asegurar que esté correctamente registrado

// 5. Configurar CORS (Opcional pero recomendado)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// 6. Agregar controladores y configuración de Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Task Manager API",
        Version = "v1"
    });
});
builder.Services.AddSignalR();

// Construir la aplicación
var app = builder.Build();

// 7. Configurar Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHub<TaskHub>("/taskHub");

app.UseCors("AllowAll"); // Habilitar CORS

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
