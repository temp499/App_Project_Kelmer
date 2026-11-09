using System.ComponentModel.DataAnnotations;

namespace App_Proyecto.DTOS
{
    public class LoginRequestDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
    }
}
