using System.ComponentModel.DataAnnotations;

namespace App_Proyecto.DTOS
{
    public class AsignarCarreraDto
    {
        [Range(1, int.MaxValue)]
        public int CarreraId { get; set; }
    }
}
