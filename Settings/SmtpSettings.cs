namespace App_Proyecto.Settings
{
    public class SmtpSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public bool Enable { get; set; }
        public int EnableSsl { get; set; }
        public string User { get; set; } = null!;
        public string Password { get; set; } = null!;


    }
}


