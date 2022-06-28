using System;

namespace Highstreetly.Infrastructure.Commands
{
    /// <summary>
    /// rename this  - registration doens't make sense
    /// </summary>
    public interface IExpireRegistrationProcess : ICommand
    {
        Guid ProcessId { get; set; }
    }
}