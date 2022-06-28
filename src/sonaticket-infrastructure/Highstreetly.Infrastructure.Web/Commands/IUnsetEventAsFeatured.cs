using System;

namespace Highstreetly.Infrastructure.Commands
{
    public interface IUnsetEventAsFeatured : ICommand
    {
        Guid SourceId { get; set; }
    }
}