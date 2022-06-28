using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Highstreetly.Reservations.Distance
{
    public class DistanceApi : IDistanceApi
    {
        private IConfiguration _config;
        static readonly HttpClient Client = new HttpClient();
        private readonly ILogger<DistanceApi> _logger;

        public DistanceApi(IConfiguration config, ILoggerFactory loggerFactory)
        {
            _config = config;
            _logger = loggerFactory.CreateLogger<DistanceApi>();
        }


        public async Task<DistanceModel> IsWithinRange(string origin, string destination)
        {
            var apiKey = _config.GetSection("Geo").GetValue<string>("ApiKey");
            _logger.LogInformation($"Distance API using KEY {apiKey}");
            var result = await Client.GetStringAsync(
                $"https://maps.googleapis.com/maps/api/distancematrix/json?units=imperial&origins={origin}&destinations={destination}&key={apiKey}");

            _logger.LogInformation($"Result from distance API: {result}");

            return DistanceModel.FromJson(result);
        }
    }
}