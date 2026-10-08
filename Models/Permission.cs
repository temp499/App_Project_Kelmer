namespace App_Proyecto.Models
{
    public class Permission
    {

        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }

        public ICollection<UsuarioPermiso> UsuarioPermissions { get; set; } = [];
        public ICollection<RolePermission> RolePermissions { get; set; } = [];
    }

    }
