using App_Proyecto.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace App_Proyecto.Data

{
    public class AppDBContext(DbContextOptions<AppDBContext> options) : DbContext(options)
    {

        public DbSet<Role> Roles => Set<Role>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<UsuarioPermission> UsuarioPermissions => Set<UsuarioPermission>();
        public DbSet<Estudiante> Estudiantes => Set<Estudiante>();
        public DbSet<Curso> Cursos => Set<Curso>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UsuarioPermission>()
                .HasKey(up => new { up.UsuarioId, up.PermissionId });

            modelBuilder.Entity<UsuarioPermission>()
                .HasOne(Up => Up.Usuario)
                .WithMany(u => u.UsuarioPermissions)
                .HasForeignKey(up => up.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UsuarioPermission>()
                .HasOne(Up => Up.Permission)
                .WithMany(u => u.UsuarioPermissions)
                .HasForeignKey(up => up.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Estudiante>()
                .HasOne(e => e.Curso)
                .WithMany(c => c.Estudiantes)
                .HasForeignKey(e => e.CursoId)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
