# INFORME DE ALINEACIÓN ACADÉMICA — App_Proyecto

Fecha: 28 de octubre de 2026.

## Resultado y alcance

Se corrigió directamente `E:\Kelmer\Ing.Software II\App_Proyecto` usando App_Proyecto.zip como base y JoseProyecto.zip como referencia autorizada. El proyecto conserva App_Proyecto, sus namespaces, su solución y `net9.0`. El objetivo es reproducir el avance de clases, incluidos sus componentes incompletos, sin desarrollar módulos nuevos.

La comparación final del inventario, excluyendo .git, .vs, bin, obj y bibliotecas web de terceros, encontró todos los archivos de referencia con sus equivalentes de App_Proyecto, sin faltantes. Antes de añadir este informe, no había archivos adicionales en ese inventario. El único documento adicional final es INFORME_ALINEACION_ACADEMICA.md, solicitado por el usuario.

Se verificaron 58 archivos de referencia: su contenido coincide tras normalizar saltos de línea y sustituir JoseProyecto por App_Proyecto. La única excepción de contenido es Data/DbSeeder.cs, cuya protección de credenciales fue autorizada por separado. Parte de esos archivos reemplaza las versiones adicionales o modificadas de las fases anteriores.

## Respaldo completo y verificado

**Ubicación:** `E:\Kelmer\Ing.Software II\App_Proyecto_backup_alineacion_20261028_105004`

Antes de modificar código o retirar archivos se realizó una copia completa de los 539 archivos existentes y se comparó SHA-256 para cada uno. Resultado: **539 coincidencias, cero diferencias**. Incluye .git, archivos sin seguimiento, fase 1, fase 2 parcial, bin, obj y .vs.

Para liberar los índices .vsidx se cerró temporalmente la solución, tras comprobar que no hubiera documentos sin guardar. Visual Studio permaneció abierto; posteriormente se reabrió la solución original. No se cambiaron configuraciones globales del IDE.

Los 31 archivos retirados se comprobaron individualmente contra el respaldo antes de retirarlos. La comprobación final confirmó que siguen en el respaldo con sus hashes originales. No se modificaron App_Proyecto.zip, JoseProyecto.zip ni los respaldos anteriores.

La verificación detallada está en la copia de entrega VERIFICACION_RESPALDO_ALINEACION.json.

## Identidad y configuraciones conservadas

Se verificaron idénticos al ZIP original, byte por byte:

- App_Proyecto.sln.
- App_Proyecto.csproj.user.
- Program.cs.
- Properties/launchSettings.json.
- appsettings.json y appsettings.Development.json.
- Views/Shared/_Layout.cshtml.

El csproj conserva las propiedades originales, el identificador de secretos propio, el framework y las versiones de paquetes originales. Se mantiene únicamente la adición de Microsoft.EntityFrameworkCore.SqlServer 9.0.20, presente en José y necesaria para las referencias de sus componentes. Se retiró Microsoft.AspNetCore.Identity.EntityFrameworkCore de la fase 2 parcial.

Se recuperó el Program.cs original de plantilla, equivalente al de José. No registra contexto de base de datos, servicios académicos, seeder, AutoMapper, JWT ni autenticación adicional. Se retiraron los enlaces MVC adicionales, la conexión local añadida por Codex y DOTNET_ROLL_FORWARD de los perfiles. No se cambiaron los puertos originales.

Dockerfile y .dockerignore se incorporaron porque existen en la referencia. Solo se adaptaron las referencias de Dockerfile a App_Proyecto.csproj y App_Proyecto.dll. No se ejecutó Docker, no se instaló ninguna herramienta y no se añadieron perfiles ni propiedades Docker al proyecto propio.

## Archivos de referencia incorporados o alineados

- `.dockerignore`
- `Authorization/JwtPermissionPolicies.cs`
- `DTOS/AssignUserPermission.cs`
- `DTOS/AssingPermissionDto.cs`
- `DTOS/ChangePasswordDto.cs`
- `DTOS/CreatePermissionDto.cs`
- `DTOS/CreateRolDto.cs`
- `DTOS/CursoTableDto.cs`
- `DTOS/DataTableResponseDto.cs`
- `DTOS/DataTablesRequestDto.cs`
- `DTOS/EstudianteTableDto.cs`
- `DTOS/ForgetPasswordDto.cs`
- `DTOS/LoginRequestDto.cs`
- `DTOS/LoginResponseDto.cs`
- `DTOS/PermissionDto.cs`
- `DTOS/RegisterRequestDto.cs`
- `DTOS/ResetPasswordRequestDto.cs`
- `DTOS/RoleWithPermissionDto.cs`
- `DTOS/SaveCursoDto.cs`
- `DTOS/SaveEstudianteDto.cs`
- `DTOS/SavePermissionDto.cs`
- `DTOS/UpdateUserRoleDto.cs`
- `DTOS/UserListItemDto.cs`
- `DTOS/UserPermissionDto.cs`
- `DTOS/UserPermissionOverride.cs`
- `Data/AppDBContext.cs`
- `Data/DbSeeder.cs`
- `Dockerfile`
- `Extensions/ClaimsPrincipalExtension.cs`
- `Mapping/UserProfile.cs`
- `Models/Curso.cs`
- `Models/ErrorViewModel.cs`
- `Models/Estudiante.cs`
- `Models/Permission.cs`
- `Models/Role.cs`
- `Models/RolePermission.cs`
- `Models/Settings/JwtSettings.cs`
- `Models/Settings/SmtpSettings.cs`
- `Models/Usuario.cs`
- `Models/UsuarioPermission.cs`
- `Program.cs`
- `Repositories/CursoRepository.cs`
- `Repositories/EstudianteRepository.cs`
- `Repositories/ICursoRepository.cs`
- `Repositories/IEstudianteRepository.cs`
- `Repositories/IPermissionRepository.cs`
- `Repositories/IRoleRepository.cs`
- `Repositories/IUsuarioRepository.cs`
- `Repositories/PermissionRepository.cs`
- `Repositories/RolRepository.cs`
- `Repositories/UsuarioRepository.cs`
- `Services/CursoService.cs`
- `Services/EmailService.cs`
- `Services/IAuthService.cs`
- `Services/ICursoService.cs`
- `Services/IEmailService.cs`
- `Services/IEstudianteService.cs`
- `Services/ITokenService.cs`

Los modelos Curso y Estudiante conservan los nombres de sus propiedades. Se incorporaron los fragmentos correspondientes de José, incluidas su inicialización de colección y su nulabilidad. ErrorViewModel contiene RequestId y ShowRequestId como la referencia y como requieren el controlador y la vista de error existentes.

Se conservaron literalmente los nombres y firmas de la referencia, incluidos DTOS, AssingPermissionDto, ForgetPasswordDto, RolRepository, RecordsTotalFiltered y otros nombres del avance actual. No se rediseñaron contratos ni se ajustaron a una implementación nueva de DataTables.

## Archivos retirados del proyecto activo

- `Authorization/PermissionAuthorization.cs`
- `Authorization/Permissions.cs`
- `Controllers/CursosController.cs`
- `Controllers/EstudiantesController.cs`
- `DTOS/SecurityDtos.cs`
- `DTOS/TableDtos.cs`
- `Data/InitialAcademico.sql`
- `INFORME_INTEGRACION.md`
- `Migrations/20261028162459_InitialAcademico.Designer.cs`
- `Migrations/20261028162459_InitialAcademico.cs`
- `Migrations/AppDBContextModelSnapshot.cs`
- `README_INTEGRACION.md`
- `Repositories/SecurityRepositories.cs`
- `Services/EstudianteService.cs`
- `Services/IUsuarioService.cs`
- `Services/UsuarioService.cs`
- `Views/Cursos/Create.cshtml`
- `Views/Cursos/Delete.cshtml`
- `Views/Cursos/Details.cshtml`
- `Views/Cursos/Edit.cshtml`
- `Views/Cursos/Index.cshtml`
- `Views/Cursos/_Details.cshtml`
- `Views/Cursos/_Form.cshtml`
- `Views/Estudiantes/Create.cshtml`
- `Views/Estudiantes/Delete.cshtml`
- `Views/Estudiantes/Details.cshtml`
- `Views/Estudiantes/Edit.cshtml`
- `Views/Estudiantes/Index.cshtml`
- `Views/Estudiantes/_Details.cshtml`
- `Views/Estudiantes/_Form.cshtml`
- `tests/smoke_mvc.py`

Se retiraron además los directorios que quedaron vacíos: Views/Cursos, Views/Estudiantes, Migrations y tests. No se eliminaron de sus respaldos. Las versiones adicionales de archivos que sí tienen equivalente en José, como repositorios, DTOs, servicios, modelos y AppDBContext, se reemplazaron por el contenido de referencia adaptado y también permanecen conservadas en el respaldo.

## Seeder: única excepción autorizada de contenido

Se conservan DbSeeder, sus métodos, los nombres de roles, las relaciones, los ejemplos académicos y la estructura de inserción de dos cuentas. Sus nombres de cuenta, correos y contraseñas no se copiaron. Se leen exclusivamente estas variables:

| Variable | Uso |
|---|---|
| APP_PROYECTO_SEED_ADMIN_USERNAME | Nombre de la cuenta de ejemplo con rol Admin |
| APP_PROYECTO_SEED_ADMIN_EMAIL | Correo de esa cuenta |
| APP_PROYECTO_SEED_ADMIN_PASSWORD | Contraseña que se pasa al hashing original BCrypt |
| APP_PROYECTO_SEED_USUARIO_USERNAME | Nombre de la cuenta de ejemplo con rol Usuario |
| APP_PROYECTO_SEED_USUARIO_EMAIL | Correo de esa cuenta |
| APP_PROYECTO_SEED_USUARIO_PASSWORD | Contraseña que se pasa al hashing original BCrypt |

Si falta cualquiera o está vacía o contiene solo espacios, SeedUsersAsync omite la creación de las cuentas. No existen valores predeterminados. Se conserva la comprobación original que omite la creación si ya hay usuarios. Los valores secretos no se incluyen en archivos de configuración ni en este informe.

**No se ejecutó el seeder ni se registró para ejecución automática.** Su llamada interna a MigrateAsync permanece tal como en José, pero no se invocó. Esta alineación no ejecutó comandos ni operaciones sobre SQL Server. No se eliminó la base App_Proyecto_Academico ni se modificaron sus registros. Retirar los archivos de migraciones del código no revierte las migraciones que ya estuvieran aplicadas a esa base.

## Métodos y módulos que permanecen pendientes

Se mantienen los tres NotImplementedException originales:

- PermissionRepository.IsAssignedToUsuarioAsync(int permissionId).
- RolRepository.GetByIdWithPermissionsAsync(Guid id).
- UsuarioRepository.GetUsuarioByIdWithPermissionAsync(string email).

No se implementaron EstudianteService, AuthService ni TokenService: no existen implementaciones de esas clases en José. Sus interfaces presentes en la referencia sí están incorporadas. EmailService y CursoService se conservan con el contenido de José.

No se corrigieron los defectos lógicos de la referencia: la FK de UsuarioPermission, la ausencia de su clave compuesta, la comprobación HasAnyEstudentAsync, las diferencias de tipos de identificadores, los nombres de DataTables y otros detalles pendientes. No impiden la compilación y su corrección excedería la autorización. No se completaron esos módulos ni se afirma que funcionen en ejecución.

## Validaciones ejecutadas

| Comprobación | Resultado |
|---|---|
| Copia completa previa y SHA-256 | 539 archivos, cero diferencias |
| Contenido de los 58 archivos de referencia | Solo difiere el seeder autorizado, tras adaptar namespaces y saltos de línea |
| Inventario frente a José | Sin faltantes ni extras antes del informe; identidades de solución/proyecto adaptadas |
| Restauración NuGet con MSBuild de Visual Studio 2026 | Correcta |
| Build con MSBuild de Visual Studio 2026 | Correcto, salida 0 |
| Compilación desde la solución abierta en Visual Studio, vía DTE | LastBuildInfo=0 |
| dotnet build final --no-restore | 0 errores, 22 advertencias; salida 0 |
| git diff --check | Sin errores de espacios; aviso de normalización LF/CRLF por configuración Git existente |
| Conservación de .git/config y commit de master | Iguales al respaldo |
| Archivos retirados | Todos presentes y verificados en el respaldo |

Las 22 advertencias son de nulabilidad (CS8601, CS8602 y CS8618). Incluyen advertencias del código de referencia y cuatro de las cadenas obtenidas del entorno en el seeder: la comprobación conjunta con Any evita su uso cuando faltan, pero el análisis estático no deduce que todas son no nulas después de esa condición. No se ocultaron ni se aplicaron refactorizaciones para eliminarlas.

Se utilizó Visual Studio Community 2026 y el SDK instalado 10.0.402 para compilar el destino net9.0. No se instaló una herramienta global ni se cambió el framework. La ejecución con los perfiles originales requiere disponer del runtime compatible con net9.0; no se añadieron ajustes de ejecución ajenos a la referencia para suplirlo.

No se ejecutaron pruebas de los módulos incompletos ni los 33 escenarios de la fase 1: sus controladores y vistas fueron retirados por autorización y ya no representan el alcance académico. No se realizaron pruebas HTTP, migraciones, seed, instalación de autenticación, JWT operativo o DataTables visual en esta corrección. La validación solicitada es de compilación y fidelidad a la referencia.

## Git y entrega

Rama activa: integracion/alineacion-academica. Se creó sin modificar commits anteriores. .git permanece en su ubicación original, .git/config y el commit de master se verificaron contra el respaldo. No se importó historial de José, no se cambió el remoto, no se añadieron archivos al índice, no se hicieron commits y no se ejecutó push.

El proyecto queda abierto en Visual Studio 2026 y con los cambios sin confirmar para revisión. La solución reproduce los componentes y pendientes de la referencia, sin los desarrollos adicionales de las fases anteriores.
