using System;

namespace Highstreetly.Infrastructure.DataBase
{
    /// <summary>
    /// Represents an identifiable entity in the system.
    /// </summary>
    public interface IAggregateRoot
    {
        Guid Id { get; }
    }
}