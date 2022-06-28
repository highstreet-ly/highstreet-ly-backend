using System;

namespace Highstreetly.Infrastructure.Commands
{
    public class RegisterB2BUser : IRegisterB2BUser
    {
        public Guid CorrelationId { get; }
        public Guid Id { get; }
        public TimeSpan Delay { get; set; }
        public string TypeInfo { get; set; }
        public Guid SourceId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string UserName { get; set; }
        public string LastName { get; set; }
        public string Redirect { get; set; }
        public string createEventName { get; set; }
        public DateTime createEventStartDate { get; set; }
        public bool Onboarding { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public Guid? CreateEventBusinessTypeId { get; set; }
    }
}