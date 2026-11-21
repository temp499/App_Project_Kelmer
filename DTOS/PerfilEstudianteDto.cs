namespace App_Proyecto.DTOS
{
    public class PerfilEstudianteDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int? CarreraId { get; set; }
        public string CarreraNombre { get; set; } = string.Empty;
    }
}
