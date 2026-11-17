namespace App_Proyecto.DTOS
{
    public class CrearMatriculaDto
    {
        public int PeriodoAcademicoId { get; set; }
        public int[] CursoIds { get; set; } = [];
    }
}
