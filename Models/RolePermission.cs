namespace App_Proyecto.Models
{
    public class RolePermission
    {
        public int RoleId { get; set; }
        public string PermissionId { get; set; } = null!;
        public virtual Rol Role { get; set; }
        public virtual Permission Permission { get; set; } = null!;

    }
}
