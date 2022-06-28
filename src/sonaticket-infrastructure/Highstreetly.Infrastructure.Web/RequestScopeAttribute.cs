using System;

namespace Highstreetly.Infrastructure
{
    public class RequestScopeAttribute : Attribute
    {
        public string Scope { get; set; }

    }

    public class RequestServiceAttribute : Attribute
    {
        public string Service { get; set; }

    }
}