namespace App_Proyecto.Models
{
    public class DetalleMatricula
    {
        public int MatriculaId { get; set; }
        public Matricula Matricula { get; set; } = null!;
        public int CursoId { get; set; }
        public Curso Curso { get; set; } = null!;
    }
}
