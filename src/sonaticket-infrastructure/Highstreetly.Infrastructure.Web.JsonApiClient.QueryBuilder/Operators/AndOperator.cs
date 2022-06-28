using System;
using System.Collections.Generic;
using System.Linq;

namespace Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder.Operators
{
    public class AndOperator : IBooleanOperator
    {
        private readonly List<IOperator> _operators = new();

        public AndOperator()
        {

        }

        public AndOperator(params IOperator[] operators)
        {
            _operators.AddRange(operators);
        }

        public AndOperator And(AndOperator and)
        {
            _operators.Add(and);
            return this;
        }

        public AndOperator Any(AnyOperator any)
        {
            _operators.Add(any);
            return this;
        }

        public AndOperator Contains(ContainsOperator contains)
        {
            _operators.Add(contains);
            return this;
        }

        public AndOperator EndsWith(EndsWithOperator endsWith)
        {
            _operators.Add(endsWith);
            return this;
        }

        public AndOperator Equalz(EqualsOperator equals)
        {
            _operators.Add(equals);
            return this;
        }

        public AndOperator Equalz(string property, string value)
        {
            _operators.Add(new EqualsOperator(property, value));
            return this;
        }

        public AndOperator Not(NotOperator and)
        {
            _operators.Add(and);
            return this;
        }

        public AndOperator Or(OrOperator and)
        {
            _operators.Add(and);
            return this;
        }

        public AndOperator Has(HasOperator has)
        {
            _operators.Add(has);
            return this;
        }

        public AndOperator GreaterThan(GreaterThanOperator op)
        {
            _operators.Add(op);
            return this;
        }

        public AndOperator GreaterThanOrEqualTo(GreaterThanOrEqualOperator op)
        {
            _operators.Add(op);
            return this;
        }

        public AndOperator LessThan(LessThanOperator op)
        {
            _operators.Add(op);
            return this;
        }

        public AndOperator LessThanOrEqualTo(LessThanOrEqualOperator op)
        {
            _operators.Add(op);
            return this;
        }

        public override string ToString()
        {
            if (_operators.Count <2)
            {
                throw new ArgumentException($"Cannot use {nameof(AndOperator)} with a single comparison operator");
            }

            var final = string.Join(
                ',',
                _operators.Select(x => x.ToString())
                    .ToArray());

            return $"and({final})";
        }
    }
}