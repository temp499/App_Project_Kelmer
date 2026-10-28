using System.Security.Claims;

namespace App_Proyecto.Extensions
{
    public static class ClaimsPrincipalExtension
    {


        public const string PermissionClaimType = "permission";

        public static bool HasPermission(this ClaimsPrincipal user, string permission)
        {
            return user.Claims.Any(c => c.Type == PermissionClaimType &&
            string.Equals(c.Value, permission, StringComparison.OrdinalIgnoreCase));
        }

        public static bool HasAnyPermission(this ClaimsPrincipal user, params string[] permission)
        {
            return permission.Any(user.HasPermission);
        }

        public static IReadOnlyList<string> GetPermission(this ClaimsPrincipal user)
        {
            return user.Claims
                .Where(c => c.Type == PermissionClaimType)
                .Select(c => c.Value)
                .OrderBy(p => p)
                .ToList();
        }

    }
}
