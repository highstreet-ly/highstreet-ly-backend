using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Marten.Events;

namespace Highstreetly.Infrastructure
{
    /// <summary>
    /// Extracts metadata about a payload so that it's placed in the 
    /// message envelope.
    /// </summary>
    public class StandardMetadataProvider : IMetadataProvider
    {
        /// <summary>
        /// Gets metadata associated with the payload, which can be
        /// used by processors to filter and selectively subscribe to
        /// messages.
        /// </summary>
        public virtual IDictionary<string, string> GetMetadata(object payload)
        {
            var metadata = new Dictionary<string, string>();
            var type = payload.GetType();

            // The standard metadata could be used as a sort of partitioning already, 
            // maybe considering different assembly names as being the area/subsystem/bc.

            metadata[StandardMetadata.AssemblyName] = Path.GetFileNameWithoutExtension(type.GetTypeInfo().Assembly.ManifestModule.FullyQualifiedName);
            metadata[StandardMetadata.FullName] = type.FullName;
            metadata[StandardMetadata.Namespace] = type.Namespace;
            metadata[StandardMetadata.TypeName] = type.Name;

            if (payload is IEvent e)
            {
                metadata[StandardMetadata.SourceId] = e.Id.ToString();
                metadata[StandardMetadata.Kind] = StandardMetadata.EventKind;
            }

            if (payload is ICommand c)
            {
                metadata[StandardMetadata.Kind] = StandardMetadata.CommandKind;
            }

            // NOTE: here we may add an "Area" or "Subsystem" or 
            // whatever via .NET custom attributes on the payload 
            // type.

            return metadata;
        }
    }
}