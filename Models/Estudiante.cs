namespace App_Proyecto.Models
{
    public class Estudiante
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public int CursoId { get; set; }
        public virtual Curso Curso { get; set; }
    }

}
