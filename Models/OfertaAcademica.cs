namespace App_Proyecto.Models
{
    public class OfertaAcademica
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int PeriodoAcademicoId { get; set; }
        public int SedeId { get; set; }
        public bool Activa { get; set; }
        public int PlanEstudioId { get; set; }
        public PlanEstudio? PlanEstudio { get; set; }
        public PeriodoAcademico? PeriodoAcademico { get; set; }
        public Sede? Sede { get; set; }
    }
}
