using App_Proyecto.DTOS;
using App_Proyecto.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App_Proyecto.Controllers
{
    public class CursoController(ICursoService cursoService) : Controller
    {
        [Authorize(Policy = "curso.ver")]

        public IActionResult Index()
        {
            return View();

        }
        [Authorize(Policy = "curso.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData([FromForm]DataTablesRequestDto request)

        {
            var result = await cursoService.GetPagedAsync(request);
            return Ok(result);

        }
        [Authorize(Policy = "curso.ver")]
        [HttpGet]
        public async Task<IActionResult> GetById(int Id)  
        {
            var result = await cursoService.GetByIdAsync(Id);
            if (result == null)
            {
                return NotFound(new { message = "Curso no encontrado" });
            } 
                return Ok(result);
        }

        [Authorize(Policy = "curso.crear")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]SaveCursoDto cursoDto)
        {
           if(!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Revise los datos del curso" });
            }
            var (success, Errormessage) = await cursoService.CreateAsync(cursoDto);
            return success ?
                Ok(new { success = true, message = "Curso creado correctamente" }) :
                BadRequest(new { success = false, message = Errormessage });
        }

        [Authorize(Policy = "curso.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody]SaveCursoDto cursoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Revise los datos del curso" });
            }
            var (success, Errormessage) = await cursoService.UpdateAsync(cursoDto);
            return success ?
                Ok(new { success = true, message = "Curso actualizado correctamente" }) :
                BadRequest(new { success = false, message = Errormessage });
        }

        [Authorize(Policy = "curso.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int Id)
        {
            var (success, Errormessage) = await cursoService.DeleteAsync(Id);
            return success ?
                Ok(new { success = true, message = "Curso eliminado correctamente" }) :
                BadRequest(new { success = false, message = Errormessage });
        }

    }
}
