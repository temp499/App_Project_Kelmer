namespace App_Proyecto.Services
{
    public interface IEmailService
    {
        void SendPasswordResetEmail(string toEmail, string body);
    }
}
