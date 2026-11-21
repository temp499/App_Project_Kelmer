namespace App_Proyecto.Models
{
    public class DetalleHorario
    {
        public int Id { get; set; }
        public int DetalleOfertaAcademicaId { get; set; }
        public DetalleOfertaAcademica? DetalleOfertaAcademica { get; set; }
        public int AulaId { get; set; }
        public Aula? Aula { get; set; }
        public int DiaSemana { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
    }
}
