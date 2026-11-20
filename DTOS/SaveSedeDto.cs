using System.ComponentModel.DataAnnotations;

namespace App_Proyecto.DTOS
{
    public class SaveSedeDto
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Descripcion { get; set; }
    }
}
