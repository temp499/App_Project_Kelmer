namespace App_Proyecto.Models
{
    public class Aula
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Capacidad { get; set; }
        public int SedeId { get; set; }
        public Sede? Sede { get; set; }
    }
}
