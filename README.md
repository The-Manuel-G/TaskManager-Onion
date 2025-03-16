

# 🛠️ TaskManager - Configuración y Ejecución

Este documento explica cómo clonar, configurar y ejecutar el proyecto **TaskManager** utilizando **.NET 7+** y **Entity Framework Core** con Code First.

---

## 📌 **Requisitos Previos**
Antes de comenzar, asegúrate de tener instalado:

- [.NET SDK](https://dotnet.microsoft.com/download) (versión 7+)
- [SQL Server](https://www.microsoft.com/es-es/sql-server/sql-server-downloads) o **SQL Server Express**
- [Entity Framework Core Tools](https://docs.microsoft.com/en-us/ef/core/cli/dotnet)
- Un IDE como **Visual Studio** o **VS Code**

Para verificar que tienes `EF Core Tools` instalado, ejecuta:
```powershell
dotnet tool list -g
```
Si no está instalado, agrégalo con:
```powershell
dotnet tool install --global dotnet-ef
```

---

## 📌 **1️⃣ Clonar el Proyecto**
Ejecuta en la terminal:
```powershell
git clone https://github.com/The-Manuel-G/TaskManager-Onion.git
cd TaskManager
```
**NoTa**:Asegurate de reconstruir la solucion  cuando clones  el proyecto 
---

## 📌 **2️⃣ Configurar la Base de Datos**
Abre el archivo **`appsettings.json`** y configura la cadena de conexión:
```json
"ConnectionStrings": {
    "TaskManagerDB": "Server=(localdb)\\MSSQLLocalDB;Database=TaskManagerDB;Trusted_Connection=True;"
}
```
Si usas **SQL Server en Docker**, usa:
```json
"ConnectionStrings": {
    "TaskManagerDB": "Server=localhost,1433;Database=TaskManagerDB;User Id=sa;Password=TuPassword;"
}
```

---

## 📌 **3️⃣ Generar la Base de Datos**
Ejecuta los siguientes comandos en **Package Manager Console** (Visual Studio) o en la terminal:

```powershell
dotnet ef migrations add InitialMigration --project InfrastructureLayer --startup-project TaskManager
dotnet ef database update --project InfrastructureLayer --startup-project TaskManager
```
🔹 **Esto creará y aplicará la estructura de la base de datos.**

---

## 📌 **4️⃣ Ejecutar el Proyecto**
Para iniciar la API, usa:
```powershell
dotnet run --project TaskManager
```
Esto levantará el servidor y podrás acceder a la API.

---

## 📌 **5️⃣ Acceder a Swagger**
Después de ejecutar el proyecto, abre tu navegador y ve a:
```
http://localhost:5000/swagger/index.html
```
Aquí podrás probar los endpoints de la API.

---

## 📌 **Comandos Útiles**
| Acción | Comando |
|--------|---------|
| 📌 Crear una nueva migración | `dotnet ef migrations add NombreMigracion --project InfrastructureLayer --startup-project TaskManager` |
| 📌 Aplicar cambios a la base de datos | `dotnet ef database update --project InfrastructureLayer --startup-project TaskManager` |
| 📌 Ver migraciones existentes | `dotnet ef migrations list --project InfrastructureLayer --startup-project TaskManager` |
| 📌 Eliminar última migración | `dotnet ef migrations remove --project InfrastructureLayer --startup-project TaskManager` |

---

## 📌 **Solución a Errores Comunes**
### ❌ **Error: "No database provider has been configured for this DbContext"**
🔹 **Solución:** Asegúrate de que en `TaskManagerContext.cs` tengas:
```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    if (!optionsBuilder.IsConfigured)
    {
        optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=TaskManagerDB;Trusted_Connection=True;");
    }
}
```

### ❌ **Error: "Unable to create an object of type 'TaskManagerContext'"**
🔹 **Solución:** Crea una fábrica en `InfrastructureLayer/TaskManagerContextFactory.cs`:
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace InfrastructureLayer
{
    public class TaskManagerContextFactory : IDesignTimeDbContextFactory<TaskManagerContext>
    {
        public TaskManagerContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<TaskManagerContext>();
            var connectionString = configuration.GetConnectionString("TaskManagerDB");
            optionsBuilder.UseSqlServer(connectionString);

            return new TaskManagerContext(optionsBuilder.Options);
        }
    }
}
```

---


# 🛠 TaskManager - Pruebas Implementadas

Este documento describe las pruebas unitarias e integrales realizadas en el sistema **TaskManager** para validar su correcto funcionamiento.

---

## 📌 1. Tipos de Pruebas Realizadas

### ✅ **Pruebas Unitarias**
- **Objetivo:** Validar el funcionamiento de componentes individuales como servicios y repositorios.
- **Herramientas utilizadas:** `xUnit`, `Moq`, `FluentAssertions`, `EntityFrameworkCore.InMemory`.

### ✅ **Pruebas de Integración**
- **Objetivo:** Evaluar la interacción entre componentes, simulando escenarios reales.
- **Herramientas utilizadas:** `WebApplicationFactory`, `HttpClient`, `FluentAssertions`.

---

## 🔍 2. Cobertura de Pruebas

### 🏗 **Pruebas Unitarias**

#### 📝 **TaskServiceTests**
- **Validación de la gestión de tareas.**
- **Casos probados:**
  - ✔ Crear una tarea con usuario válido devuelve éxito.
  - ❌ Intentar crear una tarea con usuario inexistente debe fallar.
  - 🔍 Obtener una tarea por ID debe devolver los datos correctos.

#### 🔑 **AuthServiceTests**
- **Validación del proceso de autenticación.**
- **Casos probados:**
  - ✔ Autenticación con credenciales válidas devuelve un JWT.
  - ❌ Autenticación con credenciales incorrectas debe fallar.
  - 🔄 Refrescar un token válido genera un nuevo JWT.

#### 🔄 **RefreshTokenRepositoryTests**
- **Gestión de tokens de actualización en la base de datos.**
- **Casos probados:**
  - ✔ Un token agregado debe poder recuperarse por su clave.
  - ❌ Un token eliminado no debe estar disponible en la base de datos.

#### 🔢 **TaskCalculationsTests**
- **Validación del cálculo de porcentaje de tareas completadas.**
- **Casos probados:**
  - ✔ 50% de tareas completadas debe devolver `50.0`.
  - ✔ 0% de tareas completadas debe devolver `0.0`.

---

### 🔄 **Pruebas de Integración**

#### 🏛 **TaskManagerApiTests**
- **Validación de la API REST.**
- **Casos probados:**
  - ✔ Inicio de sesión exitoso devuelve un `JWT`.
  - ❌ Acceder a `/api/Tareas` sin autenticación devuelve `401 Unauthorized`.
  - ✔ Acceder a `/api/Tareas` con un `JWT` válido devuelve `200 OK`.
  - ❌ Intentar crear una tarea sin datos devuelve `400 Bad Request`.

#### 🔑 **AuthControllerTests**
- **Pruebas del flujo de autenticación desde la API.**
- **Casos probados:**
  - ✔ Login exitoso genera un token JWT.
  - ❌ Usuario incorrecto devuelve `401 Unauthorized`.

#### 👤 **UserControllerTests**
- **Validación de la gestión de usuarios desde la API.**
- **Casos probados:**
  - ✔ Registrar un usuario nuevo.
  - 🔄 Modificar información de un usuario existente.
  - 🗑️ Eliminar un usuario y verificar que no pueda autenticarse.

---

## 🏗 3. Cómo Ejecutar las Pruebas

### 🔹 **Ejecutar Pruebas Unitarias**
Para ejecutar las pruebas unitarias, usa el siguiente comando:

```sh
dotnet test --filter FullyQualifiedName~TaskServiceTests


## 📌 **Conclusión**
✅ **Ahora puedes clonar, configurar y ejecutar el proyecto fácilmente.**  
✅ **El proyecto está listo para usarse con EF Core Code First.**  
✅ **Puedes agregar migraciones y actualizar la base de datos con los comandos mencionados.**  

🚀 **¡Listo para empezar a desarrollar en TaskManager!** 😊



---



## 🚀 **¿Qué incluye este `README.md`?**
✅ **Instrucciones claras para instalar dependencias y configurar el entorno.**  
✅ **Pasos detallados para generar la base de datos y ejecutar el proyecto.**  
✅ **Comandos útiles de `dotnet ef` para administrar la base de datos.**  
✅ **Solución a errores comunes en EF Core Code First.**  

🔹 **Cualquiera que clone el proyecto podrá configurarlo sin problemas.**  
Si necesitas ajustes adicionales, dime. 😊🚀
