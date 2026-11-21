using System.ComponentModel.DataAnnotations;

namespace App_Proyecto.Models
{
    public class DetalleOfertaAcademica
    {
        public int Id { get; set; }
        public int OfertaAcademicaId { get; set; }
        public OfertaAcademica? OfertaAcademica { get; set; }
        public int CursoId { get; set; }
        public Curso? Curso { get; set; }
        public int DocenteId { get; set; }
        public Docente? Docente { get; set; }

        [MaxLength(20)]
        public string Grupo { get; set; } = string.Empty;

        public int Cupo { get; set; }
        public ICollection<DetalleHorario> Horarios { get; set; } = [];
    }
}
