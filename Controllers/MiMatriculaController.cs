using System.Security.Claims;
using App_Proyecto.DTOS;
using App_Proyecto.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App_Proyecto.Controllers
{
    [Authorize]
    public class MiMatriculaController(
        IMatriculaEstudianteService service,
        ICursoService cursos,
        IPeriodoAcademicoService periodos) : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Oferta(int periodoId, CancellationToken cancellationToken)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var usuarioId) || usuarioId <= 0)
                return Unauthorized();

            if (periodoId <= 0)
                return BadRequest(new { message = "Selecciona un periodo valido" });

            return Ok(await service.GetOfertaAsync(usuarioId, periodoId, cancellationToken));
        }

        [HttpGet]
        public async Task<IActionResult> Historial(CancellationToken cancellationToken)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var usuarioId) || usuarioId <= 0)
                return Unauthorized();

            return Ok(await service.GetHistorialAsync(usuarioId, cancellationToken));
        }

        [HttpGet]
        public async Task<IActionResult> Perfil(CancellationToken cancellationToken)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var usuarioId) || usuarioId <= 0)
                return Unauthorized();

            var perfil = await service.GetPerfilAsync(usuarioId, cancellationToken);
            if (perfil == null)
                return NotFound(new { message = "Tu cuenta no tiene un estudiante vinculado" });

            return Ok(perfil);
        }

        [HttpGet]
        public async Task<IActionResult> Cursos()
        {
            return Ok(await cursos.GetAllAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Periodos()
        {
            var disponibles = await periodos.GetAllAsync();
            return Ok(disponibles.Where(periodo => periodo.MatriculaAbierta).ToList());
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CrearMatriculaDto dto, CancellationToken cancellationToken)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var usuarioId) || usuarioId <= 0)
                return Unauthorized();

            if (!ModelState.IsValid)
                return BadRequest(new { message = "Revisa los cursos y el periodo de matricula" });

            var resultado = await service.CreateAsync(usuarioId, dto, cancellationToken);
            if (!resultado.MatriculaId.HasValue)
                return BadRequest(new { message = resultado.ErrorMessage });

            return CreatedAtAction(nameof(GetById), new { id = resultado.MatriculaId.Value }, new
            {
                matriculaId = resultado.MatriculaId.Value,
                message = "Matricula registrada correctamente"
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var usuarioId) || usuarioId <= 0)
                return Unauthorized();

            if (id <= 0)
                return BadRequest(new { message = "Selecciona una matricula valida" });

            var matricula = await service.GetByIdAsync(usuarioId, id, cancellationToken);
            if (matricula == null)
                return NotFound(new { message = "Matricula no encontrada" });

            return Ok(matricula);
        }
    }
}
