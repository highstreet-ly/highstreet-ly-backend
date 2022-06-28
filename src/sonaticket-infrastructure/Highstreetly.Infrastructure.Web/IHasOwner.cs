using System;

namespace Highstreetly.Infrastructure
{
    public interface IHasOwner
    {
        Guid? OwnerId { get; set; }
    }
}