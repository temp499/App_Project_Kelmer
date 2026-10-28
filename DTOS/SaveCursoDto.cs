using System.ComponentModel.DataAnnotations;

namespace App_Proyecto.DTOS
{
    public class SaveCursoDto
    {
        public int? Id { get; set; }
        [Required]
        [MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;
        [MaxLength]
        public string? Descripcion { get; set; }

    }
}
