using System;

namespace Highstreetly.Infrastructure.Commands
{
    public interface IPublishEventInstance : ICommand
    {
        Guid SourceId { get; set; }
        string Slug { get; set; }
        bool Published { get; set; }
    }
}