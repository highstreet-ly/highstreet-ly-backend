using System;

namespace Highstreetly.Infrastructure
{
    public interface IHasEventOrganiser
    {
        Guid EventOrganiserId { get; set; }
    }
}