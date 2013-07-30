namespace CypherNet.UnitTests
{
    #region

    using System.Linq;
    using CypherNet.Queries;
    using Dynamic4Neo.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    #endregion

    [TestClass]
    public class CypherQueryIntegrationTests
    {
        

        [TestMethod]
        public void BuildCypherQuery_WithStartAndMatch_ExecutesCorrectQuery()
        {
            var cypher = new Mock<ICypher>();
            cypher.Setup(c => c.ExecuteQuery<TestCypherClause>(It.IsAny<string>())).Returns(() => null);
            var query = new FluentCypherQueryBuilder<TestCypherClause>(cypher.Object, new TransactionEndpointCypherQueryBuilder());
            var results = query
                .Start(v => Start.At(v.movie, 1))
                .Match(v => Pattern.Start(v.movie).Incoming("STARED_IN", 1, 5).From(v.actor))
                .Return(v => new {v.actor, v.movie})
                .Execute();
            
            VerifyCypher(cypher, results.FirstOrDefault(), "START movie=node(1) MATCH (movie)<-[:STARED_IN*1..5]-(actor) RETURN actor as actor, movie as movie");
        }

        [TestMethod]
        public void BuildCypherQuery_WithStartMatchWhere_ExecutesCorrectQuery()
        {
            var cypher = new Mock<ICypher>();
            cypher.Setup(c => c.ExecuteQuery<TestCypherClause>(It.IsAny<string>())).Returns(() => null);
            var query = new FluentCypherQueryBuilder<TestCypherClause>(cypher.Object, new TransactionEndpointCypherQueryBuilder());
            
            var results = query
                .Start(v => Start.Any(v.movie))
                .Match(v => Pattern.Start(v.movie).Incoming("STARED_IN").From(v.actor))
                .Where(v => v.actor.Prop<string>("name") == "Bob Dinero" || v.actor.Prop<string>("role") == "Keyser Söze")
                .Return(v => new { v.actor, v.movie })
                .Execute();

            VerifyCypher(cypher, results.FirstOrDefault(), "START movie=node(*) MATCH (movie)<-[:STARED_IN]-(actor) WHERE ((actor.name = 'Bob Dinero') OR (actor.role = 'Keyser Söze')) RETURN actor as actor, movie as movie");
        }

        [TestMethod]
        public void BuildCypherQuery_MatchByLabel_ExecutesCorrectQuery()
        {
            var cypher = new Mock<ICypher>();
            cypher.Setup(c => c.ExecuteQuery<TestCypherClause>(It.IsAny<string>())).Returns(() => null);
            var query = new FluentCypherQueryBuilder<TestCypherClause>(cypher.Object, new TransactionEndpointCypherQueryBuilder());
            var results = query
                .Match(v => Pattern.Start(v.actor, "METHOD_ACTOR").Outgoing("STARED_IN").To().Outgoing(v.directedBy, "DIRECTED_BY").To(v.director))
                .Return(v => new { v.actor, v.director })
                .Execute();

            VerifyCypher(cypher, results.FirstOrDefault(), "MATCH (actor:METHOD_ACTOR)-[:STARED_IN]->()-[directedBy:DIRECTED_BY]->(director) RETURN actor as actor, director as director");
        }

        [TestMethod]
        public void BuildCypherQuery_StartAtNodeN_ExecutesCorrectQuery()
        {
            var cypher = new Mock<ICypher>();
            cypher.Setup(c => c.ExecuteQuery<TestCypherClause>(It.IsAny<string>())).Returns(() => null);
            var query = new FluentCypherQueryBuilder<TestCypherClause>(cypher.Object, new TransactionEndpointCypherQueryBuilder());
            var results = query
                .Start(v => Start.At(v.actor, 1))
                .Return(v => new { v.actor})
                .Execute();
                                                           
            VerifyCypher(cypher, results.FirstOrDefault(), "START actor=node(1) RETURN actor as actor");
        }

        void VerifyCypher<TResult>(Mock<ICypher> mock, TResult proto, string query)
        {
            mock.Verify(
              c =>
              c.ExecuteQuery<TResult>(query));
        }
    }
}