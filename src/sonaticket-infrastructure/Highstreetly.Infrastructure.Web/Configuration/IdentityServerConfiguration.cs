namespace Highstreetly.Infrastructure.Configuration
{
    public class IdentityServerConfiguration
    {
        public string Url { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public int Port { get; set; }
    }
}