using System.ComponentModel.DataAnnotations;

namespace App_Proyecto.DTOS
{
    public class SaveDetallePlanEstudioDto
    {
        public int? Id { get; set; }

        [Range(1, int.MaxValue)]
        public int PlanEstudioId { get; set; }

        [Range(1, int.MaxValue)]
        public int CursoId { get; set; }

        [Range(1, int.MaxValue)]
        public int Creditos { get; set; }

        [Range(1, int.MaxValue)]
        public int Semestre { get; set; }
    }
}
