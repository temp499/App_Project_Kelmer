using App_Proyecto.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App_Proyecto.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MatriculaController(IMatriculaService service) : Controller
    {
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
    }
}
