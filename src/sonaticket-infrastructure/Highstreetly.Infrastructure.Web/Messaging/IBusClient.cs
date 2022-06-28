using System;
using System.Threading.Tasks;
using MassTransit;

namespace Highstreetly.Infrastructure.Messaging
{
    public interface IBusClient
    {
        Task Send<T>(object values)where T : class, CorrelatedBy<Guid>;
        Task Publish<T>(object values) where T : class, CorrelatedBy<Guid>;
    }
}