using App_Proyecto.DTOS;
using App_Proyecto.Extensions;
using App_Proyecto.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App_Proyecto.Controllers;

[Authorize]
public class AdminController(IRolePermissionService service) : Controller
{
    [HttpGet]
    [Authorize(Policy = "admin.panel")]
    public IActionResult Index() => View(User.GetPermission());

    [HttpGet]
    [Authorize(Policy = "permiso.ver")]
    public IActionResult Permissions() => View();

    [HttpPost]
    [Authorize(Policy = "permiso.ver")]
    public async Task<IActionResult> GetPermissionsData([FromForm] DataTablesRequestDto request) =>
        Ok(await service.GetPermissionsPagedAsync(request));

    [HttpGet]
    [Authorize(Policy = "permiso.ver")]
    public async Task<IActionResult> GetPermissionById(int id)
    {
        var permission = await service.GetPermissionByIdAsync(id);
        return permission is null ? NotFound(new { message = "Permiso no encontrado" }) : Ok(permission);
    }

    [HttpPost]
    [Authorize(Policy = "permiso.crear")]
    public async Task<IActionResult> CreatePermission([FromBody] SavePermissionDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { message = "Revise el nombre y la descripción del permiso" });

        var (success, _, error) = await service.CreatePermissionAsync(dto);
        return Mutation((success, error), "Permiso creado correctamente");
    }

    [HttpPost]
    [Authorize(Policy = "permiso.actualizar")]
    public async Task<IActionResult> UpdatePermission([FromBody] SavePermissionDto dto) =>
        ModelState.IsValid
            ? Mutation(await service.UpdatePermissionAsync(dto), "Permiso actualizado correctamente")
            : BadRequest(new { message = "Revise el nombre y la descripción del permiso" });

    [HttpPost]
    [Authorize(Policy = "permiso.borrar")]
    public async Task<IActionResult> DeletePermission(int id) =>
        Mutation(await service.DeletePermissionAsync(id), "Permiso borrado correctamente");

    [HttpGet]
    [Authorize(Policy = "rol.ver")]
    public async Task<IActionResult> Roles() => View(await service.GetAllRolesAsync());

    [HttpPost]
    [Authorize(Policy = "rol.crear")]
    public async Task<IActionResult> CreateRole(CreateRolDto dto)
    {
        if (ModelState.IsValid)
        {
            var (success, _, error) = await service.CreateRoleAsync(dto);
            if (success)
            {
                TempData["Success"] = "Rol creado correctamente";
                return User.HasPermission("rol.ver")
                    ? RedirectToAction(nameof(Roles))
                    : RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", error ?? "No se pudo crear el rol");
        }

        if (!User.HasPermission("rol.ver"))
            return BadRequest(ModelState);

        return View(nameof(Roles), await service.GetAllRolesAsync());
    }

    [HttpGet]
    [Authorize(Policy = "rol.asignar")]
    public async Task<IActionResult> RoleDetails(int id)
    {
        var role = await service.GetRoleByIdAsync(id);
        if (role is null)
            return NotFound();

        ViewBag.AllPermissions = await service.GetAllPermissionsAsync();
        return View(role);
    }

    [HttpPost]
    [Authorize(Policy = "rol.asignar")]
    public async Task<IActionResult> AssignPermissionToRole(int roleId, int permissionId) =>
        Mutation(await service.AssignPermissionToRoleAsync(roleId, permissionId), "Permiso asignado al rol");

    [HttpPost]
    [Authorize(Policy = "rol.asignar")]
    public async Task<IActionResult> RemovePermissionFromRole(int roleId, int permissionId) =>
        Mutation(await service.RemovePermissionFromRoleAsync(roleId, permissionId), "Permiso quitado del rol");

    [HttpGet]
    [Authorize(Policy = "usuario.ver")]
    public async Task<IActionResult> Users() => View(await service.GetAllUsersAsync());

    [HttpGet]
    [Authorize(Policy = "usuario.asignar")]
    public async Task<IActionResult> UserPermissions(int id)
    {
        var user = await service.GetUserPermissionsAsync(id);
        if (user is null)
            return NotFound();

        ViewBag.AllPermissions = await service.GetAllPermissionsAsync();
        ViewBag.AllRoles = User.HasPermission("usuario.cambiar_rol")
            ? await service.GetAllRolesAsync()
            : Array.Empty<RoleWithPermissionDto>();
        return View(user);
    }

    [HttpPost]
    [Authorize(Policy = "usuario.asignar")]
    public async Task<IActionResult> AssignPermissionToUser(int userId, [FromBody] AssignUserPermission dto) =>
        ModelState.IsValid
            ? Mutation(await service.AssignPermissionToUserAsync(userId, dto),
                dto.IsGranted ? "Permiso concedido al usuario" : "Permiso denegado al usuario")
            : BadRequest(new { message = "Seleccione un permiso válido" });

    [HttpPost]
    [Authorize(Policy = "usuario.asignar")]
    public async Task<IActionResult> RemoveUserPermission(int userId, int permissionId) =>
        Mutation(await service.RemoveUserPermissionAsync(userId, permissionId), "Asignación directa quitada");

    [HttpPost]
    [Authorize(Policy = "usuario.cambiar_rol")]
    public async Task<IActionResult> UpdateUserRole(int userId, [FromBody] UpdateUserRoleDto dto) =>
        ModelState.IsValid
            ? Mutation(await service.UpdateUserRoleAsync(userId, dto.RoleId), "Rol del usuario actualizado")
            : BadRequest(new { message = "Seleccione un rol válido" });

    private IActionResult Mutation((bool Success, string? ErrorMessage) result, string message) =>
        result.Success ? Ok(new { success = true, message }) :
            BadRequest(new { success = false, message = result.ErrorMessage });
}
