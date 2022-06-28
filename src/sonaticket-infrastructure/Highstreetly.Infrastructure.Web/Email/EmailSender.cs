using System;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;

namespace Highstreetly.Infrastructure.Email
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string m, byte[] attachment = null, string templateId = null)
        {
            var message = new SendGridMessage
            {
                From = new EmailAddress($"system@{Environment.GetEnvironmentVariable("A_RECORD")}.{Environment.GetEnvironmentVariable("DOMAIN_TLD")}", Environment.GetEnvironmentVariable("A_RECORD")),
                Subject = subject
            };

            message.AddTo(email);
            message.HtmlContent = m;
            if (templateId != null)
            {
                message.SetTemplateId(templateId);
            }

            if (attachment != null)
            {
                var file = Convert.ToBase64String(attachment);
                message.AddAttachment("ticket.png", file);
            }

            var client = new SendGrid.SendGridClient(Environment.GetEnvironmentVariable("SENGRID_KEY"));
            await client.SendEmailAsync(message);
        }

        public async Task SendEmailAsync(string email, string subject, object data, string templateId, byte[] attachment = null)
        {
            var message = new SendGridMessage
            {
                From = new EmailAddress($"system@{Environment.GetEnvironmentVariable("A_RECORD")}.{Environment.GetEnvironmentVariable("DOMAIN_TLD")}", Environment.GetEnvironmentVariable("A_RECORD")),
                Subject = subject
            };

            message.AddTo(email);
            message.SetTemplateId(templateId);
            message.SetTemplateData(data);
            message.SetSubject(subject);

            if (attachment != null)
            {
                var file = Convert.ToBase64String(attachment);
                message.AddAttachment("ticket.png", file);
            }

            var client = new SendGrid.SendGridClient(Environment.GetEnvironmentVariable("SENGRID_KEY"));
            await client.SendEmailAsync(message);
        }
    }
}