namespace Highstreetly.Infrastructure.Configuration
{
    public class ApplicationConfiguration
    {
        public bool Dev { get; set; }
        public string[] Cors { get; set; }
        public int Port { get; set; }
    }
}