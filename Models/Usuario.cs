namespace App_Proyecto.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string password { get; set; }

        public string Email { get; set; }

        public string EmailConfirmed { get; set; }

        public string? EmailConfirmedToken { get; set; }

        public string? RefreshToken { get; set; }
        
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public string? PasswordResetToken { get; set; }

        public DateTime? PasswordResetTokenExpiryTime { get; set; } 

        public int? RolId { get; set; }

        public virtual Role Rol { get; set; }

        public ICollection<UsuarioPermission> UsuarioPermissions{ get; set; } = [];

    }
}
