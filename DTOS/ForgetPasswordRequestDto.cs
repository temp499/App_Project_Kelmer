using System.ComponentModel.DataAnnotations;

namespace App_Proyecto.DTOS
{
    public class ForgetPasswordRequestDto
    {
    
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

    }
}
