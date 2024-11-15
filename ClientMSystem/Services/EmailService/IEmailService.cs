using ClientMSystem.Models;

namespace ClientMSystem.Services.EmailService
{
    public interface IEmailService
    {
        void SendEmail(EmailDto request);
    }
}
