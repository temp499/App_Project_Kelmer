namespace App_Proyecto.Models
{
    public class UsuarioPermiso
    {
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; } = null!;
        public int PermissionId { get; set; }
        public virtual Permission Permission { get; set; } = null!;
        public bool IsGranted { get; set; }
    }
}
