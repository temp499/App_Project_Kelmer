namespace App_Proyecto.Models
{
    public class Rol
    {
        public int Id { get; set; }

        public string Descripcion { get; set; } = string.Empty;

        public ICollection<Usuario> Usuario { get; set; } = [];

        public  ICollection<RolePermission> RolePermissions { get; set; } = [];

    }
}
