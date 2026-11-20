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
        public DbSet<Carrera> Carreras => Set<Carrera>();
        public DbSet<Facultad> Facultades => Set<Facultad>();
        public DbSet<PeriodoAcademico> PeriodosAcademicos => Set<PeriodoAcademico>();
        public DbSet<Matricula> Matriculas => Set<Matricula>();
        public DbSet<DetalleMatricula> DetallesMatricula => Set<DetalleMatricula>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Carrera>()
                .HasOne(item => item.Facultad)
                .WithMany()
                .HasForeignKey(item => item.FacultadId)
                .OnDelete(DeleteBehavior.Restrict);

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

            modelBuilder.Entity<Matricula>()
                .HasIndex(m => new { m.EstudianteId, m.PeriodoAcademicoId })
                .IsUnique();

            modelBuilder.Entity<Matricula>()
                .HasOne(m => m.Estudiante)
                .WithMany()
                .HasForeignKey(m => m.EstudianteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Matricula>()
                .HasOne(m => m.PeriodoAcademico)
                .WithMany()
                .HasForeignKey(m => m.PeriodoAcademicoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DetalleMatricula>()
                .HasKey(d => new { d.MatriculaId, d.CursoId });

            modelBuilder.Entity<DetalleMatricula>()
                .HasOne(d => d.Matricula)
                .WithMany(m => m.Detalles)
                .HasForeignKey(d => d.MatriculaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DetalleMatricula>()
                .HasOne(d => d.Curso)
                .WithMany()
                .HasForeignKey(d => d.CursoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Estudiante>()
                .HasIndex(estudiante => estudiante.UsuarioId)
                .IsUnique()
                .HasFilter("[UsuarioId] IS NOT NULL");

            modelBuilder.Entity<Estudiante>()
                .HasOne(estudiante => estudiante.Usuario)
                .WithOne()
                .HasForeignKey<Estudiante>(estudiante => estudiante.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Estudiante>()
                .HasOne(e => e.Curso)
                .WithMany(c => c.Estudiantes)
                .HasForeignKey(e => e.CursoId)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
