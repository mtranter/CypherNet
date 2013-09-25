namespace CypherNet.UnitTests
{
    #region

    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Dynamic4Neo.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Queries;
    using Transaction;

    #endregion

    [TestClass]
    public class CypherQueryTests
    {
        [TestMethod]
        public void BuildCypherQuery_WithStartAndMatch_ExecutesCorrectQuery()
        {
            var cypher = new Mock<ICypherClient>();
            cypher.Setup(c => c.ExecuteQuery<TestCypherClause>(It.IsAny<string>())).Returns(() => null);
            var factory = new Mock<ICypherClientFactory>();
            factory.Setup(f => f.Create()).Returns(cypher.Object);
            var query = new FluentCypherQueryBuilder<TestCypherClause>(factory.Object);
            var results = query
                .Start(v => Start.At(v.movie, 1))
                .Match(v => Pattern.Start(v.movie).Incoming("STARED_IN", 1, 5).From(v.actor))
                
                .Return(v => new {v.actor, v.movie})
                .Fetch();

            VerifyCypher(cypher, results.FirstOrDefault(),
                         "START movie=node(1) MATCH (movie)<-[:STARED_IN*1..5]-(actor) RETURN actor as actor, id(actor) as actor__Id, labels(actor) as actor__Labels, movie as movie, id(movie) as movie__Id, labels(movie) as movie__Labels");
        }

        [TestMethod]
        public void BuildCypherQuery_WithSimpleReturn_ExecutesCorrectQuery()
        {
            var cypher = new Mock<ICypherClient>();
            cypher.Setup(c => c.ExecuteQuery<TestCypherClause>(It.IsAny<string>())).Returns(() => null);
            var factory = new Mock<ICypherClientFactory>();
            factory.Setup(f => f.Create()).Returns(cypher.Object);
            var query = new FluentCypherQueryBuilder<TestCypherClause>(factory.Object);
            var results = query
                .Start(v => Start.At(v.movie, 1))
                .Return(v => v.movie)
                .Fetch();

            VerifyCypher(cypher, results.FirstOrDefault(),
                         "START movie=node(1) RETURN movie as movie, id(movie) as movie__Id, labels(movie) as movie__Labels");
        }

        [TestMethod]
        public void BuildCypherQuery_WithSimpleDelete_ExecutesCorrectQuery()
        {
            var cypher = new Mock<ICypherClient>();
            cypher.Setup(c => c.ExecuteQuery<TestCypherClause>(It.IsAny<string>())).Returns(() => null);
            var factory = new Mock<ICypherClientFactory>();
            factory.Setup(f => f.Create()).Returns(cypher.Object);
            var query = new FluentCypherQueryBuilder<TestCypherClause>(factory.Object);
           
            query
                .Start(v => Start.At(v.movie, 1))
                .Match(v => Pattern.Start(v.movie).Incoming("STARED_IN", 1, 5).From(v.actor))

                .Delete(v => v.actor)
                .Execute();

            VerifyCypher(cypher, "START movie=node(1) MATCH (movie)<-[:STARED_IN*1..5]-(actor) DELETE actor");
        }

        [TestMethod]
        public void BuildCypherQuery_WithComplexDelete_ExecutesCorrectQuery()
        {
            var cypher = new Mock<ICypherClient>();
            cypher.Setup(c => c.ExecuteQuery<TestCypherClause>(It.IsAny<string>())).Returns(() => null);
            var factory = new Mock<ICypherClientFactory>();
            factory.Setup(f => f.Create()).Returns(cypher.Object);
            var query = new FluentCypherQueryBuilder<TestCypherClause>(factory.Object);
            query
                .Start(v => Start.At(v.movie, 1))
                .Match(v => Pattern.Start(v.movie).Incoming("STARED_IN", 1, 5).From(v.actor))
                .Delete(v => new { v.actor, v.movie })
                .Execute();

            VerifyCypher(cypher,
                         "START movie=node(1) MATCH (movie)<-[:STARED_IN*1..5]-(actor) DELETE actor, movie");
        }

        [TestMethod]
        public void BuildCypherQuery_UsingSetMethod_ExecutesCorrectQuery()
        {
            var cypher = new Mock<ICypherClient>();
            cypher.Setup(c => c.ExecuteQuery<TestCypherClause>(It.IsAny<string>())).Returns(() => null);
            var factory = new Mock<ICypherClientFactory>();
            factory.Setup(f => f.Create()).Returns(cypher.Object);
            var query = new FluentCypherQueryBuilder<TestCypherClause>(factory.Object);
            var results = query
                .Match(v => Pattern.Start(v.movie, "arthouse"))
                .Update(v => v.movie.Set("requiresSubtitles", "yes"))
                
                .Return(v => new {v.actor, v.movie})
                .Fetch();

            VerifyCypher(cypher, results.FirstOrDefault(),
                         @"MATCH (movie:arthouse) SET movie.requiresSubtitles = ""yes"" RETURN actor as actor, id(actor) as actor__Id, labels(actor) as actor__Labels, movie as movie, id(movie) as movie__Id, labels(movie) as movie__Labels");
        }

        [TestMethod]
        public void BuildCypherQuery_CreateRelationship_ExecutesCorrectQuery()
        {
            var cypher = new Mock<ICypherClient>();
            cypher.Setup(c => c.ExecuteQuery<TestCypherClause>(It.IsAny<string>())).Returns(() => null);
            var factory = new Mock<ICypherClientFactory>();
            factory.Setup(f => f.Create()).Returns(cypher.Object);
            var query = new FluentCypherQueryBuilder<TestCypherClause>(factory.Object);
            var results = query
                .Start(v => Start.At(v.actor, 1).At(v.movie, 2))
                .Create(v => Create.Relationship(v.actor, v.actedIn, "ACTED_IN", v.movie))
                .Return(v => new {v.actedIn})
                .Fetch();

            VerifyCypher(cypher, results.FirstOrDefault(),
                         "START actor=node(1), movie=node(2) CREATE (actor)-[actedIn:ACTED_IN]->(movie) RETURN actedIn as actedIn, id(actedIn) as actedIn__Id, type(actedIn) as actedIn__Type");
        }

        [TestMethod]
        public void BuildCypherQuery_CreateRelationshipWithProperties_ExecutesCorrectQuery()
        {
            var cypher = new Mock<ICypherClient>();
            cypher.Setup(c => c.ExecuteQuery<TestCypherClause>(It.IsAny<string>())).Returns(() => null);
            var factory = new Mock<ICypherClientFactory>();
            factory.Setup(f => f.Create()).Returns(cypher.Object);
            var query = new FluentCypherQueryBuilder<TestCypherClause>(factory.Object);
            var results = query
                .Start(v => Start.At(v.actor, 1).At(v.movie, 2))
                .Create(v => Create.Relationship(v.actor, v.actedIn, "ACTED_IN", new {name = "mark"}, v.movie))
                .Return(v => new {v.actedIn})
                .Fetch();

            VerifyCypher(cypher, results.FirstOrDefault(),
                         @"START actor=node(1), movie=node(2) CREATE (actor)-[actedIn:ACTED_IN {""name"": ""mark""}]->(movie) RETURN actedIn as actedIn, id(actedIn) as actedIn__Id, type(actedIn) as actedIn__Type");
        }

        [TestMethod]
        public void BuildCypherQuery_WithStartMatchWhere_ExecutesCorrectQuery()
        {
            var cypher = new Mock<ICypherClient>();
            cypher.Setup(c => c.ExecuteQuery<TestCypherClause>(It.IsAny<string>())).Returns(() => null);
            var factory = new Mock<ICypherClientFactory>();
            factory.Setup(f => f.Create()).Returns(cypher.Object);
            var query = new FluentCypherQueryBuilder<TestCypherClause>(factory.Object);

            var results = query
                .Start(v => Start.Any(v.movie))
                .Match(v => Pattern.Start(v.movie).Incoming("STARED_IN").From(v.actor))
                .Where(v => v.actor.Get<string>("name") == "Bob Dinero" || v.actor.Get<string>("role") == "Keyser Söze")
                .Return(v => new {v.actor, v.movie})
                .Fetch();

            VerifyCypher(cypher, results.FirstOrDefault(),
                         "START movie=node(*) MATCH (movie)<-[:STARED_IN]-(actor) WHERE ((actor.name = 'Bob Dinero') OR (actor.role = 'Keyser Söze')) RETURN actor as actor, id(actor) as actor__Id, labels(actor) as actor__Labels, movie as movie, id(movie) as movie__Id, labels(movie) as movie__Labels");
        }

        [TestMethod]
        public void BuildCypherQuery_MatchByLabel_ExecutesCorrectQuery()
        {
            var cypher = new Mock<ICypherClient>();
            cypher.Setup(c => c.ExecuteQuery<TestCypherClause>(It.IsAny<string>())).Returns(() => null);
            var factory = new Mock<ICypherClientFactory>();
            factory.Setup(f => f.Create()).Returns(cypher.Object);
            var query = new FluentCypherQueryBuilder<TestCypherClause>(factory.Object);
            var results = query
                .Match(
                       v =>
                       Pattern.Start(v.actor, "METHOD_ACTOR")
                              .Outgoing("STARED_IN")
                              .To()
                              .Outgoing(v.directedBy, "DIRECTED_BY")
                              .To(v.director))
                .Return(v => new {v.actor, v.director})
                .Fetch();

            VerifyCypher(cypher, results.FirstOrDefault(),
                         "MATCH (actor:METHOD_ACTOR)-[:STARED_IN]->()-[directedBy:DIRECTED_BY]->(director) RETURN actor as actor, id(actor) as actor__Id, labels(actor) as actor__Labels, director as director, id(director) as director__Id, labels(director) as director__Labels");
        }

        [TestMethod]
        public void BuildCypherQuery_StartAtNodeN_ExecutesCorrectQuery()
        {
            var cypher = new Mock<ICypherClient>();
            cypher.Setup(c => c.ExecuteQuery<TestCypherClause>(It.IsAny<string>())).Returns(() => null);
            var factory = new Mock<ICypherClientFactory>();
            factory.Setup(f => f.Create()).Returns(cypher.Object);
            var query = new FluentCypherQueryBuilder<TestCypherClause>(factory.Object);
            var results = query
                .Start(v => Start.At(v.actor, 1))
                .Return(v => new {v.actor})
                .OrderBy(p => p.actedIn.Get<int>("fgds"), p => p.actedIn.Get<string>("name"))
                .Skip(2)
                .Limit(1)
                .Fetch();

            VerifyCypher(cypher, results.FirstOrDefault(),
                         "START actor=node(1) RETURN actor as actor, id(actor) as actor__Id, labels(actor) as actor__Labels ORDER BY actedIn.fgds, actedIn.name SKIP 2 LIMIT 1");
        }

        private void VerifyCypher<TResult>(Mock<ICypherClient> mock, TResult proto, string query)
        {
            mock.Verify(
                        c =>
                        c.ExecuteQuery<TResult>(query));
        }

        private void VerifyCypher(Mock<ICypherClient> mock, string query)
        {
            mock.Verify(
                        c =>
                        c.ExecuteCommand(query));
        }
    }
}