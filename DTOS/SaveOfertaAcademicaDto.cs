using System.ComponentModel.DataAnnotations;

namespace App_Proyecto.DTOS
{
    public class SaveOfertaAcademicaDto
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Nombre { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int PlanEstudioId { get; set; }

        [Range(1, int.MaxValue)]
        public int PeriodoAcademicoId { get; set; }

        [Range(1, int.MaxValue)]
        public int SedeId { get; set; }

        public bool Activa { get; set; }
    }
}
