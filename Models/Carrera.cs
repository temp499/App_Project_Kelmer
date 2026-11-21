namespace App_Proyecto.Models
{
    public class Carrera
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int FacultadId { get; set; }
        public Facultad? Facultad { get; set; }
    }
}
