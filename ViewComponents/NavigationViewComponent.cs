using App_Proyecto.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace App_Proyecto.ViewComponents;

public class NavigationViewComponent : ViewComponent
{
    public IViewComponentResult Invoke() => View(HttpContext.User.GetPermission());
}
