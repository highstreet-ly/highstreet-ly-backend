using System;
using MassTransit;

namespace Highstreetly.Infrastructure
{
    public interface ICommand : CorrelatedBy<Guid>
    {
        Guid Id { get; }
        TimeSpan Delay { get; set; }
        string TypeInfo { get; set; }
    }
}