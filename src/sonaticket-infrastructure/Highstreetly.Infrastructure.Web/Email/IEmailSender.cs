using System.Threading.Tasks;

namespace Highstreetly.Infrastructure.Email
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, object data, string templateId, byte[] attachment = null);
        Task SendEmailAsync(string email, string subject, string message, byte[] attachment = null, string templateId = null);
    }
}