using System.Threading.Tasks;

namespace Highstreetly.Infrastructure
{
    public interface IEventOrganiserSiglnalrService
    {
        Task Send(string orgId, string message);
    }
}