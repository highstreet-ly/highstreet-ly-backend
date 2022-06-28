using System;
using System.Collections.Generic;
using JsonApiDotNetCore.Serialization;

namespace Highstreetly.Infrastructure
{
    public sealed class CopyrightResponseMeta : IResponseMeta
    {
        public IReadOnlyDictionary<string, object> GetMeta()
        {
            return new Dictionary<string, object>
            {
                ["copyright"] = $"Copyright (C) {DateTime.Today.Year} Highstreetly ltd.",
                ["Backend-Version"] = $"Version {Environment.GetEnvironmentVariable("BACKEND_VERSION")}",
                ["Timestamp"] = $"Rendered UTC {DateTime.UtcNow.ToFileTimeUtc()}",
            };
        }
    }
}