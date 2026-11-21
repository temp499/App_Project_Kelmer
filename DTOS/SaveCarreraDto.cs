using System.ComponentModel.DataAnnotations;

namespace App_Proyecto.DTOS
{
    public class SaveCarreraDto
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Nombre { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int FacultadId { get; set; }
    }
}
