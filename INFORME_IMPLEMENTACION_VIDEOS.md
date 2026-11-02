# Informe de implementación de los videos

Fecha: 1 de noviembre de 2026.

## Alcance y análisis

Analicé el audio y el contenido visible de estos videos del profesor:

| Video | Duración aproximada | Contenido identificado |
|---|---:|---|
| `AuthService.mp4` | 54:39 | Inyección de dependencias y flujo de registro, confirmación de correo, inicio y renovación de sesión, cambio y recuperación de contraseña. Las referencias aproximadas van de 00:00 a 54:30. |
| `RolePermissionService.mp4` | 50:13 | Asignación y retiro de permisos, gestión de roles y permisos, consultas paginadas y cálculo de permisos efectivos. Las referencias aproximadas van de 00:00 a 50:00. |
| `ViewComponetrr y Migraciones.mp4` | 14:40 | `NavigationViewComponent`, carga de permisos efectivos, registro de `AppDBContext`, creación de la migración inicial y actualización de la base demostrativa. Las referencias aproximadas van de 00:00 a 14:30. |

Los ejemplos del profesor usan el proyecto `ProyectoAuthJwt`, con nombres de entidades y DTOs que no coinciden en todos los casos con `App_Proyecto`. Adapté las operaciones a las entidades, contratos y nombres que ya existen aquí. El video termina con algunas diferencias respecto a los contratos locales; la adaptación de `ResendConfirmationEmailAsync` y la clase concreta `TokenService` se completó siguiendo la interfaz actual y el flujo que muestran las clases. En el video de configuración aparecen una cadena de conexión local y una clave JWT de demostración; no copié esos valores al proyecto.

## Implementación

### Autenticación

- Creé `Services/AuthService.cs` con cambio y confirmación de correo, registro como rol `Usuario`, inicio de sesión, rotación de refresh token, restablecimiento de contraseña y reenvío del correo de confirmación.
- El registro almacena la contraseña con BCrypt, guarda un token de confirmación y manda el enlace por el `IEmailService` existente. El enlace usa `Application:BaseUrl` si está configurado; por defecto apunta al perfil HTTPS local `https://localhost:7249`.
- La renovación valida el refresh token y su vencimiento, emite tokens nuevos y actualiza su expiración a siete días. Los enlaces para restablecer contraseña expiran en una hora.
- Creé `Services/TokenService.cs`, necesario para completar las llamadas de `AuthService` al contrato existente `ITokenService`. Emite JWT con identidad, rol y permisos efectivos, además de refresh y reset tokens criptográficamente aleatorios.
- Se conserva el nombre de contrato existente `ConfirmEmialAsync` por compatibilidad con `IAuthService`.

### Roles, permisos y navegación

- Creé `Services/IRolePermissionService.cs` y `Services/RolePermissionService.cs`. Implementé la asignación y retiro de permisos a roles y usuarios, creación y actualización de roles y permisos, borrado protegido de permisos asignados directamente, consultas de listas, consulta individual, paginación y actualización del rol de usuario.
- Creé `Extensions/EffectivePermission.cs`: reúne permisos del rol y aplica las excepciones directas `IsGranted` del usuario. Una denegación directa quita el permiso heredado del rol.
- Creé `ViewComponents/NavigationViewComponent.cs` y su vista `Views/Shared/Components/Navigation/Default.cshtml`. El componente usa los permisos del token y, cuando hay un usuario autenticado, vuelve a cargar los permisos efectivos desde el repositorio. La vista los presenta en el menú y el layout invoca el componente.

### EF Core y compatibilidad del modelo

- Registré el contexto SQL Server, AutoMapper, repositorios y servicios implementados en `Program.cs`. Las secciones de configuración se leen desde la configuración de ASP.NET Core; no agregué claves ni credenciales al repositorio.
- Creé la migración EF Core `InitialCreate` y el snapshot del modelo en `Migrations/`.
- Seguí la indicación de clase y agregué `**/Migrations/` a `.gitignore`: los archivos quedan en el proyecto local, pero Git los ignora y no se subirían al repositorio.
- Añadí a `AppDBContext` la clave compuesta de `UsuarioPermission` (`UsuarioId`, `PermissionId`), necesaria para que EF Core pueda modelar y guardar las excepciones directas.
- Alineé `IRoleRepository` y `IUsuarioRepository` con los identificadores `int` de sus modelos, retiré las implementaciones que lanzaban `NotImplementedException` para esos contratos y completé `PermissionRepository.IsAssignedToUsuarioAsync`.
- Corregí `UsuarioRepository.UserWithPermission` para cargar por separado los permisos del rol y las excepciones directas del usuario. Así el componente y el DTO reciben la lista completa, incluidos permisos directos que no pertenezcan al rol.

## Archivos creados

- `Services/AuthService.cs`
- `Services/TokenService.cs`
- `Services/IRolePermissionService.cs`
- `Services/RolePermissionService.cs`
- `Extensions/EffectivePermission.cs`
- `ViewComponents/NavigationViewComponent.cs`
- `Views/Shared/Components/Navigation/Default.cshtml`
- `Migrations/20261102010321_InitialCreate.cs`
- `Migrations/20261102010321_InitialCreate.Designer.cs`
- `Migrations/AppDBContextModelSnapshot.cs`

## Archivos modificados

- `Program.cs`
- `Data/AppDBContext.cs`
- `Repositories/IRoleRepository.cs`
- `Repositories/RolRepository.cs`
- `Repositories/IUsuarioRepository.cs`
- `Repositories/UsuarioRepository.cs`
- `Repositories/PermissionRepository.cs`
- `Views/Shared/_Layout.cshtml`
- `.gitignore`

Este informe también se añadió como `INFORME_IMPLEMENTACION_VIDEOS.md`.

## Validaciones

- `dotnet restore`: correcto.
- `dotnet build --no-restore`: correcto, 0 errores y 22 advertencias de nulabilidad en componentes existentes (`DbSeeder`, modelos y repositorio académico, entre otros).
- `dotnet-ef migrations add InitialCreate`: correcto. Se usó una herramienta local en `work/`; no se instaló una herramienta global.
- El equipo solo tiene instalado el runtime .NET 10; para ejecutar la herramienta EF 9 y la comprobación HTTP usé `DOTNET_ROLL_FORWARD=Major` en el proceso de prueba. El proyecto conserva `net9.0`.
- Prueba HTTP local sin conexión a base de datos: `/` y `/Home/Privacy` respondieron 200. El servidor de prueba se detuvo al terminar.
- `git diff --check`: sin errores.
- No hay proyecto ni archivos de pruebas automatizadas en el repositorio.
- No ejecuté `Update-Database`, `DbSeeder` ni operaciones contra SQL Server. El proyecto no contiene una cadena de conexión configurada y no se proporcionaron credenciales SMTP o JWT.

## Configuración y pendientes

Para usar los servicios en ejecución se deben proporcionar valores seguros mediante User Secrets o variables de entorno: `ConnectionStrings:DefaultConnection`, `Jwt:Key` (mínimo 32 bytes), `Jwt:Issuer`, `Jwt:Audience`, y los campos de `Smtp`. `Application:BaseUrl` permite definir el host público para los enlaces de correo. No dejé valores secretos en `appsettings.json`.

Los videos implementan las capas de servicio y el componente, pero no muestran controladores de autenticación ni páginas para registro, confirmación, login y recuperación. Por tanto, esos métodos están registrados en DI, pero todavía no se exponen como rutas HTTP. Tampoco se añadió el middleware JWT al pipeline, ya que el video no muestra esa configuración y el proyecto no tiene una clave JWT configurada. El componente puede consumir claims cuando ese flujo se integre.

La migración se generó, pero debe revisarse contra la base de datos real antes de aplicarla. En particular, hay que comprobar si existen tablas o un historial de migraciones de ejecuciones previas; una migración inicial puede fallar si el esquema ya existe. No se ejecutó la migración ni se alteraron datos.

Permanecen fuera de este alcance el servicio de estudiantes, controladores y vistas CRUD que no aparecen en estos videos, y otros pendientes históricos del informe de alineación. No se ejecutó el seeder.

## Git y respaldo

- Rama activa: `feature/clases-profesor-videos`, creada desde el `master` que estaba limpio en el commit `77bc89e`.
- Respaldo recuperable previo: `C:\Users\Anthony\source\repos\App_Project_Kelmer-backup-before-video-implementation.bundle`.
- No hice commit, merge ni push.
