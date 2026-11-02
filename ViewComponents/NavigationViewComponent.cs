using System.Security.Claims;
using App_Proyecto.DTOS;
using App_Proyecto.Extensions;
using App_Proyecto.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace App_Proyecto.ViewComponents
{
    public class NavigationViewComponent(IUsuarioRepository userRepository) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = HttpContext.User;
            IReadOnlyList<string> permissions = user.GetPermission();

            if (user.Identity?.IsAuthenticated == true &&
                int.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            {
                var databaseUser = await userRepository.GetUsuarioByIdWithPermissionAsync(userId);
                if (databaseUser is not null)
                    permissions = EffectivePermission.FromUser(databaseUser);
            }

            return View(permissions);
        }
    }
}
