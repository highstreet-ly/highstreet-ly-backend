using System;
using System.Collections.Generic;
using System.Linq;
using Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder.Operators;
using Microsoft.AspNetCore.WebUtilities;

namespace Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder
{
    /// <summary>
    /// TODO: sorting, pagination etc
    /// </summary>
    public class QueryBuilder
    {
        private readonly QueryBuilder _childQueryBuilder;
        private readonly string _model;
        private readonly bool _integrationMode;
        private readonly List<IOperator> _operators = new();
        private readonly List<string> _includes = new();
        private readonly List<SparseFieldSet> _fields = new();

        public QueryBuilder(QueryBuilder childQueryBuilder = null, string model = null,  bool integrationMode = true)
        {
            _childQueryBuilder = childQueryBuilder;
            _model = model;
            _integrationMode = integrationMode;
        }

        public QueryBuilder And(AndOperator and)
        {
            _operators.Add(and);
            return this;
        }

        public QueryBuilder Any(AnyOperator any)
        {
            _operators.Add(any);
            return this;
        }

        public QueryBuilder Contains(ContainsOperator contains)
        {
            _operators.Add(contains);
            return this;
        }

        public QueryBuilder EndsWith(EndsWithOperator endsWith)
        {
            _operators.Add(endsWith);
            return this;
        }

        public QueryBuilder Equalz(EqualsOperator equals)
        {
            _operators.Add(equals);
            return this;
        }

        public QueryBuilder Equalz(string property, string value)
        {
            _operators.Add(new EqualsOperator(property, value));
            return this;
        }

        public QueryBuilder Not(NotOperator and)
        {
            _operators.Add(and);
            return this;
        }

        public QueryBuilder Or(OrOperator and)
        {
            _operators.Add(and);
            return this;
        }

        public QueryBuilder StartsWith(StartsWithOperator startsWith)
        {
            _operators.Add(startsWith);
            return this;
        }

        public QueryBuilder Includes(params string[] includes)
        {
            _includes.AddRange(includes);
            return this;
        }

        public QueryBuilder Has(HasOperator has)
        {
            _operators.Add(has);
            return this;
        }

        public QueryBuilder GreaterThan(GreaterThanOperator op)
        {
            _operators.Add(op);
            return this;
        }

        public QueryBuilder GreaterThanOrEqualTo(GreaterThanOrEqualOperator op)
        {
            _operators.Add(op);
            return this;
        }

        public QueryBuilder LessThan(LessThanOperator op)
        {
            _operators.Add(op);
            return this;
        }

        public QueryBuilder LessThanOrEqualTo(LessThanOrEqualOperator op)
        {
            _operators.Add(op);
            return this;
        }

        public QueryBuilder Fields(
            SparseFieldSet fields)
        {
            _fields.Add(fields);
            return this;
        }

        public string Build(string final = "")
        {
            string compiled;
            var includes = string.Empty;
            var fields = string.Empty;

            var filterPropertyExpression = "filter";

            if (!string.IsNullOrWhiteSpace(_model))
            {
                filterPropertyExpression = $"filter[{_model}]";
            }

            if (_integrationMode)
            {
               compiled = string.Join(
                    '&',
                    _operators.Select(x => $"{filterPropertyExpression}=expr:{x}")
                        .ToArray());
            }
            else
            {
                compiled = string.Join(
                    '&',
                    _operators.Select(x => $"{filterPropertyExpression}={x}")
                        .ToArray());
            }

            if (_includes.Count > 0)
            {
                includes = "include=" + string.Join(
                    ",",
                    _includes);
            }

            if (_fields.Count > 0)
            {
                fields = "fields=" + string.Join(
                    ",",
                    _fields);
            }

            if (!string.IsNullOrWhiteSpace(compiled))
            {
                final += final.HasQuery() ? $"&{compiled}" : $"?{compiled}";
            }

            if (!string.IsNullOrWhiteSpace(includes))
            {
                final += final.HasQuery() ? $"&{includes}" : $"?{includes}";
            }

            if (!string.IsNullOrWhiteSpace(fields))
            {
                final += final.HasQuery() ? $"&{fields}" : $"?{fields}";
            }

            if (_childQueryBuilder != null)
            {
                final = $"{_childQueryBuilder.Build(final)}";
            }
           
            return final;
        }
    }
}