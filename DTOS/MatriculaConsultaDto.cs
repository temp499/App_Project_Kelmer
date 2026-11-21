namespace App_Proyecto.DTOS
{
    public class MatriculaConsultaDto
    {
        public int Id { get; set; }
        public int EstudianteId { get; set; }
        public string EstudianteNombre { get; set; } = string.Empty;
        public int PeriodoAcademicoId { get; set; }
        public string PeriodoNombre { get; set; } = string.Empty;
        public IReadOnlyList<CursoMatriculaDto> Cursos { get; set; } = [];
    }
    public class CursoMatriculaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int? GrupoId { get; set; }
        public string Grupo { get; set; } = string.Empty;
        public IReadOnlyList<HorarioGrupoDto> Horarios { get; set; } = [];
    }
}
