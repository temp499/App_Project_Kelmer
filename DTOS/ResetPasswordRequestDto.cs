using System.ComponentModel.DataAnnotations;

namespace App_Proyecto.DTOS
{
    public class ResetPasswordRequestDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string Token { get; set; } = null!;
        [Required, StringLength(128, MinimumLength = 8)]
        public string NewPassword { get; set; } = null!;

    }
}
