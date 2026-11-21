namespace App_Proyecto.Models
{
    public class PlanEstudio
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public int CarreraId { get; set; }
        public Carrera? Carrera { get; set; }
    }
}
