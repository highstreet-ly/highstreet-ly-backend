using System;

namespace Highstreetly.Infrastructure.Commands
{
    public interface ISetOrderProcessing : ICommand
    {
        Guid OrderId { get; set; }
    }
}