using System.ComponentModel.DataAnnotations;

namespace App_Proyecto.DTOS
{
    public class SavePeriodoAcademicoDto
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Nombre { get; set; } = string.Empty;

        public bool MatriculaAbierta { get; set; }
    }
}
