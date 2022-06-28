using FluentAssertions;
using Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder.Operators;
using NUnit.Framework;

namespace Highstreetly.Infrastructure.Web.JsonApiClient.QueryBuilder.Tests
{
    public class QueryBuilderTests
    {
        [Test]
        public void Test1()
        {
            var queryBuilder = new QueryBuilder();

            queryBuilder
                .And(
                    new AndOperator()
                        .Equalz(
                            "a",
                            "b")
                        .Equalz(
                            "c",
                            "d")
                )
                .Not(
                    new NotOperator(
                        new EqualsOperator(
                            "not1",
                            "not2")))
                .Or(
                    new OrOperator()
                        .Equalz(
                            "or1",
                            "or2")
                        .Equalz(
                            "or3",
                            "or4")
                )
                .Includes(
                    "a.b.c",
                    "r.t.y",
                    "8")
                .Or(
                    new OrOperator(
                        new EqualsOperator(
                            "a",
                            "c"),
                        new NotOperator(
                            new EqualsOperator(
                                "not1",
                                "not5"))))
                .Any(
                    new AnyOperator(
                        "PropertyAny",
                        "1",
                        "2",
                        "3"))
                .Equalz(
                    new EqualsOperator(
                        "12",
                        "34"));

            var result = queryBuilder.Build();
            result.Should()
                .NotBeNull();
        }

        [Test]
        public void CanCreateExampleUri()
        {
            var ownerArticlesRevisionsBuilder = new QueryBuilder(
                    model: "owner.articles.revisions",
                    integrationMode:false)
                .GreaterThan(
                    new GreaterThanOperator(
                        "publishTime",
                        "2005-05-05"));

            var ownerArticlesBuilder = new QueryBuilder(
                    model: "owner.articles",
                    integrationMode: false,
                    childQueryBuilder: ownerArticlesRevisionsBuilder)
                .Equalz(
                    new EqualsOperator(
                        "caption",
                        "Two"));

            var queryBuilder = new QueryBuilder(ownerArticlesBuilder, integrationMode: false);

            queryBuilder
                .And(
                    new AndOperator(
                        new OrOperator(
                            new EqualsOperator(
                                "title",
                                "Technology"),
                            new HasOperator("owner.articles")),
                        new NotOperator(
                            new EqualsOperator(
                                "owner.lastName",
                                "null"))))
                .Includes("owner.articles.revisions");

            var result = queryBuilder.Build();

            result.Should()
                .NotBeEmpty();

            result.Should()
                .Be(
                    "?filter=and(or(equals(title,'Technology'),has(owner.articles)),not(equals(owner.lastName,null)))&include=owner.articles.revisions&filter[owner.articles]=equals(caption,'Two')&filter[owner.articles.revisions]=greaterThan(publishTime,'2005-05-05')");
        }
    }
}