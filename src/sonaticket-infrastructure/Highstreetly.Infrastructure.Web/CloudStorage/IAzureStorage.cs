using System;
using System.Threading.Tasks;

namespace Highstreetly.Infrastructure.CloudStorage
{
    public interface IAzureStorage
    {
        Task UploadJsonPayloadToAuzureBlob(string name, string json);
        Task<string> ReadJsonPayloadFromAuzureBlob(string name);
    }
}