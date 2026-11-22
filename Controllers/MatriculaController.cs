using App_Proyecto.DTOS;
using App_Proyecto.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App_Proyecto.Controllers
{
    [Authorize(Roles = "Admin")]


    public class MatriculaController(
        IMatriculaService service,
        IEstudianteService estudiantes,
        IRolePermissionService usuarios,
        ICarreraService carreras) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Estudiantes = await estudiantes.GetAllAsync();
            ViewBag.Usuarios = await usuarios.GetAllUsersAsync();
            ViewBag.Carreras = await carreras.GetAllAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromQuery] int estudianteId,
            [FromBody] CrearMatriculaDto dto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Revisa los datos de la matricula" });

            var (matriculaId, errorMessage) = await service.CreateAsync(
                estudianteId, dto, cancellationToken);

            if (!matriculaId.HasValue)
                return BadRequest(new { success = false, message = errorMessage });

            return CreatedAtAction(nameof(GetById), new { id = matriculaId.Value }, new
            {
                success = true,
                matriculaId = matriculaId.Value,
                message = "Matricula registrada correctamente"
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                return BadRequest(new { message = "El identificador de matricula no es valido" });

            var matricula = await service.GetByIdAsync(id, cancellationToken);
            if (matricula == null)
                return NotFound(new { message = "Matricula no encontrada" });

            return Ok(matricula);
        }

        [HttpGet]
        public async Task<IActionResult> GetHistorial(
            [FromQuery] ConsultaHistorialDto consulta,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Revisa el estudiante y la Pagina Solicitada" });
            var historial = await service.GetHistorialAsync(consulta, cancellationToken);
            if (historial == null)
                return NotFound(new { message = "Estudiante no encontrado" });

            return Ok(historial);

        }
    }
}
