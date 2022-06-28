using System.Threading.Tasks;
using Highstreetly.Infrastructure.Email;

namespace Highstreetly.Reservations
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailTicketUnAssignedAsync(this IEmailSender emailSender, string email, string name)
        {
            return emailSender.SendEmailAsync(email, "Your Ticket has been unassigned", name);
        }
    }
}