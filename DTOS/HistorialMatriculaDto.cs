namespace App_Proyecto.DTOS
{
    public class HistorialMatriculaDto
    {
        public int EstudianteId { get; set; }
        public String EstudianteNombre { get; set; } = string.Empty;
        public int Pagina { get; set; }
        public int TamañoPagina { get; set; }
        public int TotalRegistros { get; set; }

        public IReadOnlyList<MatriculaResumenDto> Matriculas { get; set; } = [];

        public class MatriculaResumenDto
        {
            public int Id { get; set; }
            public int PeriodoAcademicoId { get; set; }
            public string PeriodoNombre { get; set; } = string.Empty;

            public int TotalCursos { get; set; }
        }



    }
}
