namespace App_Proyecto.Models
{
    public class Permission
    {

        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public ICollection <UsuarioPermission> UsuarioPermissions { get; set; } = [];

        public ICollection <RolePermission> RolePermissions { get; set; } = [];

    }
}
