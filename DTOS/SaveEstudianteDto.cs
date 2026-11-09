using System.ComponentModel.DataAnnotations;

namespace App_Proyecto.DTOS
{
    public class SaveEstudianteDto
    {
        public int? Id { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Apellido { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un curso")]
        public int CursoId { get; set; }

    }
}
