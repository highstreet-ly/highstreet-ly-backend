using System.Threading.Tasks;

namespace Highstreetly.Permissions
{
    public interface INotificationSenderService
    {
        Task SendForgotPasswordEmail(string email, string url);
        Task SendEmailAlreadyExistsAsync(string email);
        Task SendPasswordWasResetEmail(string userEmail);
        Task SendWelcomeEapEmail(string email, string url);

        Task SendEmailConfirmationAsync(

            string email,
            string userId,
            string code,
            string redirect);
    }
}