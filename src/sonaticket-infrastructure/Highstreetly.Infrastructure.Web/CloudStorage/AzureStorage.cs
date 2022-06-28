using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Polly;
using Polly.Retry;

namespace Highstreetly.Infrastructure.CloudStorage
{
    public class AzureStorage : IAzureStorage
    {
        private AsyncRetryPolicy _waitForOrder;
        private const string ContainerName = "payloads";

        public AzureStorage()
        {
            _waitForOrder = Policy
                            .Handle<Exception>()
                            .WaitAndRetryAsync(new[]
                                               {
                                                   TimeSpan.FromSeconds(1),
                                                   TimeSpan.FromSeconds(2),
                                                   TimeSpan.FromSeconds(3),
                                                   TimeSpan.FromSeconds(5),
                                                   TimeSpan.FromSeconds(10),
                                               });
        }

        public async Task UploadJsonPayloadToAuzureBlob(string name, string json)
        {
            var connectionString = Environment.GetEnvironmentVariable("PAYLOAD_STORAGE_CONNECTION");
            var container = new BlobContainerClient(connectionString, ContainerName);
            await using var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await container.UploadBlobAsync($"{name}.json", ms);
        }

        public async Task<string> ReadJsonPayloadFromAuzureBlob(string name)
        {
            return await _waitForOrder.ExecuteAsync(async () =>
            {
                var connectionString = Environment.GetEnvironmentVariable("PAYLOAD_STORAGE_CONNECTION");
                var container = new BlobContainerClient(connectionString, ContainerName);
                var client = container.GetBlobClient($"{name}.json");

                await using var s = await client.OpenReadAsync();
                using var sr = new StreamReader(s);
                var result = await sr.ReadToEndAsync();

                return result;
            });
        }
    }
}
