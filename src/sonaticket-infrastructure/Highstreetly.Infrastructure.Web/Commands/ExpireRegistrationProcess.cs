using System;

namespace Highstreetly.Infrastructure.Commands
{
    /// <summary>
    /// rename this  - registration doens't make sense
    /// </summary>
    public class ExpireRegistrationProcess : IExpireRegistrationProcess
    {
        public ExpireRegistrationProcess()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string TypeInfo { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid ProcessId { get; set; }

        public TimeSpan Delay { get; set; }
    }
}