# Informe de correcciones V1.1

Fecha: 1 de noviembre de 2026.

## Correcciones

- `ChangePasswordAsync` y `ResetPasswordAsync` ahora limpian `RefreshToken` y `RefreshTokenExpiryTime` al guardar un cambio de contraseña válido.
- `RefreshAccessTokenAsync` busca el token enviado contra el valor persistido y comprueba su fecha de expiración. Tras revocarlo, el token antiguo ya no coincide y no puede renovarse. La entidad mantiene un solo refresh token activo por usuario; los JWT de acceso ya emitidos conservan su validez hasta su expiración normal.
- `Program.cs` registra `AppDBContext` con SQL Server usando `ConnectionStrings:DefaultConnection`. La aplicación no contiene una cadena de conexión ni credenciales reales.
- `DbSeeder.SeedRolesAsync` crea `Admin` y `Usuario` solo si faltan. Las otras etapas omiten inserciones cuando sus tablas ya contienen datos; `SeedUsersAsync` requiere la tabla vacía y las seis variables de entorno de credenciales. No ejecuté el seeder; su método principal también llama a `MigrateAsync`.

## Migraciones

Existe localmente una migración `InitialCreate` y su snapshot, con tres archivos. EF Core confirmó: “No changes have been made to the model since the last migration”. No hay duplicados detectados.

Se conserva la regla académica `**/Migrations/` de `.gitignore`; los archivos locales no se incluirán en el commit. En otra computadora, si no hay carpeta de migraciones, los comandos son:

```powershell
dotnet tool install --global dotnet-ef --version 9.0.20
dotnet ef migrations add InitialCreate --output-dir Migrations
```

Después de verificar que la conexión apunte a una base de desarrollo autorizada, se puede aplicar la migración con:

```powershell
dotnet ef database update
```

No ejecuté ese último comando. Si las tablas ya existen en esa base, primero hay que revisar su esquema y `__EFMigrationsHistory`; `InitialCreate` no debe aplicarse a ciegas.

## SQL Server y secretos locales

El proyecto ya tiene `UserSecretsId`. Para una instancia SQL Server local con autenticación integrada, configura la cadena en User Secrets desde la carpeta del proyecto, reemplazando servidor y base según la computadora:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=App_Proyecto;Trusted_Connection=True;TrustServerCertificate=True;"
```

También se puede suministrar solo al proceso actual mediante la variable `ConnectionStrings__DefaultConnection`. Para SQL Authentication, guarda el usuario y contraseña en User Secrets o en un almacén seguro del sistema; no los agregues a `appsettings.json` ni al repositorio. La conexión real aún no está configurada y no comprobé conectividad con SQL Server.

`Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience`, `Smtp:*` y `Application:BaseUrl` también se leen de configuración segura. Las credenciales opcionales del seeder se suministran mediante `APP_PROYECTO_SEED_ADMIN_USERNAME`, `APP_PROYECTO_SEED_ADMIN_EMAIL`, `APP_PROYECTO_SEED_ADMIN_PASSWORD`, `APP_PROYECTO_SEED_USUARIO_USERNAME`, `APP_PROYECTO_SEED_USUARIO_EMAIL` y `APP_PROYECTO_SEED_USUARIO_PASSWORD`. No añadí valores para esas claves.

## Archivos

Modifiqué `Services/AuthService.cs`. Añadí este informe. El resto de implementaciones de los videos permanece como trabajo local pendiente de commit; no agregué controladores, pantallas ni middleware JWT reservado para clases futuras. Las migraciones siguen excluidas por instrucción académica.

## Verificaciones

- `dotnet restore`: correcto.
- `dotnet build --no-restore`: 0 errores. La compilación completa reportó las mismas 22 advertencias de nulabilidad de componentes existentes; ninguna corresponde a las líneas de revocación añadidas. La compilación incremental final terminó con 0 advertencias y 0 errores.
- Prueba aislada de `AuthService` con repositorio en memoria: cambio de contraseña revoca el refresh token; restablecimiento de contraseña revoca el refresh token y guarda la contraseña nueva. No se usaron SQL Server, SMTP ni credenciales reales.
- `dotnet-ef migrations has-pending-model-changes`: sin cambios pendientes.
- `git diff --check`: correcto.
- No hay proyectos de pruebas automatizadas en el repositorio. No ejecuté `Update-Database` ni `DbSeeder`.

## Git

- Rama: `feature/clases-profesor-videos`.
- Commit local: `fix: seguridad de autenticacion y preparacion academica V1.1`. Incluye las implementaciones anteriores verificadas y los cambios V1.1; la carpeta `Migrations/` sigue ignorada según la regla académica.
- Push y merge: no realizados.
