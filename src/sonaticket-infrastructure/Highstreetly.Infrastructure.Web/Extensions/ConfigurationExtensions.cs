using System;
using Microsoft.Extensions.Configuration;

namespace Highstreetly.Infrastructure.Extensions
{
    public static class ConfigurationExtensions
    {
        public static string GetConsulUrl(this IConfiguration configuration)
        {
            var connection = configuration["ConsulConfig:address"];
            return connection;
        }

        public static string GetIdsUrl(this IConfiguration configuration)
        {
            var connection = configuration["IdentityServer:Url"];
            var isDev = Environment.GetEnvironmentVariable("env") == "dev";

            if (isDev)
            {
                return string.Format(connection, Environment.GetEnvironmentVariable("IDS"));
            }
            else
            {
                return connection;
            }
        }
        
        public static int GetConsulPort(this IConfiguration configuration)
        {
            var isDev = Environment.GetEnvironmentVariable("env") == "dev";

            if (isDev)
            {
                return 32229;
            }

            return 8500;
        }

        public static string GetConsulHost(this IConfiguration configuration)
        {
            var isDev = Environment.GetEnvironmentVariable("env") == "dev";

            if (isDev)
            {
                return Environment.GetEnvironmentVariable("MINIKUBE_IP");
            }

            return Environment.GetEnvironmentVariable("HOST_IP");
        }
    }
}
