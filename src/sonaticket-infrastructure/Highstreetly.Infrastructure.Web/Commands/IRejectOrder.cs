using System;

namespace Highstreetly.Infrastructure.Commands
{
    public interface IRejectOrder : ICommand
    {
        Guid OrderId { get; set; }
    }
}