namespace App_Proyecto.DTOS
{
    public class GrupoDisponibleDto
    {
        public int Id { get; set; }
        public int CursoId { get; set; }
        public string CursoNombre { get; set; } = string.Empty;
        public string Grupo { get; set; } = string.Empty;
        public string Sede { get; set; } = string.Empty;
        public string Docente { get; set; } = string.Empty;
        public int Cupo { get; set; }
        public int Inscritos { get; set; }
        public int Disponibles => Math.Max(0, Cupo - Inscritos);
        public IReadOnlyList<HorarioGrupoDto> Horarios { get; set; } = [];
    }

    public class HorarioGrupoDto
    {
        public int DiaSemana { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public string Aula { get; set; } = string.Empty;
    }
}
