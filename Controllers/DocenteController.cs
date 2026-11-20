using App_Proyecto.DTOS;
using App_Proyecto.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App_Proyecto.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DocenteController(IDocenteService service) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            return Ok(await service.GetAllAsync());
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
        public async Task<IActionResult> Create([FromBody] SaveDocenteDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Revisa los datos de docente" });

            var result = await service.CreateAsync(dto);
            if (!result.Success)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(new { success = true, message = "Registro guardado correctamente" });
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] SaveDocenteDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Revisa los datos de docente" });

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
