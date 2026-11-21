using App_Proyecto.DTOS;
using App_Proyecto.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App_Proyecto.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DetalleHorarioController(IDetalleHorarioService service) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> GetData(int? detalleOfertaAcademicaId)
        {
            return Ok(await service.GetAllAsync(detalleOfertaAcademicaId));
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await service.GetByIdAsync(id);
            if (item == null)
                return NotFound(new { message = "Registro no encontrado" });

            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveDetalleHorarioDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Revisa los datos del horario" });

            var result = await service.CreateAsync(dto);
            if (!result.Success)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(new { success = true, message = "Registro guardado correctamente" });
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] SaveDetalleHorarioDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Revisa los datos del horario" });

            var result = await service.UpdateAsync(dto);
            if (!result.Success)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(new { success = true, message = "Registro actualizado correctamente" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await service.DeleteAsync(id);
            if (!result.Success)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(new { success = true, message = "Registro eliminado correctamente" });
        }
    }
}
