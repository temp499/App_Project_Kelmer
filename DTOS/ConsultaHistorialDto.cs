using System.ComponentModel.DataAnnotations;

namespace App_Proyecto.DTOS
{

    public class ConsultaHistorialDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Selecciona un estudiante valido")]
        public int EstudianteId { get; set; }

        [Range(1, int.MaxValue / 50, ErrorMessage = "La pagina no es valida")]
        public int Pagina { get; set; } = 1;

        [Range(1, 50, ErrorMessage = "Solicita entre 1 y 50 Registros")]
        public int TamañoPagina { get; set; } = 10;

    }
}
