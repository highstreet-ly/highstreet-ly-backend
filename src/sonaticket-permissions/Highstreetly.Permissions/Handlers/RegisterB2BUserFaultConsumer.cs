using System.Threading.Tasks;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.Email;
using MassTransit;
using Newtonsoft.Json;

namespace Highstreetly.Permissions.Handlers
{
    public class RegisterB2BUserFaultConsumer :
        IConsumer<Fault<IRegisterB2BUser>>
    {
        private readonly IEmailSender _emailSender;

        public RegisterB2BUserFaultConsumer(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task Consume(ConsumeContext<Fault<IRegisterB2BUser>> context)
        {
            // delete the event instance
            // delete the event series
            // delete the event organiser
            // delete the chargebee sub

            // send an email to me
            await _emailSender.SendEmailAsync(
                "us@highstreet.ly",
                "FAULT: RegisterB2BUserFaultConsumer",
                $"User failed to register: {JsonConvert.SerializeObject(context.Message.Message)} {JsonConvert.SerializeObject(context.Message.Exceptions)}");

        }
    }
}