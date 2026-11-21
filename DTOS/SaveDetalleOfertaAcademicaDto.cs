using System.ComponentModel.DataAnnotations;

namespace App_Proyecto.DTOS
{
    public class SaveDetalleOfertaAcademicaDto
    {
        public int? Id { get; set; }

        [Range(1, int.MaxValue)]
        public int OfertaAcademicaId { get; set; }

        [Range(1, int.MaxValue)]
        public int CursoId { get; set; }

        [Range(1, int.MaxValue)]
        public int DocenteId { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 1)]
        public string Grupo { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Cupo { get; set; }
    }
}
