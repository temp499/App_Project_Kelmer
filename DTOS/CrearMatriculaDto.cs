using System.ComponentModel.DataAnnotations;

namespace App_Proyecto.DTOS
{
    public class CrearMatriculaDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Selecciona un periodo academico")]
        public int PeriodoAcademicoId { get; set; }
        [Required(ErrorMessage = "Selecciona al menos un curso")]
        public int[] CursoIds { get; set; } = [];

        [Required]
        public int[] GrupoIds { get; set; } = [];
    }
}
