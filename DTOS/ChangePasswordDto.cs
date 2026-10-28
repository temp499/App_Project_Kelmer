namespace App_Proyecto.DTOS
{
    public class ChangePasswordDto
    {
        public required string CurrentPassword { get; set; } = null!;
        public required string NewPassword { get; set; } = null!;
    }
}
