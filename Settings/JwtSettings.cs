namespace App_Proyecto.Settings
{
    public class JwtSettings
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;

        public String audience { get; set; } = String.Empty;
        public int TokenValidateInMinutes { get; set; } = 10;


    }
}
