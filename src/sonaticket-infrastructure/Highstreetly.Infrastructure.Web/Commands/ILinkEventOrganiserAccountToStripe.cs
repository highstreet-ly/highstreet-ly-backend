using System;

namespace Highstreetly.Infrastructure.Commands
{
    public interface ILinkEventOrganiserAccountToStripe : ICommand
    {
        string Code { get; set; }
        Guid EventOrganiserId { get; set; }
        string Scope { get; set; }
    }
}