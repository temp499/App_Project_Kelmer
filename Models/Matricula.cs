namespace App_Proyecto.Models
{
    public class Matricula
    {
        public int Id { get; set; }
        public int EstudianteId { get; set; }
        public Estudiante Estudiante { get; set; } = null!;
        public int PeriodoAcademicoId { get; set; }
        public PeriodoAcademico PeriodoAcademico { get; set; } = null!;
        public ICollection<DetalleMatricula> Detalles { get; set; } = [];
    }
}
