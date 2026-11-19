using App_Proyecto.DTOS;
using App_Proyecto.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App_Proyecto.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PeriodoAcademicoController(IPeriodoAcademicoService service) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            return Ok(await service.GetAllAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SavePeriodoAcademicoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Revisa los datos del periodo" });

            var (success, errorMessage) = await service.CreateAsync(dto);
            if (!success)
                return BadRequest(new { success = false, message = errorMessage });

            return Ok(new { success = true, message = "Periodo creado correctamente" });
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] SavePeriodoAcademicoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Revisa los datos del periodo" });

            var (success, errorMessage) = await service.UpdateAsync(dto);
            if (!success)
                return BadRequest(new { success = false, message = errorMessage });

            return Ok(new { success = true, message = "Periodo actualizado correctamente" });
        }
    }
}
