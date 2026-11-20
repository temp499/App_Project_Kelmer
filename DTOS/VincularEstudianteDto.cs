using System.ComponentModel.DataAnnotations;

namespace App_Proyecto.DTOS
{
    public class VincularEstudianteDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Selecciona una cuenta valida")]
        public int UsuarioId { get; set; }
    }
}
