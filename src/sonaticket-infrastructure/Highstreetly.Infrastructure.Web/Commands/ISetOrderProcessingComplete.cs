using System;

namespace Highstreetly.Infrastructure.Commands
{
    public interface ISetOrderProcessingComplete : ICommand
    {
        Guid OrderId { get; set; }
    }
}