using System;

namespace Highstreetly.Infrastructure.Commands
{
    public interface ISetEventAsFeatured : ICommand
    {
        Guid SourceId { get; set; }
        bool Published { get; set; }
        string Slug { get; set; }
    }
}