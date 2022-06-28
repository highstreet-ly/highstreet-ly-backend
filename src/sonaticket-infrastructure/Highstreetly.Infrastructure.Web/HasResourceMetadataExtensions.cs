namespace Highstreetly.Infrastructure
{
    public static class HasResourceMetadataExtensions
    {
        public static void AddMetadata(this IHasResourceMetadata resource, string name, string value)
        {
            var md = resource.Metadata;

            if (md.ContainsKey(name))
            {
                md[name] = value;
                return;
            }

            md.Add(name, value);
            resource.Metadata = md;
        }
    }
}