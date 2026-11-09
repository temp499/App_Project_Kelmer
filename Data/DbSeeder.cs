using Microsoft.EntityFrameworkCore;
using App_Proyecto.Models;

namespace App_Proyecto.Data
{
    public class DbSeeder
    {
        public static async Task SeedAsync(AppDBContext context)
        {
            if (context.Database.GetMigrations().Any())
                await context.Database.MigrateAsync();
            else
                await context.Database.EnsureCreatedAsync();

            await using var transaction = await context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            await SeedRolesAsync(context);
            await SeedPermissionsAsync(context);
            await SeedRolePermissionsAsync(context);
            await SeedUsersAsync(context);
            await SeedCursosAsync(context);
            await SeedEstudiantesAsync(context);
            await transaction.CommitAsync();
        }

        private static async Task SeedRolesAsync(AppDBContext context)
        {
            var changed = false;
            if (!await context.Roles.AnyAsync(r => r.Name == "Admin"))
            {
                context.Roles.Add(new Role { Name = "Admin" });
                changed = true;
            }
            if (!await context.Roles.AnyAsync(r => r.Name == "Usuario"))
            {
                context.Roles.Add(new Role { Name = "Usuario" });
                changed = true;
            }
            if (changed)
            {
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedPermissionsAsync(AppDBContext context)
        {
            if (await context.Permissions.AnyAsync())
            {
                if (!await context.Permissions.AnyAsync(p => p.Nombre == "admin.panel"))
                {
                    var admin = await context.Roles.FirstAsync(r => r.Name == "Admin");
                    context.RolePermissions.Add(new RolePermission
                    {
                        RoleId = admin.Id,
                        Permission = new Permission { Nombre = "admin.panel", Descripcion = "Acceder al panel administrativo" }
                    });
                    await context.SaveChangesAsync();
                }
                return;
            }
            context.Permissions.AddRange(
                new Permission { Nombre = "admin.panel", Descripcion = "Acceder al panel administrativo" },
                new Permission { Nombre = "estudiante.ver", Descripcion = "Ver estudiante" },
                new Permission { Nombre = "estudiante.crear", Descripcion = "Crear estudiante" },
                new Permission { Nombre = "estudiante.actualizar", Descripcion = "Actualizar estudiante" },
                new Permission { Nombre = "estudiante.borrar", Descripcion = "Borrar estudiante" },
                new Permission { Nombre = "rol.ver", Descripcion = "Ver roles" },
                new Permission { Nombre = "rol.crear", Descripcion = "Crear rol" },
                new Permission { Nombre = "rol.asignar", Descripcion = "Asignar rol" },
                new Permission { Nombre = "permiso.ver", Descripcion = "Ver permisos" },
                new Permission { Nombre = "permiso.crear", Descripcion = " Crear permisos" },
                new Permission { Nombre = "permiso.actualizar", Descripcion = "Actualizar permisos" },
                new Permission { Nombre = "permiso.borrar", Descripcion = "Borrar permisos" },
                new Permission { Nombre = "usuario.ver", Descripcion = "Ver usuarios" },
                new Permission { Nombre = "usuario.asignar", Descripcion = "Asignar usuarios" },
                new Permission { Nombre = "usuario.cambiar_rol", Descripcion = "Cambiar rol de usuario" },
                new Permission { Nombre = "curso.ver", Descripcion = "Ver curso" },
                new Permission { Nombre = "curso.crear", Descripcion = "Crear curso" },
                new Permission { Nombre = "curso.actualizar", Descripcion = "Actualizar curso" },
                new Permission { Nombre = "curso.borrar", Descripcion = "Borrar curso " });
            await context.SaveChangesAsync();
        }

        private static async Task SeedRolePermissionsAsync(AppDBContext context)
        {
            if (await context.RolePermissions.AnyAsync())
                return;
            var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");
            var usuarioRole = await context.Roles.FirstAsync(r => r.Name == "Usuario");

            var permissions = await context.Permissions.ToListAsync();

            foreach (var permission in permissions)
            {
                context.RolePermissions.Add(new RolePermission
                {

                    RoleId = adminRole.Id,
                    PermissionId = permission.Id
                });
            }

            var usuarioPermissions = permissions.
                Where(p => p.Nombre is "estudiante.ver" or "curso.ver")
                .ToList();

            foreach (var permission in usuarioPermissions)
            {

                context.RolePermissions.Add(new RolePermission
                {
                    RoleId = usuarioRole.Id,
                    PermissionId = permission.Id
                });
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedUsersAsync(AppDBContext context)
        {
            if (await context.Usuarios.AnyAsync())
                return;
            // Credenciales propias opcionales; no se copian cuentas de la referencia.
            var adminUserName = Environment.GetEnvironmentVariable("APP_PROYECTO_SEED_ADMIN_USERNAME");
            var adminEmail = Environment.GetEnvironmentVariable("APP_PROYECTO_SEED_ADMIN_EMAIL");
            var adminPassword = Environment.GetEnvironmentVariable("APP_PROYECTO_SEED_ADMIN_PASSWORD");
            var usuarioUserName = Environment.GetEnvironmentVariable("APP_PROYECTO_SEED_USUARIO_USERNAME");
            var usuarioEmail = Environment.GetEnvironmentVariable("APP_PROYECTO_SEED_USUARIO_EMAIL");
            var usuarioPassword = Environment.GetEnvironmentVariable("APP_PROYECTO_SEED_USUARIO_PASSWORD");
            if (new[] { adminUserName, adminEmail, adminPassword, usuarioUserName, usuarioEmail, usuarioPassword }.Any(string.IsNullOrWhiteSpace))
                return;

            var AdminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");
            var UsuarioRole = await context.Roles.FirstAsync(r => r.Name == "Usuario");

            context.Usuarios.AddRange(
                new Usuario
                {
                    UserName = adminUserName!,
                    Email = adminEmail!,
                    password = BCrypt.Net.BCrypt.EnhancedHashPassword(adminPassword!),
                    EmailConfirmed = "true",
                    RolId = AdminRole.Id,
                },
                new Usuario
                {
                    UserName = usuarioUserName!,
                    Email = usuarioEmail!,
                    password = BCrypt.Net.BCrypt.EnhancedHashPassword(usuarioPassword!),
                    EmailConfirmed = "true",
                    RolId = UsuarioRole.Id,
                });
            await context.SaveChangesAsync();

        }

        private static async Task SeedCursosAsync(AppDBContext context)
        {
            if (await context.Cursos.AnyAsync())
                return;
            context.Cursos.AddRange(
                new Curso { Nombre = "Matemáticas", Descripcion = "Algebra y geometría" },
                new Curso { Nombre = "Programacion", Descripcion = "Fundamentos de desarrollo" },
                new Curso { Nombre = "Historia", Descripcion = "Historia Universal" });
            await context.SaveChangesAsync();
        }
        private static async Task SeedEstudiantesAsync(AppDBContext context)
        {
            if (await context.Estudiantes.AnyAsync())
                return;
            var matematicas = await context.Cursos.FirstOrDefaultAsync(c => c.Nombre == "Matemáticas");
            var programacion = await context.Cursos.FirstOrDefaultAsync(c => c.Nombre == "Programacion");
            var historia = await context.Cursos.FirstOrDefaultAsync(c => c.Nombre == "Historia");
            if (matematicas is null || programacion is null || historia is null)
                return;
            context.Estudiantes.AddRange(
                new Estudiante { Nombre = "Juan", Apellido = "Perez", Email = "juan.perez@gmail.com", CursoId = matematicas.Id },
                new Estudiante { Nombre = "Maria", Apellido = "Lopez", Email = "maria.lopez@gmail.com", CursoId = programacion.Id },
                new Estudiante { Nombre = "Carlos", Apellido = "Sanchez", Email = "carlos.sanchez@gmail.com", CursoId = historia.Id },
                new Estudiante { Nombre = "Ana", Apellido = "Gomez", Email = "ana.gomez@gmail.com", CursoId = programacion.Id });
            await context.SaveChangesAsync();
        }
    }
}



