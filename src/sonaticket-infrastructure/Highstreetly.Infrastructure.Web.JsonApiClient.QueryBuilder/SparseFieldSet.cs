using System.Collections.Generic;

namespace Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder
{
    public class SparseFieldSet
    {
        public SparseFieldSet(string modelName = null, params string[] fields)
        {
            ModelName = modelName;
            Fields.AddRange(fields);
        }

        public string ModelName { get; set; }

        public List<string> Fields { get; set; } = new();

        public override string ToString()
        {
            var fields = $"fields[{ModelName}]={string.Join(',', Fields)}";

            return fields;
        }
    }
}