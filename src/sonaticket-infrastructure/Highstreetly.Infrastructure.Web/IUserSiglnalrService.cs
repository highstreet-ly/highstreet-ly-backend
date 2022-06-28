using System.Threading.Tasks;

namespace Highstreetly.Infrastructure
{
    public interface IUserSiglnalrService
    {
        Task Send(string userId, string message);
    }
}