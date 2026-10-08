namespace App_Proyecto.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Username { get; set; }

        public string password { get; set; }

        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }

        public string? EmailConfirmToken { get; set; }

        public string RefreshToken { get; set; }

        public DateTime? RefresTokenExpiryTime { get; set; }

        public string? PasswordResetToken { get; set; }

        public DateTime? PasswordResetTokenExpiryTime { get; set; }


        public int RoleId { get; set; }
        public virtual Rol Role { get; set; }

        public virtual ICollection<UsuarioPermiso> UsuarioPermisos { get; set; } = [];









    }
}
