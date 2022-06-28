using System.Collections.Generic;

namespace Highstreetly.Infrastructure
{
    public interface IHasResourceMetadata
    {
        Dictionary<string, string> Metadata { get; set; }
    }
}