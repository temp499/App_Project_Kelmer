using System.ComponentModel.DataAnnotations;

namespace App_Proyecto.DTOS
{
    public class ChangePasswordDto
    {
        [Required]
        public required string CurrentPassword { get; set; } = null!;
        [Required, StringLength(128, MinimumLength = 8)]
        public required string NewPassword { get; set; } = null!;
    }
}
