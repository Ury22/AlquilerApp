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
