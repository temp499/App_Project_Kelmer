using System.ComponentModel.DataAnnotations;

namespace App_Proyecto.DTOS
{
    public class RegisterRequestDto
    {
        [Required, StringLength(100, MinimumLength = 2)]
        public required string UserName { get; set; }
        [Required, EmailAddress, MaxLength(150)]
        public required string Email { get; set; }
        [Required, StringLength(128, MinimumLength = 8)]
        public required string Password { get; set; }
        public int RoleId { get; set; } = 2;
    }
}
