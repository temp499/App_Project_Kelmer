namespace App_Proyecto.Models
{
    public class DetallePlanEstudio
    {
        public int Id { get; set; }
        public int PlanEstudioId { get; set; }
        public PlanEstudio? PlanEstudio { get; set; }
        public int CursoId { get; set; }
        public Curso? Curso { get; set; }
        public int Creditos { get; set; }
        public int Semestre { get; set; }
    }
}
