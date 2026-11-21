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
        public DbSet<DetalleOfertaAcademica> DetallesOfertaAcademica => Set<DetalleOfertaAcademica>();
        public DbSet<OfertaAcademica> OfertasAcademicas => Set<OfertaAcademica>();
        public DbSet<DetallePlanEstudio> DetallesPlanEstudio => Set<DetallePlanEstudio>();
        public DbSet<PlanEstudio> PlanesEstudio => Set<PlanEstudio>();
        public DbSet<Docente> Docentes => Set<Docente>();
        public DbSet<Aula> Aulas => Set<Aula>();
        public DbSet<Sede> Sedes => Set<Sede>();
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

            modelBuilder.Entity<Aula>()
                .HasOne(item => item.Sede)
                .WithMany()
                .HasForeignKey(item => item.SedeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PlanEstudio>()
                .HasOne(item => item.Carrera)
                .WithMany()
                .HasForeignKey(item => item.CarreraId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DetallePlanEstudio>()
                .HasIndex(detalle => new { detalle.PlanEstudioId, detalle.CursoId })
                .IsUnique();

            modelBuilder.Entity<DetallePlanEstudio>()
                .HasOne(detalle => detalle.PlanEstudio)
                .WithMany()
                .HasForeignKey(detalle => detalle.PlanEstudioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DetallePlanEstudio>()
                .HasOne(detalle => detalle.Curso)
                .WithMany()
                .HasForeignKey(detalle => detalle.CursoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OfertaAcademica>()
                .HasIndex(oferta => new { oferta.PlanEstudioId, oferta.PeriodoAcademicoId, oferta.SedeId })
                .IsUnique();

            modelBuilder.Entity<OfertaAcademica>()
                .HasOne(oferta => oferta.PeriodoAcademico)
                .WithMany()
                .HasForeignKey(oferta => oferta.PeriodoAcademicoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OfertaAcademica>()
                .HasOne(oferta => oferta.Sede)
                .WithMany()
                .HasForeignKey(oferta => oferta.SedeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OfertaAcademica>()
                .HasOne(item => item.PlanEstudio)
                .WithMany()
                .HasForeignKey(item => item.PlanEstudioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DetalleOfertaAcademica>()
                .HasIndex(grupo => new { grupo.OfertaAcademicaId, grupo.CursoId, grupo.Grupo })
                .IsUnique();

            modelBuilder.Entity<DetalleOfertaAcademica>()
                .HasOne(grupo => grupo.OfertaAcademica)
                .WithMany()
                .HasForeignKey(grupo => grupo.OfertaAcademicaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DetalleOfertaAcademica>()
                .HasOne(grupo => grupo.Curso)
                .WithMany()
                .HasForeignKey(grupo => grupo.CursoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DetalleOfertaAcademica>()
                .HasOne(grupo => grupo.Docente)
                .WithMany()
                .HasForeignKey(grupo => grupo.DocenteId)
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
