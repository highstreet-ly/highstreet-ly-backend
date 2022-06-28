using System;

namespace Highstreetly.Infrastructure.Commands
{
    public interface IConfirmOrder : ICommand
    {
        Guid OrderId { get; set; }
        string Email { get; set; }
    }
}