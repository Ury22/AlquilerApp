# AlquilerApp

Aplicación ASP.NET Core MVC para gestionar alquileres.

## Requisitos
- **.NET SDK:** 8.0 o superior (verifica con `dotnet --info`).
- **IDE (opcional):** Visual Studio 2022/2023 o Visual Studio Code.

## Ejecutar en desarrollo (PowerShell)
1. Abrir PowerShell en la carpeta del proyecto (la raíz donde está `AlquilerApp.csproj`).
2. Restaurar paquetes y compilar:
```powershell
dotnet restore
dotnet build
```
3. Ejecutar la aplicación:
```powershell
dotnet run
```
4. Abrir el navegador en `http://localhost:5000` o la URL que muestre la salida de `dotnet run`.

Notas sobre el entorno:
- Para forzar el entorno de ASP.NET Core a `Development` en PowerShell:
```powershell
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet run
```

## Ejecutar desde Visual Studio
- Abre la solución `AlquilerApp.sln` en Visual Studio.
- Selecciona el perfil de ejecución (IIS Express o el proyecto) y presiona F5 para depurar.

## Ejecutar con hot-reload (desarrollo rápido)
```powershell
dotnet watch run
```

## Publicar y ejecutar (Release)
1. Publicar en carpeta `publish`:
```powershell
dotnet publish -c Release -o publish
```
2. Ejecutar el binario publicado (Windows):
```powershell
Set-Location publish
./AlquilerApp.exe
```

## Base de datos y configuración
- Revisa `appsettings.json` y `appsettings.Development.json` para la cadena de conexión (`ConnectionStrings`).
- Si usas una base de datos local, actualiza la cadena en `appsettings.Development.json` o establece la variable de entorno `ConnectionStrings__DefaultConnection`.

## Consejos y problemas comunes
- Si ves errores relacionados con dependencias nativas en `bin/` u `obj/`, ejecuta `dotnet clean` y vuelve a compilar.
- Si `dotnet run` usa un puerto diferente, verifica la URL en la salida de la consola.

## Subir a GitHub (resumen rápido)
1. Crea un repositorio en GitHub.
2. Añade el remoto y sube:
```powershell
git remote add origin https://github.com/USUARIO/REPO.git
git branch -M main
git push -u origin main
```

Si quieres, puedo crear el repositorio remoto por ti (necesitaré un token personal con permisos para crear repositorios o autorización para usar tu cuenta).

---

Archivos importantes
- `AlquilerApp.csproj` — proyecto principal
- `Controllers/` — controladores MVC
- `Views/` — vistas Razor
- `Data/ApplicationDbContext.cs` — contexto de EF Core
# AlquilerApp

Aplicación de ejemplo para gestionar alquileres (ASP.NET Core / MVC).

Estado: Repositorio local inicializado con `.gitignore` y `README.md`.

Requisitos
- .NET SDK 8.0 o superior
- Visual Studio 2022/2023 o VS Code (opcional)

Cómo compilar y ejecutar (línea de comandos)
```powershell
dotnet restore
dotnet build
dotnet run
```

Instrucciones rápidas para publicar en GitHub
1. Crea un repositorio vacío en GitHub (sin README).
2. Desde la raíz del proyecto ejecuta:
```powershell
git remote add origin https://github.com/USUARIO/REPO.git
git branch -M main
git push -u origin main
```

Notas
- Se ha añadido un `.gitignore` apropiado para proyectos .NET.
- Si quieres que yo cree también el repositorio remoto en GitHub, indícame el nombre y si debe ser público o privado (necesitaré acceso/token).

Archivos importantes
- `AlquilerApp.csproj` — proyecto principal
- `Controllers/` — controladores MVC
- `Views/` — vistas Razor
- `Data/ApplicationDbContext.cs` — contexto de EF Core
