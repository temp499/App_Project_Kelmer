using System.ComponentModel.DataAnnotations;

namespace App_Proyecto.DTOS
{
    public class SaveDetalleHorarioDto
    {
        public int? Id { get; set; }

        [Range(1, int.MaxValue)]
        public int DetalleOfertaAcademicaId { get; set; }

        [Range(1, int.MaxValue)]
        public int AulaId { get; set; }

        [Range(1, 7)]
        public int DiaSemana { get; set; }

        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
    }
}
