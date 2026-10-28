using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Reflection.Metadata;

namespace App_Proyecto.DTOS
{
    public class ForgetPasswordDto
    {
        public string Email { get; set; } = null!;
    }
}
