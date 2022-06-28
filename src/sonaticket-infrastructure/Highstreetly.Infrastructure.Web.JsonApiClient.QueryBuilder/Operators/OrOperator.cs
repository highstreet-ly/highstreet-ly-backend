using System;
using System.Collections.Generic;
using System.Linq;

namespace Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder.Operators
{
    public class OrOperator : IBooleanOperator
    {
        private readonly List<IOperator> _operators = new();

        public OrOperator()
        {

        }

        public OrOperator(params IOperator[] operators)
        {
            _operators.AddRange(operators);
        }

        public OrOperator And(AndOperator and)
        {
            _operators.Add(and);
            return this;
        }

        public OrOperator Any(AnyOperator any)
        {
            _operators.Add(any);
            return this;
        }

        public OrOperator Contains(ContainsOperator contains)
        {
            _operators.Add(contains);
            return this;
        }

        public OrOperator EndsWith(EndsWithOperator endsWith)
        {
            _operators.Add(endsWith);
            return this;
        }

        public OrOperator Equalz(EqualsOperator equals)
        {
            _operators.Add(equals);
            return this;
        }

        public OrOperator Equalz(string property, string value)
        {
            _operators.Add(new EqualsOperator(property, value));
            return this;
        }

        public OrOperator Not(NotOperator and)
        {
            _operators.Add(and);
            return this;
        }

        public OrOperator Or(OrOperator and)
        {
            _operators.Add(and);
            return this;
        }

        public OrOperator Has(HasOperator has)
        {
            _operators.Add(has);
            return this;
        }

        public OrOperator GreaterThan(GreaterThanOperator op)
        {
            _operators.Add(op);
            return this;
        }

        public OrOperator GreaterThanOrEqualTo(GreaterThanOrEqualOperator op)
        {
            _operators.Add(op);
            return this;
        }

        public OrOperator LessThan(LessThanOperator op)
        {
            _operators.Add(op);
            return this;
        }

        public OrOperator LessThanOrEqualTo(LessThanOrEqualOperator op)
        {
            _operators.Add(op);
            return this;
        }

        public override string ToString()
        {
            if (_operators.Count < 2)
            {
                throw new ArgumentException($"Cannot use {nameof(OrOperator)} with a single comparison operator");
            }

            var final = string.Join(
                ',',
                _operators.Select(x => x.ToString())
                    .ToArray());

            return $"or({final})";
        }
    }
}