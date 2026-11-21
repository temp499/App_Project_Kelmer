using App_Proyecto.DTOS;
using App_Proyecto.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App_Proyecto.Controllers
{
    public class EstudianteController(IEstudianteService estudianteService, ICursoService cursoService) : Controller
    {
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AsignarCarrera(int id, [FromBody] AsignarCarreraDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Selecciona una carrera valida" });

            var resultado = await estudianteService.AsignarCarreraAsync(id, dto.CarreraId);
            if (!resultado.Success)
                return BadRequest(new { message = resultado.ErrorMessage });

            return Ok(new { message = "Carrera asignada correctamente" });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> VincularUsuario(int id, [FromBody] VincularEstudianteDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Selecciona una cuenta valida" });

            var resultado = await estudianteService.VincularUsuarioAsync(id, dto.UsuarioId);
            if (!resultado.Success)
                return BadRequest(new { message = resultado.ErrorMessage });

            return Ok(new { message = "Cuenta vinculada correctamente" });
        }

        [Authorize(Policy = "estudiante.ver")]
        public async Task<IActionResult> Index()
        {
            return View(await cursoService.GetAllAsync());
        }

        [Authorize(Policy = "estudiante.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData([FromForm] DataTablesRequestDto request)
        {
            var result = await estudianteService.GetPagedAsync(request);
            return Ok(result);
        }

        [Authorize(Policy = "estudiante.ver")]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await estudianteService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = "Estudiante no encontrado" });
            return Ok(result);
        }

        [Authorize(Policy = "estudiante.crear")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveEstudianteDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Error en los datos a guardar" });
            var (success, errorMessage) = await estudianteService.CreateAsync(dto);
            return success ?
                Ok(new { success = true, message = "Estudiante creado correctamente" })
                : BadRequest(new { success = false, errorMessage });
        }

        [Authorize(Policy = "estudiante.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] SaveEstudianteDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Error en los datos a guardar" });
            var (success, errorMessage) = await estudianteService.UpdateAsync(dto);
            return success ?
                Ok(new { success = true, message = "Estudiante actualizado correctamente" })
                : BadRequest(new { success = false, errorMessage });
        }

        [Authorize(Policy = "estudiante.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, errorMessage) = await estudianteService.DeleteAsync(id);
            return success ?
                Ok(new { success = true, message = "Estudiante eliminado correctamente" })
                : BadRequest(new { success = false, errorMessage });
        }
    }
}

