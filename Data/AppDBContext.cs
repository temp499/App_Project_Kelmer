using App_Proyecto.Models;
using Microsoft.EntityFrameworkCore;

namespace App_Proyecto.Data
{
    public class AppDBContext(DbContextOptions<AppDBContext> options) : DbContext(options)
    {
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Estudiante> Estudiantes { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet <UsuarioPermiso> UsuarioPermiso { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId , rp.PermissionId });


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

            modelBuilder.Entity<UsuarioPermiso>()
               .HasOne(Up => Up.Usuario)
               .WithMany(Up => Up.UsuarioPermisos)
               .HasForeignKey(up => up.UsuarioId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UsuarioPermiso>()
               .HasOne(Up => Up.Permission)
               .WithMany(Up => Up.UsuarioPermissions)
               .HasForeignKey(up => up.PermissionId)
               .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Estudiante>()
               .HasOne(e => e.Curso)
               .WithMany(e => e.Estudiantes)
               .HasForeignKey(c => c.CursoId)
               .OnDelete(DeleteBehavior.Restrict);

        }


    }


}

